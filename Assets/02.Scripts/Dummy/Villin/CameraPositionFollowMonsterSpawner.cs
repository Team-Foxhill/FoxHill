using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using ObjectPoolManager = FoxHill.Tester.ObjectPoolManager;

namespace FoxHill.Core
{
    /// <summary>
    /// 이전 오브젝트 풀 기반으로 만들어진 카메라 주변에서 몬스터 생성하는 스크립트.
    /// </summary>
    public class CameraPositionFollowMonsterSpawner : MonoBehaviour
    {
        [SerializeField] private ObjectPoolManager _objectPoolManager;
        [SerializeField] private int _getCount;
        [SerializeField] private int _range;
        private int _randomValue;
        private Vector2 _newPosition;
        private Vector3 _worldSpawnPosition;
        private float _positionX;
        private float _positionY;
        private Camera _mainCamera;

        public void GetMonster()
        {
            GetFromPool(_getCount);
        }


        private void Start()
        {
            _mainCamera = Camera.main;
            GetFromPool(_getCount);
        }

        private void GetFromPool(int getCount)
        {
            for (int i = 0; i < getCount; i++)
            {
                GameObject newOne = _objectPoolManager.Get();
                if (newOne != null)
                {
                    _newPosition = GetSpawnPosition();
                    _worldSpawnPosition = _mainCamera.ViewportToWorldPoint(new Vector3(_newPosition.x, _newPosition.y, _mainCamera.nearClipPlane));

                    newOne.transform.position = _worldSpawnPosition;
                    newOne.SetActive(true);
                }
            }
        }

        private Vector2 GetSpawnPosition()
        {
            _randomValue = Random.Range(0, 4);
            switch (_randomValue)
            {                 //상대좌표          x 좌표,               y좌표.
                case 0: return new Vector2(Random.Range(0f, 1f), Random.Range(1f, 1.3f)); // 위쪽 영역
                case 1: return new Vector2(Random.Range(0f, 1f), Random.Range(0f, -0.3f)); // 아래쪽 영역
                case 2: return new Vector2(Random.Range(0f, -0.3f), Random.Range(0f, 1f)); // 좌측 영역
                case 3: return new Vector2(Random.Range(1f, 1.3f), Random.Range(0f, 1f)); // 우측 영역
                default: return new Vector2(0f, 0f);
            }
        }

    }
}
