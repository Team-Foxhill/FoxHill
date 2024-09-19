using System.Collections;
using UnityEngine;

namespace FoxHill.Player.Skill.Implementations
{
    public class Skill_AzureDragon : SkillBase
    {
        /// <summary>
        /// 플레이어 주위로 원형의 장애물을 만들어 적들의 움직임을 봉쇄하는 설치기 스킬
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast(SkillParameter parameters)
        {
            Destroy(gameObject, 10f);
        }
    }
}