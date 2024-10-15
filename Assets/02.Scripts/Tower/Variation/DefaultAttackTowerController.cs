using FoxHill.Core;
using FoxHill.Core.Damage;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace FoxHill.Tower
{
    public class DefaultAttackTowerController : TowerControllerBase
    {
        private Vector3 _startPosition;
        private SpriteRenderer _bulletRenderer;
        private WaitForSecondsRealtime _waitForSecondsRealtime;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            _startPosition = transform.position;
            _bulletRenderer = _bullet.GetComponent<SpriteRenderer>();
            _waitForSecondsRealtime = new WaitForSecondsRealtime(_attackInterval);

            base.Start();
        }

        protected override IEnumerator PerformTowerFunction()
        {
            while (true)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                if (_objectsInTrigger.Count == 0) 
                {
                    _bulletRenderer.enabled = false;
                    yield return _waitForSecondsRealtime;
                    continue;
                }

                if (_attackTarget == null || _objectsInTrigger.Contains(_attackTarget) == false)
                {
                    _attackTarget = _objectsInTrigger.ElementAt(Random.Range(0, _objectsInTrigger.Count));
                    _attackTarget.OnDead += ResetTarget;
                }

                if (_attackTarget != null)
                {
                    yield return StartCoroutine(BulletAnimation());

                    DebugFox.Log("TowerAttackPerformed!");
                    _attackTarget.TakeDamage(this, _stat.Power);
                }
                yield return null;
            }
        }

        private void ResetTarget()
        {
            _attackTarget.OnDead -= ResetTarget;
            _attackTarget = null;
        }

        /// <summary>
        /// 현재 정확한 위치에 도달하지 못함.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BulletAnimation()
        {
            _bullet.transform.position = _startPosition;
            _bulletRenderer.enabled = true;

            float elapsedTime = 0f;
            while (elapsedTime < _attackInterval)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                Vector2 direction = ((Vector2)_attackTarget.Transform.position - (Vector2)_bullet.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Z 축 회전만 적용
                _bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // 선형 보간을 사용하여 부드럽게 이동 (z 좌표 유지)
                _bullet.transform.position = Vector3.Lerp(_startPosition, _attackTarget.Transform.position, elapsedTime / _attackInterval); // 타겟 포지션을 여기서 업데이트하기.
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
