using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Tower
{
    [ExcelAsset]
    public class TowerData : ScriptableObject
    {
        public List<TowerFormRaw> Sheet1;
    }
}