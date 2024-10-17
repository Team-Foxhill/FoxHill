using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Items
{
    [CreateAssetMenu(fileName = "ItemDropTable", menuName = "Data/Create ItemDropTable")]
    public class ItemDropTable : ScriptableObject
    {
        [Header("드롭율의 범위는 0 이상 1 이하입니다.")]
        public List<DropItem> Items = new List<DropItem>();

        [Serializable]
        public class DropItem
        {
            public GameObject ItemPrefab;
            public float DropRate;
            public int ItemIndex
            {
                get => ItemPrefab.GetComponent<Item>().Info.ItemNumber;
            }
        }
    }
}