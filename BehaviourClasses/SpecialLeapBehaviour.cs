using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;
using UnityEngine;

namespace CustomWeaponBehaviour
{
    public class SpecialLeapBehaviour
    {
        public virtual bool IsSpecialLeapMode(Weapon weapon)
        {
            return (weapon.IsEquipped && weapon.HasTag(CustomWeaponBehaviour.SpecialLeapTag));
        }
    }
}
