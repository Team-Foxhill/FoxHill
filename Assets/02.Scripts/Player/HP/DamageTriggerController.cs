using FoxHill.Core.Pause;
using FoxHill.Core.Utils;
using FoxHill.Monster;
using UnityEngine;

namespace FoxHill.Player.HP
{
    public class DamageTriggerController : MonoBehaviour, IPausable
    {
        [SerializeField] private PlayerManager _playerManager;
        private bool _isPaused = false;

        private void Awake()
        {
            PauseManager.Register(this);
            _playerManager ??= GetComponentInParent<PlayerManager>();
        }

        private void OnDestroy()
        {
            PauseManager.Unregister(this);  
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isPaused == true)
            {
                return;
            }

            if (collision.gameObject.layer == LayerRepository.LAYER_PATH_FOLLOW_MONSTER)
            {
                if (collision.TryGetComponent<PathFollowMonsterController>(out var monster) == true)
                {
                    _playerManager.TakeDamage(null, monster.Power);
                }
            }
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}