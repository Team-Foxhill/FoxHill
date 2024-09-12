using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Monster
{
    [ExcelAsset]
    public class MonsterSheet : ScriptableObject
    {
        public List<MonsterFormRaw> Sheet1;
    }
}