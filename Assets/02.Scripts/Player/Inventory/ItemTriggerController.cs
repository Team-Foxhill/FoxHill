using FoxHill.Core.Pause;
using FoxHill.Core.Utils;
using UnityEngine;

namespace FoxHill.Player.Inventory
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ItemTriggerController : MonoBehaviour, IPausable
    {
        [SerializeField] private PlayerInventory _inventory;
        private bool _isPaused = false;


        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        private void Awake()
        {
            PauseManager.Register(this);
            _inventory ??= FindFirstObjectByType<PlayerInventory>();
        }

        private void OnDestroy()
        {
            PauseManager.Unregister(this);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(_isPaused == true)
            {
                return;
            }

            if (collision.gameObject.layer == LayerRepository.LAYER_ITEM)
            {
                var item = collision.gameObject.GetComponent<Items.Item>();
                if (_inventory.PushItem(item) == true)
                {
                    item.Obtain(transform);
                }
            }
        }
    }
}