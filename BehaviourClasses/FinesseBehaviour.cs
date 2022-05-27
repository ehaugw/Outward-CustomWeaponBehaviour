using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class FinesseBehaviour
    {
        public bool IsFinesseMode(Weapon weapon)
        {
            return weapon.IsEquipped && Eligible(weapon);
        }

        public virtual bool Eligible(Weapon weapon)
        {
            return weapon.HasTag(CustomWeaponBehaviour.FinesseTag);
        }

    }
}
