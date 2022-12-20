﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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

    [HarmonyLib.HarmonyPatch(typeof(Character), "SendPerformAttackTrivial", new Type[] { typeof(int), typeof(int) })]
    public class Character_SendPerformAttackTrivial
    {
        [HarmonyPrefix]
        public static void Prefix(Character __instance, ref int _type, ref int _id)
        {
            BehaviourManager.AdaptGrip(__instance, ref _type, ref _id);
        }

        //[HarmonyPostfix]
        //public static void Postfix(Character __instance)
        //{
        //    __instance.SendMessage("PerformAttack", SendMessageOptions.DontRequireReceiver);
        //    //__instance.Animator.Play("HumanSkillAxeLeap_a");
        //}
    }

    [HarmonyLib.HarmonyPatch(typeof(Character), "ReceiveBlock", new Type[] { typeof(UnityEngine.MonoBehaviour), typeof(float), typeof(Vector3), typeof(float), typeof(float), typeof(Character), typeof(float) })]
    public class Character_ReceiveBlock
    {
        [HarmonyPrefix]
        public static void Prefix(Character __instance, Character _dealerChar, Vector3 _hitDir, ref float _knockBack)
        {
            _knockBack /= 2;

            if (CustomWeaponBehaviour.Instance.parryBehaviour.DidSuccessfulParry(__instance.CurrentWeapon))
            {
                CustomWeaponBehaviour.Instance.parryBehaviour.DeliverParry(__instance, _dealerChar, _hitDir, _knockBack);
            }
        }
    }

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

            //OLD VERSION BUGGED WITH MULTI HIT SKILLS
            //Debug.Log("hit ended for " + _attackID);
            //Console.WriteLine("hit ended for " + _attackID);
            ////skills that hit once has _attackID 0
            ////chakram skill has 0 then -2
            ////feral strikes has -1 twice
            //if (_attackID != -2)
            //{
            //    CustomWeaponBehaviour.ResetGrip(__instance);

            //    if (BehaviourManager.SpellHasAttackAnimation(__instance))
            //    {
            //        At.Call(__instance, "CastDone", new object[] { });
            //    }
            //}
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Character), "StopLocomotionAction")]
    public class Character_StopLocomotionAction
    {
        [HarmonyPrefix]
        public static void Prefix(Character __instance)
        {
            CustomWeaponBehaviour.ResetGrip(__instance);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(AttackSkill), "OwnerHasAllRequiredItems")]
    public class AttackSkill_OwnerHasAllRequiredItems
    {
        [HarmonyPrefix]
        public static void Prefix(AttackSkill __instance, out List<Weapon.WeaponType> __state)
        {
            __state = null;

            if (__instance?.OwnerCharacter?.CurrentWeapon is Weapon _weapon)
            {
                if (__instance.RequiredWeaponTypes != null && BastardBehaviour.GetBastardType(_weapon.Type) is Weapon.WeaponType bastardType)
                {
                    if (__instance.RequiredWeaponTypes.Contains(bastardType) && !__instance.RequiredWeaponTypes.Contains(_weapon.Type) && CustomWeaponBehaviour.Instance.bastardBehaviour.IsBastardMode(_weapon))
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

    [HarmonyLib.HarmonyPatch(typeof(CharacterStats), "GetAmplifiedAttackSpeed")]
    public class CharacterStats_GetAmplifiedAttackSpeed
    {
        [HarmonyPostfix]
        public static void Postfix(CharacterStats __instance, ref float __result, ref Character ___m_character)
        {
            Weapon weapon = ___m_character?.CurrentWeapon;

            if (weapon == null) return;

            CustomBehaviourFormulas.PostAmplifySpeed(weapon, ref __result);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Weapon), "Damage", MethodType.Getter)]
    public class Weapon_Damage
    {
        [HarmonyPostfix]
        public static void Postfix(Weapon __instance, ref DamageList __result)
        {
            CustomBehaviourFormulas.PostAmplifyWeaponDamage(ref __instance, ref __result);
        }
    }

    //Wind Imbue Safe
    [HarmonyLib.HarmonyPatch(typeof(Weapon), "BaseImpact", MethodType.Getter)]
    public class Weapon_BaseImpact
    {
        [HarmonyPostfix]
        public static void Postfix(Weapon __instance, ref float __result)
        {
            CustomBehaviourFormulas.PostAmplifyWeaponImpact(ref __instance, ref __result);
        }
    }

    //Wind Imbue Safe
    //Gets the the damage of the weapon, excluding imbues
    [HarmonyLib.HarmonyPatch(typeof(Weapon), "GetDamage")]
    public class Weapon_GetDamage
    {
        [HarmonyPrefix]
        public static void Prefix(Weapon __instance, int _attackID, out List<float> __state)
        {
            __state = null;

            if (__instance.Stats?.Attacks != null)
            {
                int index = (_attackID > __instance.Stats.Attacks.Length - 1 || _attackID < 0) ? 0 : _attackID;
                var old = __instance.Stats.Attacks[index].Damage;


                bool changed = false;

                if (CustomWeaponBehaviour.Instance.bastardBehaviour.IsBastardMode(__instance) || CustomWeaponBehaviour.Instance.holyBehaviour.IsHolyWeaponMode(__instance) || CustomWeaponBehaviour.Instance.maulShoveBehaviour.IsMaulShoveMode(__instance))
                {
                    changed = true;
                    __instance.Stats.Attacks[index].Damage = __instance.Damage.ToDamageArray().ToList();
                }

                if (changed)
                {
                    __state = old;
                }
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Weapon __instance, int _attackID, List<float> __state, ref DamageList __result)
        {
            if (__state != null)
            {
                int index = (_attackID > __instance.Stats.Attacks.Length - 1 || _attackID < 0) ? 0 : _attackID;
                __instance.Stats.Attacks[index].Damage = __state;

            }
        }
    }

    //Wind Imbue Safe
    //Gets the the damage of the weapon, excluding imbues
    [HarmonyLib.HarmonyPatch(typeof(Weapon), "GetKnockback")]
    public class Weapon_GetKnockback
    {
        [HarmonyPrefix]
        public static void Prefix(Weapon __instance, int _attackID, out float? __state)
        {
            __state = null;

            if (__instance.Stats?.Attacks != null)
            {
                int index = (_attackID > __instance.Stats.Attacks.Length - 1 || _attackID < 0) ? 0 : _attackID;
                var old = __instance.Stats.Attacks[index].Knockback;


                bool changed = false;

                if (CustomWeaponBehaviour.Instance.bastardBehaviour.IsBastardMode(__instance) || CustomWeaponBehaviour.Instance.holyBehaviour.IsHolyWeaponMode(__instance) || CustomWeaponBehaviour.Instance.maulShoveBehaviour.IsMaulShoveMode(__instance))
                {
                    changed = true;
                    __instance.Stats.Attacks[index].Knockback = __instance.Impact;
                }

                if (changed)
                {
                    __state = old;
                }
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Weapon __instance, int _attackID, float? __state, ref float __result)
        {
            if (__state != null)
            {
                int index = (_attackID > __instance.Stats.Attacks.Length - 1 || _attackID < 0) ? 0 : _attackID;
                __instance.Stats.Attacks[index].Knockback = (float)__state;

            }
        }
    }

    //Wind Imbue Safe
    [HarmonyLib.HarmonyPatch(typeof(WeaponDamage), "BuildDamage", new Type[] { typeof(Character), typeof(DamageList), typeof(float) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref })]
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
}
