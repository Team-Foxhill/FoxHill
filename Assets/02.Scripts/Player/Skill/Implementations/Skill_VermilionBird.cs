using FoxHill.Core.Damage;
using System.Collections;
using UnityEngine;

namespace FoxHill.Player.Skill.Implementations
{
    /// <summary>
    /// 일정 시간동안 특정 방향으로 날아가며 적에게 데미지를 주는 투사체형 공격 스킬
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class Skill_VermilionBird : SkillBase
    {
        private SkillParameter _parameter;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Cast(SkillParameter parameters)
        {
            _parameter = parameters;

            // 스킬 방향에 따라 이펙트의 이미지 회전값 변환
            Vector3 rotationValue = Vector3.forward * Mathf.Atan2(parameters.Direction.y, parameters.Direction.x) * Mathf.Rad2Deg;
            transform.GetChild(0).localRotation = Quaternion.Euler(rotationValue);

            StartCoroutine(C_Cast(parameters.Direction));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent<IDamageable>(out var damageable) == true)
            {
                if(((1 << damageable.Transform.gameObject.layer) & _attackableLayer) != 0)
                {
                    damageable.OnDead += OnKillMonster;

                    damageable?.TakeDamage(this, Stat.Power);

                    if (damageable != null)
                        damageable.OnDead -= OnKillMonster;
                }
            }
        }

        private IEnumerator C_Cast(Vector2 direction)
        {
            float elapsedTime = 0f;

            while (elapsedTime < Stat.Duration)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                gameObject.transform.Translate(direction * Stat.Speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        private void OnKillMonster()
        {
            if(_parameter.Transform.TryGetComponent(out PlayerManager manager) == true)
            {
                manager.OnKillMonster.Invoke();
            }
        }
    }
}