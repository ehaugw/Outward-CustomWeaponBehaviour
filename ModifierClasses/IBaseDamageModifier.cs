using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWeaponBehaviour
{
    public interface IBaseDamageModifier
    {
        bool Eligible(Weapon weapon);
        void Apply(Weapon weapon, DamageList original, ref DamageList result);
    }
}
