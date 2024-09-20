using FoxHill.Core.Utils;
using FoxHill.Items;
using UnityEngine;

namespace FoxHill.Player.Inventory
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ItemTriggerController : MonoBehaviour
    {
        [SerializeField] private PlayerInventory _inventory;

        private void Awake()
        {
            if (_inventory == null)
                _inventory = FindFirstObjectByType<PlayerInventory>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_ITEM)
            {
                var item = collision.gameObject.GetComponent<Item>();
                if (_inventory.PushItem(item) == true)
                {
                    item.Obtain(transform);
                }
            }
        }
    }
}