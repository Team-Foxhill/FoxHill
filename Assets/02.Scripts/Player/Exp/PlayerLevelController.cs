using FoxHill.Player.Data;
using UnityEngine;

namespace FoxHill.Player.Exp
{
    public class PlayerLevelController
    {
        public PlayerLevelController(LevelTable table)
        {
            _table = table;
        }

        private LevelTable _table;

        public int CalculateLevel(int currentLevel, float currentExp)
        {
            Debug.Log(currentLevel + " " + currentExp);
            if (_table.NeedExp[currentLevel - 1] <= currentExp)
            {
                return currentLevel + 1;
            }
            else
            {
                return currentLevel;
            }
        }
    }
}