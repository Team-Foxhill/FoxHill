using FoxHill.Items;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Tower
{
    /// <summary>
    /// 타워 설치를 관리합니다.
    /// </summary>
    public class TowerManager : MonoBehaviour
    {
        private class Tower
        {
            public Tower(GameObject towerPrefab, SpriteRenderer sprite)
            {
                TowerPrefab = towerPrefab;
                Sprite = sprite;
            }

            public GameObject TowerPrefab { get; private set; }
            public SpriteRenderer Sprite { get; private set; }
        }

        private const float MOVE_OFFSET = 0.1f;

        [SerializeField] private List<GameObject> _towerPrefabs = new List<GameObject>(4); // 인스펙터에서 소환 가능한 타워 모두 연결
        private Dictionary<int, Tower> _towerRepository = new Dictionary<int, Tower>(4);

        private Tower _currentTower = null;
        [SerializeField] private SpriteRenderer _previewImage = null;

        private void Awake()
        {
            foreach (var tower in _towerPrefabs)
            {
                Tower newTower = new Tower(tower, tower.GetComponent<SpriteRenderer>());
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
            _previewImage.transform.localPosition = initialPosition;
        }
        
        public void ExitSpawnMode()
        {
            Debug.Log("exit");
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
            Instantiate(_currentTower.TowerPrefab, _previewImage.transform.position, Quaternion.identity);
            _previewImage.enabled = false;

            return true;
        }

        /// <summary>
        /// 입력에 따라 타워 스프라이트를 움직입니다.
        /// </summary>
        /// <param name="direction"></param>
        public void MoveTowerPreview(Vector2 direction)
        {
            Debug.Log(_currentTower);
            if ((direction == Vector2.up) || (direction == Vector2.down)
                || (direction == Vector2.left) || (direction == Vector2.right))
            {
                _previewImage.transform.localPosition += (Vector3)direction * MOVE_OFFSET;
            }

        }
    }
}