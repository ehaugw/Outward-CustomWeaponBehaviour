using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;

namespace CustomWeaponBehaviour
{
    public class MaulShoveModifier : IMaulShoveModifier
    {
        public void ApplyDamageModifier(Weapon weapon, DamageList original, ref DamageList result)
        {
            result += original * -0.5f;
        }

        public void ApplyImpactModifier(Weapon weapon, float original, ref float result)
        {
            switch (weapon.Type)
            {
                case Weapon.WeaponType.Sword_1H:
                    result += original * (WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Mace_2H] / WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Sword_1H] * 2f - 1);
                    break;
                case Weapon.WeaponType.Sword_2H:
                    result += original * (WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Mace_2H] / WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Sword_2H] * 1.18f - 1);
                    break;
                case Weapon.WeaponType.Axe_1H:
                    result += original * (WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Mace_2H] / WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Axe_1H] * 1.9f - 1);
                    break;
                case Weapon.WeaponType.Axe_2H:
                    result += original * (WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Mace_2H] / WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Axe_2H] * 1.29f - 1);
                    break;
                case Weapon.WeaponType.Mace_1H:
                    result += original * (WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Mace_2H] / WeaponManager.HeavyImpactModifiers[Weapon.WeaponType.Mace_1H] * 1.24f - 1);
                    break;
                case Weapon.WeaponType.Mace_2H:
                    break;
                default:
                    break;
            }
        }

        public void ApplySpeedModifier(Weapon weapon, float original, ref float result)
        {
            //result += original * -0.2f;
        }
    }
}
