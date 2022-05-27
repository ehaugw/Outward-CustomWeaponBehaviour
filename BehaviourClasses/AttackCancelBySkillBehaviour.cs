namespace CustomWeaponBehaviour
{
    public class AttackCancelBySkillBehaviour
    {
        public bool IsAttackCancelBySkillMode(Character character)
        {
            return Eligible(character);
        }

        public virtual bool Eligible(Character character)
        {
            return false;
        }
    }
}
