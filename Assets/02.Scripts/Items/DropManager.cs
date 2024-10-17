using FoxHill.Core.Exp;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Items
{
    /// <summary>
    /// 아이템이나 경험치 등 드롭되는 아이템들을 관리합니다.
    /// </summary>
    public class DropManager : MonoBehaviour
    {
        public static DropManager Instance { get; private set; }

        [SerializeField] private ItemDropTable _dropTable;
        private static Dictionary<int, GameObject> _itemPrefabs = new Dictionary<int, GameObject>(4);
        [SerializeField] private GameObject _expPrefab;

        private float _dropRange = 2f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(Instance);
            }

            foreach (var item in _dropTable.Items)
            {
                _itemPrefabs.Add(item.ItemIndex, item.ItemPrefab);
            }
        }

        public void DropExp(Transform spawnTransform, float expAmount = 10f)
        {
            Vector3 randomPosition = spawnTransform.position + Random.insideUnitSphere * _dropRange;
            var newExp = Instantiate(_expPrefab, randomPosition, Quaternion.identity, null);
            newExp.GetComponent<Exp>().Amount = expAmount;
        }

        public bool TryGetItem(int index, out GameObject item)
        {
            return _itemPrefabs.TryGetValue(index, out item);
        }

        public void DropItem(Transform spawnTransform)
        {
            float randomValue = Random.value;

            foreach (var item in _dropTable.Items)
            {
                if (randomValue <= item.DropRate)
                {
                    Vector3 randomPosition = spawnTransform.position + Random.insideUnitSphere * _dropRange;
                    Instantiate(item.ItemPrefab, randomPosition, Quaternion.identity, null);
                }
            }
        }
    }
}