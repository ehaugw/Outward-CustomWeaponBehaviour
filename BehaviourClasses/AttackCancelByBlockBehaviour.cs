using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class AttackCancelByBlockBehaviour
    {
        public bool IsBlockMode(Character character)
        {
            return Eligible(character);
        }

        public virtual bool Eligible(Character character)
        {
            return false;
        }
    }
}
