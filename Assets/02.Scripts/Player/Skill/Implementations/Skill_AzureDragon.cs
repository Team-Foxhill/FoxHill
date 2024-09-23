using System.Collections;
using UnityEngine;

namespace FoxHill.Player.Skill.Implementations
{
    public class Skill_AzureDragon : SkillBase
    {
        private Coroutine _skillCoroutine = null;

        /// <summary>
        /// 플레이어 주위로 원형의 장애물을 만들어 적들의 움직임을 봉쇄하는 설치기 스킬
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast(SkillParameter parameters)
        {
            _skillCoroutine = StartCoroutine(C_Cast());
        }

        private IEnumerator C_Cast()
        {
            float elapsedTime = 0f;

            while (elapsedTime < 10f)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                yield return null;
                elapsedTime += Time.deltaTime;

            }

            Destroy(gameObject);
        }
    }
}