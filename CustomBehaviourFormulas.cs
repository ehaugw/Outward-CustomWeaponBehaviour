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
        public static void PostAmplifySpeed(Weapon weapon, ref float result)
        {
            if (weapon == null) return;

            CustomWeaponBehaviour.Instance.bastardBehaviour.BastardPostAmplifySpeed(ref weapon, ref result);
            
            if (WeaponManager.Speeds.ContainsKey(weapon.Type) && WeaponManager.Speeds.ContainsKey(BehaviourManager.GetCurrentAnimationType(weapon)))
                result *= WeaponManager.Speeds[BehaviourManager.GetCurrentAnimationType(weapon)] / WeaponManager.Speeds[weapon.Type];
        }

        public static void PostAmplifyWeaponDamage(ref Weapon _weapon, ref DamageList _damageList)
        {
            _damageList = _damageList.Clone();

            CustomWeaponBehaviour.Instance.bastardBehaviour.BastardPostAmplifyWeaponDamage(ref _weapon, ref _damageList);
            CustomWeaponBehaviour.Instance.holyBehaviour.HolyPostAmplifyWeaponDamage(ref _weapon, ref _damageList);
        }
    }
}
