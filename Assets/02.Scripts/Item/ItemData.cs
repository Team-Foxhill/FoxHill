using UnityEngine;

namespace FoxHill.Item
{
    public enum ItemType
    {
        RestorativeItem,
        ConstructiveItem,
        QuestItem
    }

    public enum PerformType
    {
        RestorePlayerHP,
        ConstructTower,
        None
    }

    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Create ItemData")]
    public class ItemData : ScriptableObject
    {
        public int ItemNumber;
        public string ItemName;
        public string ItemString;
        public ItemType ItemType;
        public PerformType Perform;
        public float Amount;
        public float LastingTime;
        public float Range;
    }
}