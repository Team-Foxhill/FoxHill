namespace FoxHill.Player.Skill.Implementations
{
    /// <summary>
    /// 플레이어의 전방에 일시적으로 큰 데미지를 주는 근접형 공격 스킬
    /// </summary>
    public class Skill_WhiteTiger : SkillBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast(SkillParameter parameters)
        {
            Destroy(gameObject, 5f);

            gameObject.transform.Translate(parameters.Direction * 3f);
        }
    }
}