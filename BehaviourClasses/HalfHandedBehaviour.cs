using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;
using UnityEngine;

namespace CustomWeaponBehaviour
{
    public class HalfHandedBehaviour
    {
        public bool IsHalfHandedMode(Weapon weapon)
        {
            return weapon.IsEquipped && Eligible(weapon) && weapon.OwnerCharacter?.LeftHandEquipment == null && !weapon.TwoHanded;
        }

        public virtual bool Eligible(Weapon weapon)
        {
            return weapon.HasTag(CustomWeaponBehaviour.HalfHandedTag);
        }
    }
}
