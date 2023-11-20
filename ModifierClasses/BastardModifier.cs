using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class BastardModifier : IBastardModifier
    {
        public void ApplyDamageModifier(Weapon weapon, ref float modifier)
        {
            modifier += 0.2f;
        }

        public void ApplyImpactModifier(Weapon weapon, ref float modifier)
        {
           modifier += 0.2f;
        }

        public void ApplySpeedModifier(Weapon weapon, ref float modifier)
        {
            modifier += 0.0f;
        }
    }
}
