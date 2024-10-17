using UnityEngine;

namespace FoxHill.Player.Data
{

    /// <summary>
    /// Player Stat의 초기값 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Create PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public float Hp = 100f;
        public float Power = 10f;
        public float Defense = 0.05f;
        public float MoveSpeed = 5f;
        public float AttackSpeed = 2f;
        public float DodgeCooldown = 4f;
    }
}