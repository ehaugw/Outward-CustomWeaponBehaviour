using HarmonyLib;
//using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TinyHelper;
using UnityEngine;

namespace CustomWeaponBehaviour
{

    //SOONER SKILL START
    [HarmonyPatch(typeof(Skill), "HasAllRequirements")]
    public class Skill_HasAllRequirements
    {
        [HarmonyPrefix]
        public static void Prefix(Skill __instance, bool _tryingToActivate/*, out bool __result*/)
        {
            if (_tryingToActivate && __instance is MeleeSkill meleeSkill && __instance.OwnerCharacter.NextAtkAllowed == 2 && CustomWeaponBehaviour.Instance.attackCancelBySkillBehaviour.IsAttackCancelBySkillMode(__instance.OwnerCharacter) && meleeSkill.HasAllRequirements(false))
            {
                __instance.OwnerCharacter.ForceCancel(true, true);//backToLoco must be true
                At.SetValue(true, typeof(Character), __instance.OwnerCharacter, "m_inLocomotion");
                At.SetValue(true, typeof(Character), __instance.OwnerCharacter, "m_nextIsLocomotion");
            }
        }
    }

    //SOONER BLOCK START
    [HarmonyPatch(typeof(Character), "BlockInput")]
    public class Character_BlockInput
    {
        [HarmonyPrefix]
        public static bool Prefix(Character __instance, ref bool _active)
        {
            if (__instance.IsAI) return true; //for some reason, AI doesn't need to be able to block
            if (!__instance.IsPhotonPlayerLocal || __instance.Sheathing || __instance.PreparingToSleep || __instance.Dodging || __instance.CurrentSpellCast == Character.SpellCastType.Flamethrower || __instance.CurrentSpellCast == Character.SpellCastType.PushKick) return true;
            if (!_active || (__instance.InLocomotion && __instance.NextIsLocomotion)) return true;
            if (__instance.ShieldEquipped || __instance.CurrentWeapon == null || !CustomWeaponBehaviour.Instance.attackCancelByBlockBehaviour.IsBlockMode(__instance)) return true;

            int m_dodgeAllowedInAction = (int)At.GetValue(typeof(Character), __instance, "m_dodgeAllowedInAction");

            if (m_dodgeAllowedInAction > 0 && __instance.NextAtkAllowed > 0)
            {
                __instance.photonView.RPC("SendBlockStateTrivial", PhotonTargets.All, new object[] { true });
                __instance.StealthInput(false);
                return false;
            }
            return true;
        }
    }

    //INITIATE CUSTOM MOVESET
    [HarmonyLib.HarmonyPatch(typeof(Character), "SendPerformAttackTrivial", new Type[] { typeof(int), typeof(int) })]
    public class Character_SendPerformAttackTrivial
    {
        [HarmonyPrefix]
        public static void Prefix(Character __instance, ref int _type, ref int _id)
        {
            BehaviourManager.AdaptGrip(__instance, ref _type, ref _id);
        }
    }

    //TERMINATE CUSTOM MOVESET
    [HarmonyLib.HarmonyPatch(typeof(Character), "HitEnded")]
    public class Character_HitEnded
    {
        [HarmonyPrefix]
        public static void Prefix(Character __instance, ref int _attackID)
        {
            if (_attackID != -2 && !__instance.IsCasting)
            {
                CustomWeaponBehaviour.ResetGrip(__instance);
            }
        }
    }

    //TERMINATE CUSTOM MOVESET
    [HarmonyLib.HarmonyPatch(typeof(Character), "StopLocomotionAction")]
    public class Character_StopLocomotionAction
    {
        [HarmonyPrefix]
        public static void Prefix(Character __instance)
        {
            CustomWeaponBehaviour.ResetGrip(__instance);
        }
    }

    //PARRY
    [HarmonyLib.HarmonyPatch(typeof(Character), "ReceiveBlock", new Type[] { typeof(UnityEngine.MonoBehaviour), typeof(float), typeof(Vector3), typeof(float), typeof(float), typeof(Character), typeof(float) })]
    public class Character_ReceiveBlock
    {
        [HarmonyPrefix]
        public static void Prefix(Character __instance, Character _dealerChar, Vector3 _hitDir, ref float _knockBack)
        {
            if (CustomWeaponBehaviour.Instance.parryBehaviour.DidSuccessfulParry(__instance.CurrentWeapon))
            {
                _knockBack -= (__instance?.CurrentWeapon?.Impact ?? 0);
                if (_knockBack < 0)
                {
                    _knockBack = 0;
                }
                CustomWeaponBehaviour.Instance.parryBehaviour.DeliverParry(__instance, _dealerChar, _hitDir, (__instance?.CurrentWeapon?.Impact ?? 0));
            }
        }
    }

    //ENABLE MAIN HANDS TO COUNT AS OTHER TYPES FOR SKILLS
    [HarmonyLib.HarmonyPatch(typeof(AttackSkill), "OwnerHasAllRequiredItems")]
    public class AttackSkill_OwnerHasAllRequiredItemsMainHand
    {
        [HarmonyPrefix]
        public static void Prefix(AttackSkill __instance, out List<Weapon.WeaponType> __state)
        {
            __state = null;

            if (__instance?.OwnerCharacter?.CurrentWeapon is Weapon _weapon)
            {
                if (__instance.RequiredWeaponTypes != null)
                {
                    if (BastardBehaviour.GetBastardType(_weapon.Type) is Weapon.WeaponType bastardType && __instance.RequiredWeaponTypes.Contains(bastardType) && !__instance.RequiredWeaponTypes.Contains(_weapon.Type) && CustomWeaponBehaviour.Instance.bastardBehaviour.IsBastardMode(_weapon))
                    {
                        __state = __instance.RequiredWeaponTypes;
                        __instance.RequiredWeaponTypes = new List<Weapon.WeaponType>(__state);
                        __instance.RequiredWeaponTypes.Add(_weapon.Type);
                    }
                }
            }
        }

        public static void Postfix(AttackSkill __instance, List<Weapon.WeaponType> __state)
        {
            if (__state != null) __instance.RequiredWeaponTypes = __state;
        }
    }

    //ENABLE OFF HANDS TO COUNT AS OTHER TYPES FOR SKILLS
    [HarmonyLib.HarmonyPatch(typeof(AttackSkill), "OwnerHasAllRequiredItems")]
    public class AttackSkill_OwnerHasAllRequiredItemsOffHand
    {
        [HarmonyPrefix]
        public static void Prefix(AttackSkill __instance, out Tuple<List<Weapon.WeaponType>, Equipment> __state)
        {
            __state = null;
            if (__instance?.OwnerCharacter is Character character && character.LeftHandWeapon is Weapon _weapon)
            {
                if (__instance.RequiredOffHandTypes != null)
                {
                    if (__instance.RequiredOffHandTypes.Contains(Weapon.WeaponType.Dagger_OH) && _weapon.HasTag(CustomWeaponBehaviour.PointyTag))
                    {
                        __state = new Tuple<List<Weapon.WeaponType>, Equipment>(__instance.RequiredOffHandTypes, character.LeftHandWeapon ?? null);

                        __instance.RequiredOffHandTypes = new List<Weapon.WeaponType>(__state.Item1);
                        __instance.RequiredOffHandTypes.Add(_weapon.Type);

                        At.SetValue(_weapon, typeof(Character), character, "m_leftHandEquipment");

                    }
                }
            }
        }

        public static void Postfix(AttackSkill __instance, Tuple<List<Weapon.WeaponType>, Equipment> __state)
        {
            if (__state != null)
            {
                __instance.RequiredOffHandTypes = __state.Item1;
                At.SetValue(__state.Item2, typeof(Character), __instance?.OwnerCharacter, "m_leftHandEquipment");

            }
        }
    }

    //DOES NOT CHANGE DAMAGE, ONLY ENABLES THE SKILL TO BUILD DAMAGE FOR A CUSTOM WEAPON TYPE
    [HarmonyLib.HarmonyPatch(typeof(WeaponDamage), nameof(WeaponDamage.BuildDamage), new Type[] { typeof(Character), typeof(DamageList), typeof(float) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref })]
    public class WeaponDamage_BuildDamage
    {
        [HarmonyPrefix]
        public static void Prefix(WeaponDamage __instance, ref AttackSkill ___m_attackSkill, out List<Weapon.WeaponType> __state)
        {
            __state = null;
            if (___m_attackSkill?.OwnerCharacter?.CurrentWeapon is Weapon _weapon)
            {
                if (___m_attackSkill.RequiredWeaponTypes != null && BastardBehaviour.GetBastardType(_weapon.Type) is Weapon.WeaponType bastardType)
                {
                    if (___m_attackSkill.RequiredWeaponTypes.Contains(bastardType) && !___m_attackSkill.RequiredWeaponTypes.Contains(_weapon.Type) && CustomWeaponBehaviour.Instance.bastardBehaviour.IsBastardMode(_weapon))
                    {
                        __state = ___m_attackSkill.RequiredWeaponTypes;
                        ___m_attackSkill.RequiredWeaponTypes = new List<Weapon.WeaponType>(__state) { _weapon.Type };
                    }
                }
            }
        }

        public static void Postfix(AttackSkill ___m_attackSkill, List<Weapon.WeaponType> __state)
        {
            if (__state != null && ___m_attackSkill != null) ___m_attackSkill.RequiredWeaponTypes = __state;
        }
    }
    
    //WIP FOR ENABLING FISTS TO BE USED WITH DAGGER SKILLS
    [HarmonyLib.HarmonyPatch(typeof(AttackSkill), "OwnerHasAllRequiredItems")]
    public class AttackSkill_OwnerHasAllRequiredItemsFist
    {
        [HarmonyPrefix]
        public static void Prefix(AttackSkill __instance, out Tuple<List<Weapon.WeaponType>, Equipment> __state)
        {
            __state = null;
            if (__instance?.OwnerCharacter is Character character && character.CurrentWeapon is Weapon _weapon)
            {
                if (__instance.RequiredOffHandTypes != null)
                {
                    if (__instance.RequiredOffHandTypes.Contains(Weapon.WeaponType.Dagger_OH) && _weapon.HasTag(CustomWeaponBehaviour.PointyTag))
                    {
                        __state = new Tuple<List<Weapon.WeaponType>, Equipment>(__instance.RequiredOffHandTypes, character.LeftHandWeapon ?? null);

                        __instance.RequiredOffHandTypes = new List<Weapon.WeaponType>(__state.Item1);
                        __instance.RequiredOffHandTypes.Add(_weapon.Type);

                        At.SetValue(_weapon, typeof(Character), character, "m_leftHandEquipment");

                    }
                }
            }
        }

        public static void Postfix(AttackSkill __instance, Tuple<List<Weapon.WeaponType>, Equipment> __state)
        {
            if (__state != null)
            {
                __instance.RequiredOffHandTypes = __state.Item1;
                At.SetValue(__state.Item2, typeof(Character), __instance?.OwnerCharacter, "m_leftHandEquipment");

            }
        }
    }

    //EXHAUSTIVELY USED FOR CUSTOM MOVESET SPEEDS.
    //WORKS AS INTENDED
    [HarmonyLib.HarmonyPatch(typeof(CharacterStats), nameof(CharacterStats.GetAmplifiedAttackSpeed))]
    public class CharacterStats_GetAmplifiedAttackSpeed
    {
        [HarmonyPostfix]
        public static void Postfix(CharacterStats __instance, ref float __result, ref Character ___m_character)
        {
            Weapon weapon = ___m_character?.CurrentWeapon;

            if (weapon == null) return;

            CustomBehaviourFormulas.PostAffectSpeed(weapon, ref __result);
        }
    }

    [HarmonyPatch(typeof(WeaponStats), nameof(WeaponStats.GetAttackStamCost))]
    public class WeaponStats_GetAttackStamCost
    {
        [HarmonyPostfix]
        public static void Postfix(WeaponStats __instance, Item ___m_item, ref float __result)
        {
            if (___m_item is Weapon weapon)
            {
                CustomBehaviourFormulas.PostAffectStaminaCost(weapon, ref __result);
            }
        }

    }

    //MANIPULATE IMPACT AT THE MOST BASIC LEVEL
    [HarmonyLib.HarmonyPatch(typeof(Weapon), "BaseImpact", MethodType.Getter)]
    public class Weapon_BaseImpact
    {
        [HarmonyPostfix]
        public static void Postfix(Weapon __instance, ref float __result)
        {
            CustomBehaviourFormulas.PostAffectImpact(ref __instance, ref __result);
        }
    }

    //This patch causes GetAttackImpact to calculate its impact based on Stats.Impact rather than using cached values. Overrides value from the original function entirely and may break other mods
    [HarmonyLib.HarmonyPatch(typeof(WeaponStats), nameof(WeaponStats.GetAttackImpact))]
    public class WeaponStats_GetAttackImpact
    {
        [HarmonyPostfix]
        public static void Postfix(WeaponStats __instance, int _attackID, ref float __result, Item ___m_item)
        {
            var weapon = (Weapon)___m_item;
            __result = weapon.Impact * WeaponStatData.WeaponBaseDataDict[weapon.Type].ImpactMult[_attackID];
        }
    }

    //ALTERNATE WEAPON DAMAMAGE AT THE MOST BASIC LEVEL
    [HarmonyLib.HarmonyPatch(typeof(Weapon), nameof(Weapon.Damage), MethodType.Getter)]
    public class Weapon_Damage
    {
        [HarmonyPostfix]
        public static void Postfix(Weapon __instance, ref DamageList __result)
        {
            __result = __result.Clone();
            CustomBehaviourFormulas.PostAffectDamage(ref __instance, ref __result);
        }
    }

    //This patch causes GetAttackDamage to calculate its damage based on Stats.Damage rather than using cached values. Overrides value from the original function entirely and may break other mods
    [HarmonyLib.HarmonyPatch(typeof(WeaponStats), nameof(WeaponStats.GetAttackDamage))]
    public class WeaponStats_GetAttackDamage
    {
        [HarmonyPostfix]
        public static void Postfix(WeaponStats __instance, int _attackID, ref IList<float> __result, Item ___m_item)
        {
            var weapon = (Weapon)___m_item;
            var damageList = weapon.Damage;

            var damageMult = WeaponStatData.WeaponBaseDataDict[weapon.Type].DamageMult;
            if (_attackID < 0 || _attackID >= damageMult.Length)
            {
                _attackID = 0;
            }

            for (int i = 0; i < damageList.Count && i < __result.Count; i++)
            {
                __result[i] = damageList[i].Damage * damageMult[_attackID];
            }
        }
    }

    public class WeaponStatData
    {
        public static Dictionary<Weapon.WeaponType, WeaponStatData> WeaponBaseDataDict = new Dictionary<Weapon.WeaponType, WeaponStatData>()
        {
           {
                Weapon.WeaponType.Sword_1H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.495f, 1.265f, 1.265f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 1.3f, 1.1f, 1.1f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.2f, 1.1f, 1.1f }
                }
            },
            {
                Weapon.WeaponType.Sword_2H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.5f, 1.265f, 1.265f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 1.5f, 1.1f, 1.1f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.3f, 1.1f, 1.1f }
                }
            },
            {
                Weapon.WeaponType.Axe_1H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.2f, 1.2f, 1.2f }
                }
            },
            {
                Weapon.WeaponType.Axe_2H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.375f, 1.375f, 1.35f }
                }
            },
            {
                Weapon.WeaponType.Mace_1H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 2.5f, 1.3f, 1.3f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f }
                }
            },
            {
                Weapon.WeaponType.Mace_2H,
                new WeaponStatData()
                {   //                         1     2     3      4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 0.75f, 1.4f, 1.4f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 2.0f,  1.4f, 1.4f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.2f,  1.2f, 1.2f }
                }
            },
            {
                Weapon.WeaponType.Halberd_2H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.7f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.7f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.25f, 1.25f, 1.75f }
                }
            },
            {
                Weapon.WeaponType.Spear_2H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.4f, 1.3f, 1.2f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 1.2f, 1.2f, 1.1f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.25f, 1.25f, 1.25f }
                }
            },
            {
                Weapon.WeaponType.FistW_2H,
                new WeaponStatData()
                {   //                         1     2     3     4     5
                    DamageMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f },
                    ImpactMult = new float[] { 1.0f, 1.0f, 1.3f, 1.3f, 1.3f },
                    StamMult   = new float[] { 1.0f, 1.0f, 1.3f, 1.2f, 1.2f }
                }
            }
        };


        public float[] DamageMult;
        public float[] ImpactMult;
        public float[] StamMult;
    }
}
