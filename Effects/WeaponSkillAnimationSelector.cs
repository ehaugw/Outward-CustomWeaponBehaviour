using System;
using UnityEngine;

namespace CustomWeaponBehaviour
{
    public class WeaponSkillAnimationSelector : Effect
    {
        public Weapon.WeaponType WeaponType;

        protected override void ActivateLocally(Character character, object[] _infos)
        {
            CustomWeaponBehaviour.ChangeGrip(character, WeaponType);
        }

        public static Boolean SetCustomAttackAnimation(Skill skill, Weapon.WeaponType weaponType/*, int millisecondDelay = 0*/)
        {
            if (skill.transform.Find("ActivationEffects") is Transform activationEffects)
            {
                (activationEffects.gameObject.AddComponent<WeaponSkillAnimationSelector>()).WeaponType = weaponType;
                return true;
            }
            return false;
        }
    }
}