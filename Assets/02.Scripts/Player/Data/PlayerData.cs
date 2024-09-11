using UnityEngine;

namespace FoxHill.Player.Data
{
    /// <summary>
    /// Initial stat data of the player
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Create PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public float Hp = 100f;
        public float Exp = 40f;
        public float MoveSpeed = 5f;
        public float NormalAttackSpeed = 2f;
        public float NormalAttackDamage = 10f;
        public int Level = 1;
    }
}