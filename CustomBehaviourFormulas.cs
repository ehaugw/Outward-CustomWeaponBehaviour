using MapMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TinyHelper;

namespace CustomWeaponBehaviour
{
    public class CustomBehaviourFormulas
    {
        public static void PostAffectSpeed(Weapon weapon, ref float result)
        {
            if (weapon == null) return;

            float original = result;
            CustomWeaponBehaviour.Instance.bastardBehaviour.PostAffectSpeed(ref weapon, original, ref result); //keep in mind bastard behaviour is disabled when maul shove is active, so you can't stack both
            CustomWeaponBehaviour.Instance.maulShoveBehaviour.PostAffectSpeed(ref weapon, original, ref result);

            // maul shove does not have weapon type compensated speeds like regular attack moves
            if (WeaponManager.Speeds.ContainsKey(weapon.Type) && WeaponManager.Speeds.ContainsKey(BehaviourManager.GetCurrentAnimationType(weapon)) && !CustomWeaponBehaviour.Instance.maulShoveBehaviour.IsActive(weapon))
            {
                result *= WeaponManager.Speeds[BehaviourManager.GetCurrentAnimationType(weapon)] / WeaponManager.Speeds[weapon.Type];
            }

            if (result < 0)
            {
                result = 0;
            }
        }

        public static void PostAffectDamage(ref Weapon _weapon, ref DamageList result)
        {
            var original = result.Clone();
            foreach (var baseDamageModifier in CustomWeaponBehaviour.IBaseDamageModifiers)
            {
                if (baseDamageModifier.Eligible(_weapon))
                {
                    baseDamageModifier.Apply(_weapon, original, ref result);
                }
            }

            CustomWeaponBehaviour.Instance.maulShoveBehaviour.PostAffectDamage(ref _weapon, original, ref result);
            CustomWeaponBehaviour.Instance.bastardBehaviour.PostAffectDamage(ref _weapon, original, ref result);
            CustomWeaponBehaviour.Instance.holyBehaviour.PostAffectDamage(ref _weapon, original, ref result);

            CustomWeaponBehaviour.GeneralWeaponDamageModifiers(_weapon, original, ref result);
        }

        public static void PostAffectImpact(ref Weapon _weapon, ref float _impactDamage)
        {
            float original = _impactDamage;
            CustomWeaponBehaviour.Instance.maulShoveBehaviour.PostAffectImpact(ref _weapon, original, ref _impactDamage);
            CustomWeaponBehaviour.Instance.bastardBehaviour.PostAffectImpact(ref _weapon, original, ref _impactDamage);

            if (_impactDamage < 0) 
            {
                _impactDamage = 0;
            }
        }

        public static void PostAffectStaminaCost(Weapon _weapon, ref float _staminaCost)
        {
            float original = _staminaCost;
            CustomWeaponBehaviour.Instance.bastardBehaviour.PostAffectStaminaCost(ref _weapon, original, ref _staminaCost);

            if (_staminaCost < 0)
            {
                _staminaCost = 0;
            }
        }
    }
}
