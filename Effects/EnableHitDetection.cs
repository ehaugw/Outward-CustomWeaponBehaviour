namespace CustomWeaponBehaviour
{
    public class EnableHitDetection : Effect
    {
        protected override void ActivateLocally(Character _affectedCharacter, object[] _infos)
        {
            if (ParentItem is MeleeSkill meleeSkill && meleeSkill.MeleeHitDetector != null && _affectedCharacter?.SkillMeleeDetector is MeleeHitDetector meleeHitDetector)
            {
                meleeHitDetector.HitStarted(-1);
            }
            else if (_affectedCharacter?.CurrentWeapon is MeleeWeapon meleeWeapon)
            {
                meleeWeapon.HitStarted(-1);
            }
        }
    }
}
