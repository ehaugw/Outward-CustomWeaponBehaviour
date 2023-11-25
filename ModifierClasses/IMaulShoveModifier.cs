using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public interface IMaulShoveModifier
    {
        void ApplyDamageModifier(Weapon weapon, DamageList original, ref DamageList result);
        void ApplyImpactModifier(Weapon weapon, float original, ref float result);
        void ApplySpeedModifier(Weapon weapon, float original, ref float result);
    }
}
