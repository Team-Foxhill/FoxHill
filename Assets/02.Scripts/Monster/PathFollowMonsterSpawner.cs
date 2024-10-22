using FoxHill.Core.Pause;
using FoxHill.Audio;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FoxHill.Core
{
    public class PathFollowMonsterSpawner : MonoBehaviour, IPausable, IVolumeAdjustable
    {
        public bool IsInitialized { get; private set; }

        public int Stage { get; private set; }
        [SerializeField] private FoxHill.Core.Test.ObjectPoolManager _testObjectPoolManager;
        //[SerializeField] private FoxHill.Tester.ObjectPoolManager _objectPoolManager; // 현재 사용하지 않는 클래스.
        [SerializeField] private int _getCount;
        [SerializeField] private GameObject _spawnPosition;
        [SerializeField] private int _range;
        [SerializeField] private int[] _spawnCountOfStages;
        [SerializeField] private float[] _roundTimes;
        [SerializeField] private float[] _spawnEndTimeOfStages;
        [SerializeField] private GameObject[] _bossMonsters;
        [SerializeField] private int[] _bossActiveStage;
        [SerializeField] private bool[] _isAlreadySpawned;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _bgms;

        private WaitForSecondsRealtime _spawnInterval;
        private int _randomValue;
        private Vector2 _newPosition;
        private bool _isPaused;
        private float _elapsedTime;

        private void Awake()
        {
            _audioSource.clip = _bgms[0];
            _audioSource.loop = true;
            _audioSource.Play();
        }

        private void OnDestroy()
        {
            PauseManager.Unregister(this);
        }

        public IEnumerator ProgressStage(int[] getCounts, float[] roundTimes, float[] spawnEndTimes)
        {
            Stage = 0;
            int i = 0;
            if (_isPaused == true)
            {
                yield return new WaitUntil(() => _isPaused == false);
            }
            foreach (float roundTime in roundTimes)
            {
                if (i <= _bossMonsters.Length)
                {
                    i = CheckBossSpawn(i);
                }

                float stageStartTime = Time.time;
                float spawnEndTimeOfStage = spawnEndTimes[Stage];
                _elapsedTime = 0f;
                StartCoroutine(Spawn(getCounts[Stage], spawnEndTimeOfStage));
                while (_elapsedTime < roundTime)
                {
                    if (_isPaused == true)
                    {
                        float pauseStartedTime = Time.time;
                        yield return new WaitUntil(() => _isPaused == false);
                        float pauseDuration = Time.time - pauseStartedTime;
                        stageStartTime += pauseDuration;
                    }
                    _elapsedTime = Time.time - stageStartTime;
                    //DebugFox.Log($"now Stage is {Stage}, elapsed time is {_elapsedTime}.");
                    yield return null;
                }
                Stage++;
            }
            yield break;
        }

        private int CheckBossSpawn(int i)
        {
            if (_bossActiveStage[i] > Stage || _isAlreadySpawned[i] == true || i >= _bossMonsters.Length)
            {
                return i;
            }
            _bossMonsters[i].SetActive(true);
            //_audioSoure.PlayOneShot(_bgms[i]);
            _isAlreadySpawned[i] = true;
                return i++;

            //if (_bossActiveStage[i] <= Stage && _isAlreadySpawned[i] == false)
            //{
            //    _bossMonsters[i].SetActive(true);
            //    _isAlreadySpawned[i] = true;
            //    if (i < _bossMonsters.Length)
            //    {
            //        return i++;
            //    }
            //    else
            //    {
            //        return i;
            //    }
            //}
            //return i;
        }

            public IEnumerator Spawn(int getCount, float spawnEndTime)
        {
            _spawnInterval = new WaitForSecondsRealtime(1 / spawnEndTime);

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
                yield return _spawnInterval;
            }
            _spawnInterval.Reset();

            yield break;
        }


        //private void Awake()
        //{
        //    PauseManager.Register(this);
        //    StartCoroutine(ProgressStage(_spawnCountOfStages, _roundTimes, _spawnEndTimeOfStages));
        //}

        public void Initialize()
        {
            PauseManager.Register(this);
            IsInitialized = true;
            StartCoroutine(ProgressStage(_spawnCountOfStages, _roundTimes, _spawnEndTimeOfStages));
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

        public void OnVolumeChanged(float volume)
        {
            _audioSource.volume = volume / 2;
        }
    }
}
