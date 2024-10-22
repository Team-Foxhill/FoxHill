using FoxHill.Core;
using FoxHill.Core.Pause;
using FoxHill.Core.Test;
using FoxHill.Core.Utils;
using FoxHill.Monster;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Map
{
    public class DestinationHPManager : MonoBehaviour, IPausable
    {
        public event Action OnDead;
        public float CurrentHP { get; private set; }
        [SerializeField] private float destinationMaxHP = 1000;
        [SerializeField] private float getDamageInterval;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _spriteRenderer2;
        [SerializeField] private DestinationHPUI _hpUI;
        private HashSet<Collider2D> _monsterSet = new HashSet<Collider2D>(512);
        private bool _isPaused;
        private float _elapsedTime;
        private Coroutine _periodlyGetDamageCoroutine;
        private float _intervalStartTime;
        private Color _initialColor;
        private readonly Color COLOR_DAMAGED = new Color(255f / 255f, 47f / 255f, 47f / 255f);
        private readonly WaitForSeconds _colorChangeWait = new WaitForSeconds(0.2f);


        private void Start()
        {
            CurrentHP = destinationMaxHP;
            ObjectPoolManager.RegisterColliderCheckneededObject(this);
            _initialColor = _spriteRenderer.color;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER && !_monsterSet.Contains(collision))
            {
                _monsterSet.Add(collision);
                float power = 0f;
                if (collision.gameObject.GetComponent<PathFollowMonsterController>() != null)
                {
                    power = collision.gameObject.GetComponent<PathFollowMonsterController>().Power;
                }

                if (_periodlyGetDamageCoroutine == null)
                {
                    _periodlyGetDamageCoroutine = StartCoroutine(PeriodlyGetDamage(power));
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER && _monsterSet.Contains(collision))
            {
                _monsterSet.Remove(collision);
                if (_monsterSet.Count == 0 && _periodlyGetDamageCoroutine != null)
                {
                    StopCoroutine(_periodlyGetDamageCoroutine);
                    _periodlyGetDamageCoroutine = null;
                }
            }
        }

        private IEnumerator PeriodlyGetDamage(float damage)
        {
            DebugFox.Log("PeriodlyGetDamage Coroutine Started");
            if (_intervalStartTime == default)
            { _intervalStartTime = Time.time; }
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
                    CurrentHP -= totalDamage;
                    if (totalDamage != 0)
                    {
                    _hpUI.HPBar(CurrentHP/ destinationMaxHP);
                    }
                    DebugFox.Log($"currentHP = {CurrentHP}, Monsters: {_monsterSet.Count}, Damage: {totalDamage}");
                    if (CurrentHP <= 0f)
                    {
                        OnDead.Invoke();
                        yield break;
                    }
                    _elapsedTime = 0f;
                    _intervalStartTime = Time.time;
                }
                StartCoroutine(C_ChangeColor());
                yield return null;
            }
            DebugFox.Log($"MonsterSet count is {_monsterSet.Count}");
            _periodlyGetDamageCoroutine = null;
        }

        private IEnumerator C_ChangeColor()
        {
            _spriteRenderer.color = COLOR_DAMAGED;
            _spriteRenderer2.color = COLOR_DAMAGED;
            yield return _colorChangeWait;
            _spriteRenderer.color = _initialColor;
            _spriteRenderer2.color = _initialColor;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }


    }
}