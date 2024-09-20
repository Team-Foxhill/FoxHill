using FoxHill.Core;
using System;
using System.Collections.Generic;


namespace FoxHill.Tower
{
    public class TowerDataManager
    {
        private static Dictionary<int, TowerForm> _forms = new Dictionary<int, TowerForm>(8); // 엑셀 파싱 + 가공으로 얻은 Tower 데이터.
        

        public static void InitializeMonsterForms(TowerData sheet)
        {
            if (sheet == null)
            {
                throw new NullReferenceException("Cannot find sheet while InitializeMonsterForms");
            }

            _forms.Clear();

            foreach (var element in sheet.Sheet1)
            {
                var form = TowerFormConverter.Convert(element);
                _forms.Add(form.TowerIndexNumber, form);
            }

            DebugFox.Log($"Initialized {_forms.Count} MonsterDatas");
        }

        public static bool TryGetTower(int towerNumber, out TowerForm tower)
        {
            if (_forms.TryGetValue(towerNumber, out tower) == false)
            {
                DebugFox.LogWarning($"Tower number {towerNumber} not found.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
