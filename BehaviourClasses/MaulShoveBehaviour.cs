using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;

namespace CustomWeaponBehaviour
{
    public class MaulShoveBehaviour
    {
        public virtual bool Eligible(Weapon weapon)
        {
            return weapon.HasTag(CustomWeaponBehaviour.MaulShoveTag);
        }

        public virtual bool IsActive(Weapon weapon)
        {
            //return true;
            return weapon.Type != Weapon.WeaponType.Mace_2H && BehaviourManager.GetCurrentAnimationType(weapon) == Weapon.WeaponType.Mace_2H;
        }

        public bool IsMaulShoveMode(Weapon weapon)
        {
            return Eligible(weapon) && weapon.IsEquipped
                && (
                    weapon.OwnerCharacter?.LeftHandEquipment == null || 
                    (weapon.OwnerCharacter?.LeftHandEquipment is Equipment leftHand && leftHand.HasTag(CustomWeaponBehaviour.HandsFreeTag)/* && leftHand.IKType == Equipment.IKMode.None*/) ||
                    weapon.TwoHanded
                );
        }

        public void PostAffectDamage(ref Weapon weapon, DamageList original, ref DamageList result)
        {
            if (this.IsMaulShoveMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplyDamageModifier(weapon, original, ref result);
                }
            }
        }

        public void PostAffectImpact(ref Weapon weapon, float original, ref float result)
        {
            if (this.IsMaulShoveMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplyImpactModifier(weapon, original, ref result);
                }
            }
        }

        public void PostAffectSpeed(ref Weapon weapon, float original, ref float result)
        {
            if (this.IsMaulShoveMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplySpeedModifier(weapon, original, ref result);
                }
            }
        }

        public void PostAffectStaminaCost(ref Weapon weapon, float original, ref float result)
        {
            if (this.IsMaulShoveMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplyStaminaModifier(weapon, original, ref result);
                }
            }
        }
    }
}
