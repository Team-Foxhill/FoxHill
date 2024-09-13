namespace FoxHill.Player.Skill
{
    public interface ISkill
    {
        SkillModel Stat { get; }
        void Cast();
    }
}