using FoxHill.Core.Damage;
using FoxHill.Core;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace FoxHill.Tower
{
    public class DefaultAttackTowerController : TowerControllerBase
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private SpriteRenderer _bulletRenderer;
        private WaitForSecondsRealtime _waitForSecondsRealtime;

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
                if (objectsInTrigger.Count == 0)
                {
                    _bulletRenderer.enabled = false;
                    yield return _waitForSecondsRealtime;
                    continue;
                }
                if (_attackTarget == null || !objectsInTrigger.Contains(_attackTarget))
                {
                    _attackTarget = objectsInTrigger.ElementAt(Random.Range(0, objectsInTrigger.Count));
                    _damageable = _attackTarget.gameObject.GetComponent<IDamageable>();
                    _targetPosition = _attackTarget.transform.position;
                }
                if (_damageable != null)
                {
                    yield return StartCoroutine(BulletAnimation());
                    DebugFox.Log("TowerAttackPerformed!");
                    _damageable.TakeDamage(Power);
                }
                yield return null;
            }
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
                // 선형 보간을 사용하여 부드럽게 이동
                Vector2 direction = ((Vector2)_targetPosition - (Vector2)_bullet.transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Z 축 회전만 적용
                _bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // 선형 보간을 사용하여 부드럽게 이동 (z 좌표 유지)
                _bullet.transform.position = Vector3.Lerp(_startPosition, _targetPosition, elapsedTime / _attackInterval); // 타겟 포지션을 여기서 업데이트하기.
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
