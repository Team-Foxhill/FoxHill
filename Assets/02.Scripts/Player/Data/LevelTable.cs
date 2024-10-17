using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Player.Data
{
    [CreateAssetMenu(fileName = "LevelTable", menuName = "Data/Create LevelTable")]
    public class LevelTable : ScriptableObject
    {
        public List<int> NeedExp;
    }
}