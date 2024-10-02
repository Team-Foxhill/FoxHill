using System.Collections;
using UnityEngine;

namespace FoxHill.Player.Skill.Implementations
{
    public class Skill_BlackTortoise : SkillBase
    {
        /// <summary>
        /// 일정 시간동안 범위 내의 적들에게 지속적인 데미지를 주는 장판/도트 데미지형 공격 스킬
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast(SkillParameter parameters)
        {
            StartCoroutine(C_Cast(parameters.Transform));
        }

        private IEnumerator C_Cast(Transform followTarget)
        {
            float elapsedTime = 0f;

            while (elapsedTime < Stat.Duration)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                elapsedTime += Time.deltaTime;
                var attackRange = Physics2D.OverlapCircleAll(transform.position, 3f);

                // this.transform.position = followTarget.position;

                yield return null;
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }
}