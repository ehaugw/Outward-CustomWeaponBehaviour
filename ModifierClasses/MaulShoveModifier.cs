using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class MaulShoveModifier : IMaulShoveModifier
    {
        public void ApplyDamageModifier(Weapon weapon, DamageList original, ref DamageList result)
        {
            result += original * -0.25f;
        }

        public void ApplyImpactModifier(Weapon weapon, float original, ref float result)
        {
            switch (weapon.Type)
            {
                case Weapon.WeaponType.Sword_1H:
                case Weapon.WeaponType.Sword_2H:
                case Weapon.WeaponType.Axe_1H:
                case Weapon.WeaponType.Axe_2H:
                    result += original * 2f;
                    break;
                case Weapon.WeaponType.Mace_1H:
                case Weapon.WeaponType.Mace_2H:
                    result += original * 0.5f;
                    break;
                default:
                    break;
            }
        }

        public void ApplySpeedModifier(Weapon weapon, float original, ref float result)
        {
            result += original * -0.2f;
        }
    }
}
