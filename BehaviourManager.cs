using SideLoader;
using TinyHelper;
using UnityEngine;

namespace CustomWeaponBehaviour
{
    public class BehaviourManager
    {
        public static Weapon.WeaponType GetCurrentAnimationType(Weapon weapon)
        {
            if (weapon.IsEquipped && weapon.OwnerCharacter?.Animator is Animator animator)
            {
                return (Weapon.WeaponType) animator.GetInteger("WeaponType");
            }
            return weapon.Type;
        }

        public static bool IsComboAttack(Character __instance, int _type, int _id)
        {
            if (_type == 0) return true;
            if (_type == 1) return __instance.NextAtkAllowed == 2;
            return false;
        }

        public static bool AdaptGrip(Character __instance, ref int _type, ref int _id)
        {
            Weapon _weapon = __instance?.CurrentWeapon;

            if (_weapon == null) return false;

            if (_type == 2502 || _type == 2511)
            {
                if (SideLoader.At.GetField<Character>(__instance, "m_lastUsedSkill") is MeleeSkill skill &&
                    (
                        skill.ActivateEffectAnimType == Character.SpellCastType.WeaponSkill1 ||
                        skill.ActivateEffectAnimType == Character.SpellCastType.WeaponSkill2
                    )
                )
                {
                    //Weapon skills with _type == 2502 are the "weapon skills" that takes only one type of weapon. My patch temporarly appends new weapon types that would get other animations from animator, unless we change grip to the original (first) weapon type.
                    if (skill.RequiredWeaponTypes[0] != _weapon.Type)
                    {
                        CustomWeaponBehaviour.ChangeGrip(__instance, skill.RequiredWeaponTypes[0]);
                        return true;
                    }
                }
            }


            if (CustomWeaponBehaviour.Instance.finesseBehaviour.IsFinesseMode(_weapon))
            {
                if (_type == 1 && !IsComboAttack(__instance, _type, _id))
                {
                    CustomWeaponBehaviour.ChangeGrip(__instance, Weapon.WeaponType.Axe_1H);
                    return true;
                }

            }

            if (CustomWeaponBehaviour.Instance.halfHandedBehaviour.IsHalfHandedMode(_weapon))
            {
                if (_type == 1 && !IsComboAttack(__instance, _type, _id))
                {
                    _type = 0;
                    CustomWeaponBehaviour.ChangeGrip(__instance, Weapon.WeaponType.Spear_2H);
                    return true;
                }
            }

            if (CustomWeaponBehaviour.Instance.maulShoveBehaviour.IsMaulShoveMode(_weapon))
            {
                if (_type == 1 && !IsComboAttack(__instance, _type, _id))
                {
                    CustomWeaponBehaviour.ChangeGrip(__instance, Weapon.WeaponType.Mace_2H);
                    //_weapon.Unblockable
                    return true;
                }
            }

            if (CustomWeaponBehaviour.Instance.bastardBehaviour.IsBastardMode(_weapon))
            {
                if (BastardBehaviour.GetBastardType(_weapon.Type) is Weapon.WeaponType bastardType)
                {
                    if (new int[] {0, 1, 2500}.Contains(_type)) //Basic attacks and predator leap
                    {
                        CustomWeaponBehaviour.ChangeGrip(__instance, bastardType);
                        if (__instance?.LeftHandEquipment is Equipment item && item.HasTag(CustomWeaponBehaviour.HandsFreeTag) && item.HasTag(CustomWeaponBehaviour.LanternTag) && item.IKType == Equipment.IKMode.Lantern)
                            item.IKType = Equipment.IKMode.None;
                        return true;
                    }

                    ////This will cause skill to not apply damage
                    //if (_type == 2502 && bastardType == Weapon.WeaponType.Sword_2H)
                    //{
                    //    CustomWeaponBehaviour.ChangeGrip(__instance, Weapon.WeaponType.Spear_2H);
                    //    _type = 1;
                    //    return true;
                    //}
                }
            }

            return false;

        }

        public static bool SpellHasAttackAnimation(Character character)
        {
            if (character.IsCasting)
            {
                if (character.Animator.GetInteger("AttackID") < 2)
                {
                    return true;
                }
            }
            return false;

            //return character.IsCasting && character.LastUsedSkill.ItemID == IDs.punctureID;
        }

        public static bool AttackHasSpellAnimation(Character character)
        {
            if (!character.IsCasting)
            {
                if (character.Animator.GetInteger("AttackID") >= 2)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
