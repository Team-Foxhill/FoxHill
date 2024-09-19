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
            StartCoroutine(C_Cast());
        }

        // test code
        private IEnumerator C_Cast()
        {
            float elapsedTime = 0f;
            while (elapsedTime < 2.5f)
            {
                elapsedTime += Time.deltaTime;
                var s = Physics2D.OverlapBoxAll(transform.position, new Vector2(4f, 2f), 0f);

                yield return null;
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector2(8f, 3f));
        }
    }
}