using FoxHill.Items;
using TMPro;
using UnityEngine;

namespace FoxHill.Player.Inventory
{
    public class ItemDescription : MonoBehaviour
    {
        [SerializeField] private TMP_Text _itemNameText;
        [SerializeField] private TMP_Text _itemStringText;

        private void Awake()
        {
            if (_itemNameText == null)
                _itemNameText = transform.Find("ItemName").GetChild(0).GetComponent<TMP_Text>();
            if (_itemStringText == null)
                _itemStringText = transform.Find("ItemString").GetChild(0).GetComponent<TMP_Text>();

            ClearDescription();
        }

        public void UpdateDescription(ItemData item)
        {
            _itemNameText.text = item.ItemName;
            _itemStringText.text = item.ItemString;
        }

        public void ClearDescription()
        { 
            _itemNameText.text = string.Empty;
            _itemStringText.text = string.Empty;
        }
    }
}