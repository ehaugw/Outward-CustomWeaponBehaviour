using System;
using TinyHelper;
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

        public static void SetCustomAttackAnimation(Skill skill, Weapon.WeaponType weaponType/*, int millisecondDelay = 0*/)
        {
            var activationEffects = TinyGameObjectManager.MakeFreshObject("ActivationEffects", true, true, skill.transform).transform;
            (activationEffects.gameObject.AddComponent<WeaponSkillAnimationSelector>()).WeaponType = weaponType;
        }
    }
}