using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class BastardModifier
    {
        public virtual void ApplyDamageModifier(Weapon weapon, ref float modifier)
        {
            modifier += 0.2f;
        }

        public virtual void ApplyImpactModifier(Weapon weapon, ref float modifier)
        {
           modifier += 0.2f;
        }

        public virtual void ApplySpeedModifier(Weapon weapon, ref float modifier)
        {
            modifier += 0.0f;
        }
    }
}
