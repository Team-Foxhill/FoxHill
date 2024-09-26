using FoxHill.Core.Pause;
using FoxHill.Monster;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace FoxHill.Core
{
    public class PathFollowMonsterSpawner : MonoBehaviour, IPausable
    {
        [SerializeField] private FoxHill.Core.Test.ObjectPoolManager _testObjectPoolManager;
        //[SerializeField] private FoxHill.Tester.ObjectPoolManager _objectPoolManager; // 현재 사용하지 않는 클래스.
        [SerializeField] private int _getCount;
        [SerializeField] private GameObject _spawnPosition;
        [SerializeField] private float _spawnInterval;
        [SerializeField] private float _roundTime;
        [SerializeField] private int _range;
        private WaitForSecondsRealtime _time;
        private int _randomValue;
        private Vector2 _newPosition;
        private bool _isPaused;


        public void GetMonster(float getCount, float interval, float roundTime)
        {
            StartCoroutine(GetMonsterPeriodically(getCount, interval, roundTime));
        }

        public IEnumerator GetMonsterPeriodically(float getCount, float interval, float roundTime)
        {
            _time = new WaitForSecondsRealtime(interval);


            for (int i = 0; getCount > i; i++)
            {
                if (_isPaused == true)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }

                if (_testObjectPoolManager != null)
                {
                    GameObject newOne = _testObjectPoolManager.Get();
                    if (newOne != null)
                    {
                        newOne.transform.position = GetSpawnPosition();
                        newOne.SetActive(true);
                    }

                }
                //else
                //{
                //    GameObject newOne = _objectPoolManager.Get();
                //    if (newOne != null)
                //    {
                //        newOne.transform.position = GetSpawnPosition();
                //        newOne.SetActive(true);
                //    }
                //}
                yield return _time;
            }
            _time.Reset();

            yield break;
        }


        private void Start()
        {
            PauseManager.Register(this);
            StartCoroutine(GetMonsterPeriodically(_getCount, _spawnInterval, _roundTime));
        }

        //private void GetFromPool(int getCount)
        //{
        //    for (int i = 0; i < getCount; i++)
        //    {
        //        GameObject newOne = _objectPoolManager.Get();
        //        if (newOne != null)
        //        {
        //            newOne.transform.position = GetSpawnPosition();
        //            newOne.SetActive(true);
        //        }
        //    }
        //}

        private Vector2 GetSpawnPosition()
        {
            return new Vector2(_spawnPosition.transform.position.x + Random.Range(-_range, _range), _spawnPosition.transform.position.y + Random.Range(-_range, _range));
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
