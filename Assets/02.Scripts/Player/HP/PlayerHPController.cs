using UnityEngine;

namespace FoxHill.Player.HP
{
    public class PlayerHPController : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private PlayerHPUIController _uiController;

        // TODO : PlayerManager에서 PlayerDamaged 받으면
        // 데미지 처리 후 UIController 
        // 데미지 처리 중 죽으면 PlayerManager.PlayerDead invoke
    }
}
