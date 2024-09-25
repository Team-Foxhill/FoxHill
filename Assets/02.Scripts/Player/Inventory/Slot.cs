using FoxHill.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoxHill.Player.Inventory
{ 
    public class Slot : MonoBehaviour
    {
        public Items.ItemData ItemInfo;
        public Image BackgroundImage;
        public Image ItemImage;
        public TMP_Text AmountText;

        public int Amount = 0;
    }
}