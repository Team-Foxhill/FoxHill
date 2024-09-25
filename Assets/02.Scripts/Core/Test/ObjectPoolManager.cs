using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEditor;

namespace FoxHill.Core.Test
{
    public class PooledObjectData
    {
        public GameObject GameObject;
        public SpriteRenderer SpriteRenderer;
        public Collider2D Collider2D;
        public IPoolable Poolable;
        public Vector3 LastPosition;

        public PooledObjectData(GameObject go)
        {
            GameObject = go;
            SpriteRenderer = go.GetComponent<SpriteRenderer>();
            Collider2D = go.GetComponent<Collider2D>();
            Poolable = go.GetComponent<IPoolable>();
            LastPosition = go.transform.position;

            if (Poolable != null)
            {
                Poolable.OnRelease += HandleRelease;
            }
        }

        private void HandleRelease(IPoolable poolable)
        {
            ObjectPoolManager.Instance.Release(GameObject);
        }
    }

    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private int _capacity = 3000;
        [SerializeField] private float _checkInterval = 0.1f;
        [SerializeField] private float _outOfBoundsDistance = 1f;
        public Color gizmoColor = new Color(1, 0, 0, 0.2f);

        private Plane[] _frustumPlanes;
        private List<PooledObjectData> _pooledObjects;
        private Queue<int> _availableIndices;
        private Camera _mainCamera;
        private float _lastCheckTime;

        private void Awake()
        {
            //if (Instance == null)
            //{
            //    Instance = this;
            //    DontDestroyOnLoad(gameObject);
            //}
            //else
            //{
            //    Destroy(gameObject);
            //    return;
            //}

            _mainCamera = Camera.main;
            InitializePool();
            _frustumPlanes = new Plane[6];
        }

        private void InitializePool()
        {
            _pooledObjects = new List<PooledObjectData>(_capacity);
            _availableIndices = new Queue<int>(_capacity);

            for (int i = 0; i < _capacity; i++)
            {
                GameObject newObject = Instantiate(_itemPrefab);
                newObject.SetActive(false);
                _pooledObjects.Add(new PooledObjectData(newObject));
                _availableIndices.Enqueue(i);
            }
        }

        public GameObject Get()
        {
            if (_availableIndices.Count == 0)
            {
                Debug.LogWarning("Pool capacity reached!");
                return null;
            }

            int index = _availableIndices.Dequeue();
            PooledObjectData data = _pooledObjects[index];
            data.GameObject.SetActive(true);
            data.LastPosition = data.GameObject.transform.position;
            return data.GameObject;
        }

        public void Release(GameObject item)
        {
            int index = _pooledObjects.FindIndex(x => x.GameObject == item);
            if (index != -1)
            {
                _pooledObjects[index].GameObject.SetActive(false);
                _availableIndices.Enqueue(index);
            }
        }

        private void Update()
        {
            if (Time.time - _lastCheckTime >= _checkInterval)
            {
                UpdateFrustumPlanes();
                CheckObjectVisibility();
                _lastCheckTime = Time.time;
            }
        }

        private void UpdateFrustumPlanes()
        {
            GeometryUtility.CalculateFrustumPlanes(_mainCamera, _frustumPlanes);
        }


        private void CheckObjectVisibility()
        {
            Vector3 cameraPosition = _mainCamera.transform.position;

            NativeArray<Vector3> positions = new NativeArray<Vector3>(_pooledObjects.Count, Allocator.TempJob);
            NativeArray<bool> results = new NativeArray<bool>(_pooledObjects.Count, Allocator.TempJob);

            for (int i = 0; i < _pooledObjects.Count; i++)
            {
                positions[i] = _pooledObjects[i].GameObject.transform.position;
            }

            VisibilityCheckJob job = new VisibilityCheckJob
            {
                CameraPosition = cameraPosition,
                FrustumPlanes = new NativeArray<Plane>(_frustumPlanes, Allocator.TempJob),
                OutOfBoundsDistance = _outOfBoundsDistance,
                Positions = positions,
                Results = results
            };

            JobHandle jobHandle = job.Schedule(_pooledObjects.Count, 32);
            jobHandle.Complete();

            for (int i = 0; i < _pooledObjects.Count; i++)
            {
                PooledObjectData data = _pooledObjects[i];
                bool isVisible = results[i];
                data.SpriteRenderer.enabled = isVisible;
                data.Collider2D.enabled = isVisible;
            }

            job.FrustumPlanes.Dispose();
            positions.Dispose();
            results.Dispose();
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

    [BurstCompile]
    public struct VisibilityCheckJob : IJobParallelFor
    {
        [ReadOnly] public Vector3 CameraPosition;
        [ReadOnly] public NativeArray<Plane> FrustumPlanes;
        public float OutOfBoundsDistance;
        [ReadOnly] public NativeArray<Vector3> Positions;
        [WriteOnly] public NativeArray<bool> Results;

        public void Execute(int index)
        {
            Vector3 position = Positions[index];
            Vector3 viewportPosition = CameraPosition + (position - CameraPosition).normalized * OutOfBoundsDistance;
            bool isVisible = TestPlanesAABB(FrustumPlanes, new Bounds(position, Vector3.one));
            Results[index] = isVisible;
        }

        private bool TestPlanesAABB(NativeArray<Plane> planes, Bounds bounds)
        {
            for (int i = 0; i < planes.Length; i++)
            {
                Plane plane = planes[i];
                Vector3 normal = plane.normal;
                float distance = plane.distance;

                Vector3 positiveVertex = bounds.center +
                    new Vector3(normal.x >= 0 ? bounds.extents.x : -bounds.extents.x,
                                normal.y >= 0 ? bounds.extents.y : -bounds.extents.y,
                                normal.z >= 0 ? bounds.extents.z : -bounds.extents.z);

                if (normal.x * positiveVertex.x + normal.y * positiveVertex.y + normal.z * positiveVertex.z + distance < 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
