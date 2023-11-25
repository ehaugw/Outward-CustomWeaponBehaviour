using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public interface IBastardModifier
    {
        void ApplyDamageModifier(Weapon weapon, DamageList original, ref DamageList modifier);

        void ApplyImpactModifier(Weapon weapon, float original, ref float modifier);

        void ApplySpeedModifier(Weapon weapon, float original, ref float result);

        void ApplyStaminaModifier(Weapon weapon, float original, ref float result);
    }
}
