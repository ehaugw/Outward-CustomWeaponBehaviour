﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class MaulShoveModifier
    {
        public virtual void ApplyDamageModifier(Weapon weapon, ref float modifier)
        {
            modifier -= 0.25f;
        }

        public virtual void ApplyImpactModifier(Weapon weapon, ref float modifier)
        {
            switch (weapon.Type)
            {
                case Weapon.WeaponType.Sword_1H:
                case Weapon.WeaponType.Sword_2H:
                case Weapon.WeaponType.Axe_1H:
                case Weapon.WeaponType.Axe_2H:
                    modifier += 2f;
                    break;
                default:
                    break;
            }
        }

        public virtual void ApplySpeedModifier(Weapon weapon, ref float modifier)
        {
            modifier -= 0.2f;
        }
    }
}
