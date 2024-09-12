using UnityEngine;

namespace FoxHill.Player.HP
{
    public class PlayerHPController : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private PlayerHPUIController _uiController;

        private void Start()
        {
            _playerManager.OnPlayerDamaged.AddListener(ProcessDamage);
        }

        private void ProcessDamage(float damage)
        {
            float damageToApply = damage * (1 - _playerManager.Stat.Defense);
            _playerManager.Stat.CurrentHp -= damageToApply;

            if (_playerManager.Stat.CurrentHp <= 0f)
            {
                _playerManager.Stat.CurrentHp = 0f;
                _playerManager.Dead();
            }
        }
    }
}
