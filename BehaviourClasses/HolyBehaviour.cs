using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomWeaponBehaviour
{
    public class HolyBehaviour
    {
        public virtual DamageType.Types GetHolyDamageType()
        {
            return HolyDamageManager.HolyDamageManager.GetDamageType();
        }

        public virtual float GetHolyWeaponConvertionRate(Weapon weapon)
        {
            return 0.20f;
        }

        public bool IsHolyWeaponMode(Weapon weapon)
        {
            if (weapon.IsEquipped && Eligible(weapon) && weapon.Imbued)
            {
                foreach (Effect effect in weapon.FirstImbue.ImbuedEffects)
                {
                    if (effect is WeaponDamage && ((WeaponDamage)effect).Damages[0].Type == this.GetHolyDamageType())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool Eligible(Weapon weapon)
        {
            return weapon.HasTag(CustomWeaponBehaviour.HolyWeaponTag);
        }

        public void HolyPostAmplifyWeaponDamage(ref Weapon _weapon, ref DamageList _damageList)
        {
            if (this.IsHolyWeaponMode(_weapon))
            {
                float total = _damageList.TotalDamage;
                float conversionRate = this.GetHolyWeaponConvertionRate(_weapon);
                _damageList *= (1 - conversionRate);
                _damageList.Add(new DamageType(this.GetHolyDamageType(), total * conversionRate));
            }
        }
    }
}
