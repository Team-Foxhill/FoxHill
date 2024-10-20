using FoxHill.Core;
using FoxHill.Core.Pause;
using FoxHill.Core.Utils;
using FoxHill.Monster;
using ProjectDawn.Navigation.Hybrid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Tower
{
    public class DefaultDefenseTower : DefenseTowerControllerBase
    {
        [SerializeField] private float getDamageInterval = 1f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private HashSet<Collider2D> _monsterSet = new HashSet<Collider2D>(256);
        private HashSet<Transform> _monsterTransformSet = new HashSet<Transform>(256);
        private Coroutine _periodlyGetDamageCoroutine;
        private float _intervalStartTime;
        private float _elapsedTime;
        private Color _initialColor;
        private readonly Color COLOR_DAMAGED = new Color(255f / 255f, 47f / 255f, 47f / 255f);
        private readonly WaitForSeconds _colorChangeWait = new WaitForSeconds(0.2f);


        protected override void Awake()
        {
            base.Awake();
            PauseManager.Register(this);
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override IEnumerator PerformTowerFunction()
        {
            yield return null;
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER && !_monsterSet.Contains(collider))
            {
                _monsterSet.Add(collider);
                float power = collider.gameObject.GetComponent<PathFollowMonsterController>().Power;
                AgentAuthoring agentAuthoring = collider.gameObject.GetComponent<AgentAuthoring>();
                if (agentAuthoring.enabled == true)
                {
                    agentAuthoring.enabled = false;
                }

                if (_periodlyGetDamageCoroutine == null)
                {
                    _periodlyGetDamageCoroutine = StartCoroutine(PeriodlyGetDamage(power, _monsterSet));
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER)
            {
                AgentAuthoring agentAuthoring = collider.gameObject.GetComponent<AgentAuthoring>();
                if (agentAuthoring.enabled == false)
                {
                    agentAuthoring.enabled = true;
                }
                _monsterSet.Remove(collider);
                if (_monsterSet.Count == 0 && _periodlyGetDamageCoroutine != null)
                {
                    StopCoroutine(_periodlyGetDamageCoroutine);
                    _periodlyGetDamageCoroutine = null;
                }
            }
        }

        private IEnumerator PeriodlyGetDamage(float damage, HashSet<Collider2D> monsterSet)
        {
            Vector2 position = transform.position;
            DebugFox.Log("PeriodlyGetDamage Coroutine Started");
            if (_intervalStartTime == default)
            { _intervalStartTime = Time.time; }
            foreach (var monster in monsterSet)
            {
                int i = 0;
                i++;
            }
            while (_monsterSet.Count > 0)
            {
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
                    StartCoroutine(C_ChangeColor());

                    if (_stat.CurrentHp <= 0f)
                    {
                        foreach (var monster in _monsterSet)
                        {
                            AgentAuthoring agentAuthoring = monster.gameObject.GetComponent<AgentAuthoring>();

                            if (agentAuthoring.enabled == false)
                            {
                                agentAuthoring.enabled = true;
                            }
                        }
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

        private IEnumerator C_ChangeColor()
        {
            _spriteRenderer.color = COLOR_DAMAGED;
            yield return _colorChangeWait;
            _spriteRenderer.color = _initialColor;
        }
    }
}