using UnityEngine;

namespace FoxHill.Item
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Item : MonoBehaviour
    {
        public ItemData Info;
        private SpriteRenderer _icon;

        private void Awake()
        {
            _icon = GetComponent<SpriteRenderer>();
        }
    }
}