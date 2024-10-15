using UnityEngine;

namespace FoxHill.Player.HP
{
    public class PlayerHPController : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private PlayerHPUIController _uiController;

        private void Start()
        {
            _playerManager.OnPlayerDamaged?.AddListener(ProcessDamage);

            _playerManager.Inventory.OnUseRestorativeItem?.AddListener((item) =>
                Heal(item.Amount));

        }

        private void ProcessDamage(float damage)
        {
            float damageToApply = damage * (1 - _playerManager.Stat.Defense);
            _playerManager.Stat.CurrentHp -= damageToApply;

            _uiController.OnPlayerDamaged(_playerManager.Stat.CurrentHp / _playerManager.Stat.MaxHp);

            if (_playerManager.Stat.CurrentHp <= 0f)
            {
                _playerManager.Stat.CurrentHp = 0f;
                _uiController.OnPlayerDead();
                _playerManager.Dead();
            }
        }

        private void Heal(float amount)
        {
            _playerManager.Stat.CurrentHp += amount;

            if (_playerManager.Stat.CurrentHp > _playerManager.Stat.MaxHp)
            {
                _playerManager.Stat.CurrentHp = _playerManager.Stat.MaxHp;
            }

            _uiController.OnPlayerHealed(_playerManager.Stat.CurrentHp / _playerManager.Stat.MaxHp);
        }
    }
}
