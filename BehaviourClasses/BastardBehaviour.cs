using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyHelper;

namespace CustomWeaponBehaviour
{
    public class BastardBehaviour
    {
        public static Weapon.WeaponType? GetBastardType(Weapon.WeaponType type)
        {
            switch (type)
            {
                case Weapon.WeaponType.Axe_1H:
                    return Weapon.WeaponType.Axe_2H;
                case Weapon.WeaponType.Sword_1H:
                    return Weapon.WeaponType.Sword_2H;
                case Weapon.WeaponType.Mace_1H:
                    return Weapon.WeaponType.Mace_2H;
                default:
                    return type;
            }
        }

        public virtual bool Eligible(Weapon weapon)
        {
            return weapon.HasTag(CustomWeaponBehaviour.BastardTag) && !weapon.TwoHanded;
        }

        public bool IsBastardMode(Weapon weapon)
        {
            return Eligible(weapon) && weapon.IsEquipped
                && (
                    weapon.OwnerCharacter?.LeftHandEquipment == null || 
                    (weapon.OwnerCharacter?.LeftHandEquipment is Equipment leftHand && leftHand.HasTag(CustomWeaponBehaviour.HandsFreeTag)/* && leftHand.IKType == Equipment.IKMode.None*/)
                ) && !CustomWeaponBehaviour.Instance.maulShoveBehaviour.IsActive(weapon);
        }

        public void PostAffectDamage(ref Weapon weapon, DamageList original, ref DamageList result)
        {
            if (this.IsBastardMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplyDamageModifier(weapon, original, ref result);
                }
            }
        }

        public void PostAffectImpact(ref Weapon weapon, float original, ref float result)
        {
            if (this.IsBastardMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplyImpactModifier(weapon, original, ref result);
                }
            }
        }

        public void PostAffectSpeed(ref Weapon weapon, float original, ref float result)
        {
            if (this.IsBastardMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplySpeedModifier(weapon, original, ref result);
                }
            }
        }

        public void PostAffectStaminaCost(ref Weapon weapon, float original, ref float result)
        {
            if (this.IsBastardMode(weapon))
            {
                foreach (var modifier in CustomWeaponBehaviour.IBastardModifiers)
                {
                    modifier.ApplyStaminaModifier(weapon, original, ref result);
                }
            }
        }
    }
}
