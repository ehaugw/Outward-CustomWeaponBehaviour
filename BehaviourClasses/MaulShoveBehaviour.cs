using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;

namespace CustomWeaponBehaviour
{
    public class MaulShoveBehaviour
    {
        public virtual float GetMaulShoveSpeedBonus(Weapon weapon)
        {
            float modifier = 0;
            foreach (var modObj in CustomWeaponBehaviour.IMaulShoveModifiers)
            {
                modObj.ApplySpeedModifier(weapon, ref modifier);
            }
            return modifier;
        }

        public virtual float GetMaulShoveDamageBonus(Weapon weapon)
        {
            float modifier = 1;
            foreach (var modObj in CustomWeaponBehaviour.IMaulShoveModifiers)
            {
                modObj.ApplyDamageModifier(weapon, ref modifier);
            }
            return modifier;
        }

        public virtual float GetMaulShoveImpactBonus(Weapon weapon)
        {
            float modifier = 1;
            foreach (var modObj in CustomWeaponBehaviour.IMaulShoveModifiers)
            {
                modObj.ApplyImpactModifier(weapon, ref modifier);
            }
            return modifier;
        }

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

        public void MaulShovePostAmplifyWeaponDamage(ref Weapon weapon, ref DamageList _damageList)
        {
            if (this.IsMaulShoveMode(weapon) && this.IsActive(weapon))
            {
                _damageList *= this.GetMaulShoveDamageBonus(weapon);
            }
        }

        public void MaulShovePostAmplifyWeaponImpact(ref Weapon weapon, ref float _impactDamage)
        {
            if (this.IsMaulShoveMode(weapon) && this.IsActive(weapon))
            {
                _impactDamage *= this.GetMaulShoveImpactBonus(weapon);
            }
        }

        public void MaulShovePostAmplifySpeed(ref Weapon weapon, ref float result)
        {
            if (this.IsMaulShoveMode(weapon) && this.IsActive(weapon))
            {
                result += this.GetMaulShoveSpeedBonus(weapon);
            }
        }
    }
}
