using FoxHill.Core;
using System;
using System.Collections.Generic;


namespace FoxHill.Monster
{
    public class MonsterDataManager
    {
        private static Dictionary<int, MonsterForm> _forms = new Dictionary<int, MonsterForm>(8); // 엑셀 파싱 + 가공으로 얻은 Monster 데이터.
        

        public static void InitializeMonsterForms(MonsterSheet sheet)
        {
            if (sheet == null)
            {
                throw new NullReferenceException("Cannot find sheet while InitializeMonsterForms");
            }

            _forms.Clear();

            foreach (var element in sheet.Sheet1)
            {
                var form = MonsterFormConverter.Convert(element);
                _forms.Add(form.MonsterIndexNumber, form);
            }

            DebugFox.Log($"Initialized {_forms.Count} Monsters");
        }

        public static bool TryGetMonster(int monsterNumber, out MonsterForm monster)
        {
            if (_forms.TryGetValue(monsterNumber, out monster) == false)
            {
                DebugFox.LogWarning($"Monster number {monsterNumber} not found.");
                return false;
            }
            else
            {
                return true;
            }
        }



    }
}
