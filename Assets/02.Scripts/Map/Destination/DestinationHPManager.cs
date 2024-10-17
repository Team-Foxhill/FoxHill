using FoxHill.Core;
using FoxHill.Core.Pause;
using FoxHill.Core.Test;
using FoxHill.Core.Utils;
using FoxHill.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Map
{
    public class DestinationHPManager : MonoBehaviour, IPausable
    {
        public float CurrentHP { get; private set; }
        [SerializeField] private float destinationMaxHP = 1000;
        [SerializeField] private float getDamageInterval;
        private HashSet<Collider2D> _monsterSet = new HashSet<Collider2D>(512);
        private bool _isPaused;
        private float _elapsedTime;
        private Coroutine _periodlyGetDamageCoroutine;
        private float _intervalStartTime;



        private void Start()
        {
            CurrentHP = destinationMaxHP;
            ObjectPoolManager.RegisterColliderCheckneededObject(this);
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
                    DebugFox.Log($"currentHP = {CurrentHP}, Monsters: {_monsterSet.Count}, Damage: {totalDamage}");
                    if (CurrentHP <= 0f)
                    {
                        // todo.Villin 게임 오버 화면과 이어주기.
                    }
                    _elapsedTime = 0f;
                    _intervalStartTime = Time.time;
                }
                yield return null;
            }
            DebugFox.Log($"MonsterSet count is {_monsterSet.Count}");
            _periodlyGetDamageCoroutine = null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER && !_monsterSet.Contains(collision))
            {
                _monsterSet.Add(collision);
                float power = collision.gameObject.GetComponent<PathFollowMonsterController>().Power;
                if (_periodlyGetDamageCoroutine == null)
                {
                    _periodlyGetDamageCoroutine = StartCoroutine(PeriodlyGetDamage(power));
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER && _monsterSet.Contains(collision) )
            {
                _monsterSet.Remove(collision);
                if (_monsterSet.Count == 0 && _periodlyGetDamageCoroutine != null)
                {
                    StopCoroutine(_periodlyGetDamageCoroutine);
                    _periodlyGetDamageCoroutine = null;
                }
            }
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