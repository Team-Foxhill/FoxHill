using System.Collections;
using UnityEngine;

namespace FoxHill.Player.Skill.Implementations
{
    /// <summary>
    /// 일정 시간동안 특정 방향으로 날아가며 적에게 데미지를 주는 투사체형 공격 스킬
    /// </summary>
    public class Skill_VermilionBird : SkillBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast(SkillParameter parameters)
        {
            StartCoroutine(C_TESTMOVE(parameters.Direction));
        }

        // test code
        private IEnumerator C_TESTMOVE(Vector2 direction)
        {
            float elapsedTime = 0f;
            while (elapsedTime < 1.5f)
            {
                gameObject.transform.Translate(direction * 2f * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}