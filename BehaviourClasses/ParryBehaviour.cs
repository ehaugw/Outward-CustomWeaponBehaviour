using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;
using UnityEngine;

namespace CustomWeaponBehaviour
{
    public class ParryBehaviour
    {
        public virtual float GetParryWindow(Weapon weapon)
        {
            return 0.30f;
        }
        public virtual float GetParryWindUp(Weapon weapon)
        {
            return 0.10f;
        }

        public bool IsParryMode(Weapon weapon)
        {
            return weapon.IsEquipped && Eligible(weapon) && (weapon.OwnerCharacter?.LeftHandEquipment == null || OffhandEligible(weapon?.OwnerCharacter));
        }

        public virtual bool OffhandEligible(Character character)
        {
            return true;
        }

        public virtual bool Eligible(Weapon weapon)
        {
            return true;
        }

        public virtual bool DidSuccessfulParry(Weapon weapon)
        {
            return IsParryMode(weapon)
                && weapon.OwnerCharacter is Character character
                && character.Blocking && Time.time - ((float)SideLoader.At.GetField<Character>(character, "m_blockTime")) < this.GetParryWindow(weapon)
                && character.Blocking && Time.time - ((float)SideLoader.At.GetField<Character>(character, "m_blockTime")) > this.GetParryWindUp(weapon);
        }

        public virtual void DeliverParry(Character blocker, Character striker, Vector3 _hitDir, float impactDamage)
        {
            SideLoader.At.Invoke<Character>(striker, "StabilityHit", new object[] { impactDamage / 2, Vector3.Angle(striker.transform.forward, -_hitDir), false, blocker });
            CasualStagger.Stagger(striker);
        }
    }
}
