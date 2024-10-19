using FoxHill.Core;
using FoxHill.Core.Utils;
using FoxHill.Monster;
using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace FoxHill.Tower
{
    public class DefaultDefenseTower : DefenseTowerControllerBase
    {
        [SerializeField] private float getDamageInterval = 1f;
        private CrowdObstacleAuthoring _crowdObstacle;
        private Vector2[] transforms;
        private HashSet<Collision2D> _monsterSet = new HashSet<Collision2D>(256);
        private HashSet<Transform> _monsterTransformSet = new HashSet<Transform>(256);
        private Coroutine _periodlyGetDamageCoroutine;
        private float _intervalStartTime;
        private float _elapsedTime;

        // todo.Villin 타워 어떻게 해야 몬스터가 타워에 비비게 할 수 있을지.
        protected override void Awake()
        {
            base.Awake();
            _crowdObstacle = GetComponent<CrowdObstacleAuthoring>();
            _crowdObstacle.enabled = false;
        }

        protected override IEnumerator PerformTowerFunction()
        {
            yield return null;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER && !_monsterSet.Contains(collision))
            {
                _monsterSet.Add(collision);
                float power = collision.gameObject.GetComponent<PathFollowMonsterController>().Power;
                if (_crowdObstacle.enabled == false)
                {
                    _crowdObstacle.enabled = true;
                }

                if (_periodlyGetDamageCoroutine == null)
                {
                    _periodlyGetDamageCoroutine = StartCoroutine(PeriodlyGetDamage(power, _monsterTransformSet));
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER && !_monsterSet.Contains(collision))
            {
                _monsterSet.Remove(collision);
                if (_monsterSet.Count == 0 && _periodlyGetDamageCoroutine != null)
                {
                    if (_crowdObstacle.enabled == false)
                    {
                        _crowdObstacle.enabled = true;
                    }
                    StopCoroutine(_periodlyGetDamageCoroutine);
                    _periodlyGetDamageCoroutine = null;
                }
            }
        }

        private IEnumerator PeriodlyGetDamage(float damage, HashSet<Transform> transformSet)
        {
            Vector2 position = transform.position;
            DebugFox.Log("PeriodlyGetDamage Coroutine Started");
            if (_intervalStartTime == default)
            { _intervalStartTime = Time.time; }
            foreach (var transform in transformSet)
            {
                int i = 0;
                transforms[i] = transform.transform.position;
                i++;
            }
            while (_monsterSet.Count > 0)
            {
                foreach (var transform in transformSet)
                {
                    int i = 0;
                    transform.position = transforms[i];
                        i++;
                }
                if (_isPaused == true)
                {
                    float pauseStartedTime = Time.time;
                    yield return new WaitUntil(() => _isPaused == false);
                    float pauseDuration = Time.time - pauseStartedTime;
                    _intervalStartTime += pauseDuration;
                }
                _elapsedTime = Time.time - _intervalStartTime;
                if (_elapsedTime > getDamageInterval)
                {
                    float totalDamage = damage * _monsterSet.Count;
                    _stat.CurrentHp -= totalDamage;
                    if (_stat.CurrentHp <= 0f)
                    {
                        Destroy(gameObject);
                    }
                    _elapsedTime = 0f;
                    _intervalStartTime = Time.time;
                }
                yield return null;
            }
            DebugFox.Log($"MonsterSet count is {_monsterSet.Count}");
            _periodlyGetDamageCoroutine = null;
        }
    }
}