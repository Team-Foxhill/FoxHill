using FoxHill.Core.Pause;
using FoxHill.Core.Utils;
using UnityEngine;

namespace FoxHill.Player.Exp
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExpTriggerController : MonoBehaviour, IPausable
    {
        [SerializeField] private PlayerManager _playerManager;
        private bool _isPaused = false;

        private void Awake()
        {
            _playerManager ??= GetComponentInParent<PlayerManager>();
            PauseManager.Register(this);
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isPaused == true)
            {
                return;
            }

            if (collision.gameObject.layer == LayerRepository.LAYER_EXP)
            {
                if (collision.gameObject.TryGetComponent<Core.Exp.Exp>(out var exp) == true)
                {
                    _playerManager.Stat.Exp += exp.Amount;
                    exp.Obtain(transform);
                }
            }
        }
    }
}