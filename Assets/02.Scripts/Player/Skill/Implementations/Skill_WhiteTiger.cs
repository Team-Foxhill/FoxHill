using FoxHill.Core.Damage;
using FoxHill.Core.Effect;
using System.Collections;
using UnityEngine;

namespace FoxHill.Player.Skill.Implementations
{
    /// <summary>
    /// 플레이어의 전방에 일시적으로 큰 데미지를 주는 근접형 공격 스킬
    /// </summary>
    public class Skill_WhiteTiger : SkillBase
    {
        private const float SKILL_RANGE = 3f;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast(SkillParameter parameters)
        {
            gameObject.transform.Translate(parameters.Direction * SKILL_RANGE);
            EffectManager.Play(EffectManager.FeedbackType.Impulse);

            var hits = Physics2D.OverlapCircleAll(transform.position, SKILL_RANGE, _attackableLayer);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable) == true)
                {
                    damageable.TakeDamage(this, Stat.Power);
                }
            }
            StartCoroutine(C_Cast());
        }

        private IEnumerator C_Cast()
        {
            float elapsedTime = 0f;

            while (elapsedTime < Stat.Duration)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}