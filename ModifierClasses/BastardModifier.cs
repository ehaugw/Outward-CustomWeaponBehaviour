using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class BastardModifier : IBastardModifier
    {
        public void ApplyDamageModifier(Weapon weapon, DamageList original, ref DamageList result)
        {
            result += original * 0.1f;
        }

        public void ApplyImpactModifier(Weapon weapon, float original, ref float result)
        {
            result += original * 0.1f;
        }

        public void ApplySpeedModifier(Weapon weapon, float original, ref float result)
        {
            result += 0.1f;
        }
        
        public void ApplyStaminaModifier(Weapon weapon, float original, ref float stamina)
        {
            stamina += original * -0.4f;
        }
    }
}
