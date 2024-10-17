using FoxHill.Player.Data;
using FoxHill.Player.Exp;
using UnityEngine;

namespace FoxHill.Player.Stat
{
    public class PlayerStat : IPlayerStat
    {
        private PlayerLevelController _levelController;
        private PlayerData _data;

        public PlayerStat(PlayerData data, LevelTable levelTable)
        {
            _levelController = new PlayerLevelController(levelTable);
            _data = data;

            _maxHp = data.Hp;
            _currentHp = data.Hp;
            _moveSpeed = data.MoveSpeed;
            _power = data.Power;
            _defense = data.Defense;
            _attackSpeed = data.AttackSpeed;
            _dodgeCooldown = data.DodgeCooldown;
            _exp = 0f;
            _level = 1;
        }

        public float MaxHp
        {
            get => _data.Hp * Mathf.Pow(1.05f, _level);
            private set => _maxHp = value;
        }
        public float CurrentHp
        {
            get => _currentHp;
            set => _currentHp = value;
        }
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = value;
        }
        public float Power
        {
            get => _data.Power * Mathf.Pow(1.02f, _level);
            private set => _power = value;
        }
        public float Defense
        {
            get => _data.Defense * Mathf.Pow(1.01f, _level);
            private set => _defense = value;
        }
        public float AttackSpeed
        {
            get => _attackSpeed;
            private set => _attackSpeed = value;
        }
        public float DodgeCooldown
        {
            get => _dodgeCooldown;
            set => _dodgeCooldown = value;
        }
        public float Exp
        {
            get => _exp;
            set
            {
                _exp = value;
                _level = _levelController.CalculateLevel(_level, _exp);
            }
        }
        public int Level
        {
            get => _level;
            private set => _level = value;
        }


        private float _maxHp;
        private float _currentHp;
        private float _power;
        private float _defense;
        private float _moveSpeed;
        private float _attackSpeed;
        private float _dodgeCooldown;
        private float _exp;
        private int _level;
    }
}