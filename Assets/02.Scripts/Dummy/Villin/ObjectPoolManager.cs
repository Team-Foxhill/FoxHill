using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using FoxHill.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FoxHill.Tester
{
    /// <summary>
    /// 현재 사용하지 않는 클래스.
    /// 범용적으로 GameObject 를 풀링하기위한 클래스
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        [SerializeField] private GameObject _item;
        [SerializeField] private int _capacity = 200;
        [SerializeField] private int _maxSize = 400;
        [SerializeField] private float _checkInterval = 5f;
        [SerializeField] private float _outOfBoundsDistance = 0f;
        private WaitForSecondsRealtime _interval;
        private IObjectPool<GameObject> _pool;
        private Camera _mainCamera;
        //private List<GameObject> _activeItem = new List<GameObject>();
        private Dictionary<GameObject, SpriteRenderer> _activeItemDictionary = new Dictionary<GameObject, SpriteRenderer>();
        private Dictionary<SpriteRenderer, Collider2D> _itemEnvironmentDictionary = new Dictionary<SpriteRenderer, Collider2D>();
        [Header("Gizmo Settings")]
        public Color gizmoColor = new Color(1, 0, 0, 0.2f);

        private void Awake()
        {
            _mainCamera = Camera.main;
            _interval = new WaitForSecondsRealtime(_checkInterval);
            InitializePool();
            StartCoroutine(CheckBounds());
        }

        private IEnumerator CheckBounds()
        {
            while (true)
            {
                yield return _interval;
                CheckItemsOutOfView();
                CheckItemsInOfView();
                DebugFox.Log("BoundChecked");
            }
        }

        /// <summary>
        /// 풀링된 객체가 화면 바깥으로 나갔을 경우 스프라이트 렌더러 끄기.
        /// </summary>
        private void CheckItemsOutOfView()
        {
            foreach (var kvp in _activeItemDictionary)
            {
                GameObject item = kvp.Key;
                SpriteRenderer spriteRenderer = kvp.Value;
                if (IsItemOutOfView(item.transform.position) == true)
                {
                    spriteRenderer.enabled = false;
                    _itemEnvironmentDictionary.TryGetValue(spriteRenderer, out Collider2D collider2D);
                    collider2D.enabled = false;
                    DebugFox.Log($"disabled {item} renderer");
                }
            }
        }
        /// <summary>
        /// 풀링된 객체가 화면 안으로 돌아을 경우 스프라이트 렌더러 켜기.
        /// </summary>
        private void CheckItemsInOfView()
        {
            foreach (var kvp in _activeItemDictionary)
            {
                GameObject item = kvp.Key;
                SpriteRenderer spriteRenderer = kvp.Value;

                if (IsItemInView(item.transform.position) == true)
                {
                    if (!spriteRenderer.enabled)
                    {
                        spriteRenderer.enabled = true;
                        _itemEnvironmentDictionary.TryGetValue(spriteRenderer, out Collider2D collider2D);
                        collider2D.enabled = true;
                        DebugFox.Log($"Enabled {item} spriteRenderer");
                    }
                }
            }
        }

        private bool IsItemOutOfView(Vector3 position)
        {
            Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(position);
            return viewportPosition.x < -_outOfBoundsDistance ||
                   viewportPosition.x > 1 + _outOfBoundsDistance ||
                   viewportPosition.y < -_outOfBoundsDistance ||
                   viewportPosition.y > 1 + _outOfBoundsDistance;
        }

        private bool IsItemInView(Vector3 position)
        {
            Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(position);
            return viewportPosition.x >= -_outOfBoundsDistance &&
                   viewportPosition.x <= 1 + _outOfBoundsDistance &&
                   viewportPosition.y >= -_outOfBoundsDistance &&
                   viewportPosition.y <= 1 + _outOfBoundsDistance;
        }

        /// <summary>
        /// 아이템 가져오기
        /// </summary>
        /// <returns> 풀링된 아이템 </returns>
        public GameObject Get()
        {
            return _pool.Get();
        }

        /// <summary>
        /// 아이템 반납하기
        /// </summary>
        /// <param name="item"> 풀링될 아이템 </param>
        public void Release(GameObject item)
        {
            _pool.Release(item);
        }

        /// <summary>
        /// PoolType 에 따른 Pool 초기화
        /// </summary>
        void InitializePool()
        {
            _pool = new ObjectPool<GameObject>(CreatePooledItem,
                                               OnGet,
                                               OnRelease,
                                               OnRemove,
                                               true,
                                               _capacity,
                                               _maxSize);
        }

        /// <summary>
        /// 풀링할 아이템 생성
        /// </summary>
        /// <returns> 생성된 풀링할 아이템 </returns>
        GameObject CreatePooledItem()
        {
            return Instantiate(_item);
        }

        /// <summary>
        /// 풀링된 아이템을 가져다 쓸때 수행할 내용
        /// </summary>
        /// <param name="pooledItem"> 가져다쓰려는 아이템 </param>
        void OnGet(GameObject pooledItem)
        {
            //_activeItem.Add(pooledItem);
            _activeItemDictionary.Add(pooledItem, pooledItem.GetComponent<SpriteRenderer>());
            _itemEnvironmentDictionary.Add(pooledItem.GetComponent<SpriteRenderer>(), pooledItem.GetComponent<Collider2D>());
            pooledItem.SetActive(true);
        }

        /// <summary>
        /// 풀링된 아이템을 반납할때 수행할 내용
        /// </summary>
        /// <param name="pooledItem"> 반납하려는 아이템 </param>
        void OnRelease(GameObject pooledItem)
        {
            pooledItem.SetActive(false);
            _activeItemDictionary.Remove(pooledItem);
            _itemEnvironmentDictionary.Remove(pooledItem.GetComponent<SpriteRenderer>());
            //_activeItem.Remove(pooledItem);
        }

        /// <summary>
        /// 풀링된 아이템을 풀 목록에서 제거할때 수행할 내용
        /// </summary>
        /// <param name="pooledItem"> 제거하려는 아이템 </param>
        void OnRemove(GameObject pooledItem)
        {
            Destroy(pooledItem);
        }

        #region Gizmo
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_mainCamera == null) return;

            Gizmos.color = gizmoColor;

            Vector3 bottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(-_outOfBoundsDistance, -_outOfBoundsDistance, _mainCamera.nearClipPlane));
            Vector3 topLeft = _mainCamera.ViewportToWorldPoint(new Vector3(-_outOfBoundsDistance, 1 + _outOfBoundsDistance, _mainCamera.nearClipPlane));
            Vector3 topRight = _mainCamera.ViewportToWorldPoint(new Vector3(1 + _outOfBoundsDistance, 1 + _outOfBoundsDistance, _mainCamera.nearClipPlane));
            Vector3 bottomRight = _mainCamera.ViewportToWorldPoint(new Vector3(1 + _outOfBoundsDistance, -_outOfBoundsDistance, _mainCamera.nearClipPlane));

            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);

            Handles.DrawSolidRectangleWithOutline(new Vector3[] { bottomLeft, topLeft, topRight, bottomRight }, gizmoColor, Color.clear);

            // 실제 뷰포트 경계 표시
            Gizmos.color = Color.yellow;
            Vector3 viewportBottomLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
            Vector3 viewportTopLeft = _mainCamera.ViewportToWorldPoint(new Vector3(0, 1, _mainCamera.nearClipPlane));
            Vector3 viewportTopRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));
            Vector3 viewportBottomRight = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0, _mainCamera.nearClipPlane));

            Gizmos.DrawLine(viewportBottomLeft, viewportTopLeft);
            Gizmos.DrawLine(viewportTopLeft, viewportTopRight);
            Gizmos.DrawLine(viewportTopRight, viewportBottomRight);
            Gizmos.DrawLine(viewportBottomRight, viewportBottomLeft);
        }
#endif
        #endregion
    }
}