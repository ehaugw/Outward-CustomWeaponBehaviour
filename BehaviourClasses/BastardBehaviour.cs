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

        public virtual float GetBastardSpeedBonus(Weapon weapon)
        {
            float modifier = 0;
            foreach (var modObj in CustomWeaponBehaviour.BastardModifiers)
            {
                modObj.ApplySpeedModifier(weapon, ref modifier);
            }
            return modifier;
        }

        public virtual float GetBastardDamageBonus(Weapon weapon)
        {
            float modifier = 1;
            foreach (var modObj in CustomWeaponBehaviour.BastardModifiers)
            {
                modObj.ApplyDamageModifier(weapon, ref modifier);
            }
            return modifier;
        }

        public virtual bool Eligible(Weapon weapon)
        {
            return weapon.HasTag(CustomWeaponBehaviour.BastardTag);
        }

        public bool IsBastardMode(Weapon weapon)
        {
            return Eligible(weapon) && weapon.IsEquipped
                && (
                    weapon.OwnerCharacter?.LeftHandEquipment == null || 
                    (weapon.OwnerCharacter?.LeftHandEquipment is Equipment leftHand && leftHand.HasTag(TinyTagManager.GetOrMakeTag("TinyTrinketTag"))/* && leftHand.IKType == Equipment.IKMode.None*/)
                )
                && !weapon.TwoHanded;
        }

        public void BastardPostAmplifyWeaponDamage(ref Weapon weapon, ref DamageList _damageList)
        {
            if (this.IsBastardMode(weapon))
            {
                _damageList *= this.GetBastardDamageBonus(weapon);
            }
        }

        public void BastardPostAmplifySpeed(ref Weapon weapon, ref float result)
        {
            if (this.IsBastardMode(weapon))
            {
                result += this.GetBastardSpeedBonus(weapon);
            }
        }
    }
}
