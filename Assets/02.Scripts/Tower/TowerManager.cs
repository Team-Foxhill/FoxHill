using FoxHill.Items;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Tower
{
    /// <summary>
    /// 타워 설치를 관리합니다.
    /// </summary>
    public class TowerManager : MonoBehaviour
    {
        private class Tower
        {
            public Tower(GameObject towerPrefab, SpriteRenderer sprite, float size)
            {
                TowerPrefab = towerPrefab;
                Sprite = sprite;
                Size = size;
            }

            public GameObject TowerPrefab { get; private set; }
            public SpriteRenderer Sprite { get; private set; }
            public float Size { get; private set; } // 타워의 반지름 (공격 범위 X)
        }

        private const float SPAWN_VALID_RANGE = 4.5f;
        private const float PREVIEW_INITIAL_MOVE_SPEED = 2f;
        private const float PREVIEW_MAX_MOVE_SPEED = 8f;
        private readonly Color COLOR_TRANSPARENT_VALID = new Color(1f, 1f, 1f, 0.5f);
        private readonly Color COLOR_TRANSPARENT_INVALID = Color.red;

        [SerializeField] private List<GameObject> _towerPrefabs = new List<GameObject>(4); // 인스펙터에서 소환 가능한 타워 모두 연결
        private Dictionary<int, Tower> _towerRepository = new Dictionary<int, Tower>(4);

        private Tower _currentTower = null;
        [SerializeField] private SpriteRenderer _previewImage = null;

        private Vector2 _initialPosition;
        [SerializeField] private LayerMask _invalidPositionLayer; // 타워를 설치할 수 없는 위치의 Layer를 설정(ex. 지형, 몬스터, 타워)

        private Vector2 _lastMoveDirection;
        private float _moveSpeed = PREVIEW_INITIAL_MOVE_SPEED;

        private void Awake()
        {
            foreach (var tower in _towerPrefabs)
            {
                Tower newTower = new Tower(tower, tower.GetComponent<SpriteRenderer>(), tower.GetComponent<CircleCollider2D>().radius);
                int towerIndex = tower.GetComponent<TowerControllerBase>().Index;

                _towerRepository.Add(towerIndex, newTower);
            }

            _previewImage = transform.Find("PreviewImage").GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Spawn Mode로 진입합니다.
        /// Spawn Mode에서는 타워의 스프라이트를 표시하고 입력에 따라 이동, 설치할 수 있습니다.
        /// </summary>
        /// <param name="towerData">설치할 타워 데이터</param>
        public void EnterSpawnMode(ItemData towerData, Vector2 initialPosition)
        {
            _currentTower = _towerRepository[towerData.ItemNumber];

            _previewImage.enabled = true;
            _previewImage.sprite = _currentTower.Sprite.sprite;

            _initialPosition = initialPosition;
            _previewImage.transform.localPosition = _initialPosition;

            _previewImage.color = (CheckSpawnValidation() == true) ? COLOR_TRANSPARENT_VALID : COLOR_TRANSPARENT_INVALID;
        }

        public void ExitSpawnMode()
        {
            _currentTower = null;

            _previewImage.enabled = false;
            _previewImage.sprite = null;
        }

        /// <summary>
        /// 타워 설치를 시도합니다.
        /// 장애물이 있으면 설치할 수 없습니다.
        /// </summary>
        /// <returns>설치 시도 결과</returns>
        public bool TrySpawnTower()
        {
            if((CheckSpawnValidation() == true))
            {
                Instantiate(_currentTower.TowerPrefab, _previewImage.transform.position, Quaternion.identity);
                _previewImage.enabled = false;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 입력에 따라 타워 스프라이트를 움직입니다.
        /// </summary>
        /// <param name="direction"></param>
        public void MoveTowerPreview(Vector2 direction)
        {
            if ((direction == Vector2.up) || (direction == Vector2.down)
                || (direction == Vector2.left) || (direction == Vector2.right))
            {
                if(_lastMoveDirection == direction) // 같은 방향으로 계속 이동하는 경우 조금씩 가속
                {
                    _moveSpeed += 0.005f;
                    if(_moveSpeed > PREVIEW_MAX_MOVE_SPEED)
                    {
                        _moveSpeed = PREVIEW_MAX_MOVE_SPEED;
                    }
                }
                else // 다른 방향으로 이동하는 경우 초기화
                {
                    _moveSpeed = PREVIEW_INITIAL_MOVE_SPEED;
                }
                _previewImage.transform.localPosition += (Vector3)direction * _moveSpeed * Time.deltaTime;
            }

            _lastMoveDirection = direction;
            _previewImage.color = (CheckSpawnValidation() == true) ? COLOR_TRANSPARENT_VALID : COLOR_TRANSPARENT_INVALID;
        }

        /// <summary>
        /// 현재 위치에 타워를 설치할 수 있는지를 판단합니다.
        /// </summary>
        /// <returns>설치 가능 여부</returns>
        private bool CheckSpawnValidation()
        {
            if((_previewImage.transform.localPosition - (Vector3)_initialPosition).magnitude > SPAWN_VALID_RANGE)
            {
                return false;
            }

            var hits = Physics2D.OverlapCircleAll(_previewImage.transform.localPosition, _currentTower.Size, _invalidPositionLayer);
            if (hits.Length > 0)
            {
                return false;
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            if(_previewImage != null && _currentTower != null)
            {
                Gizmos.DrawWireSphere(_previewImage.transform.localPosition, _currentTower.Size);
            }
        }
    }
}