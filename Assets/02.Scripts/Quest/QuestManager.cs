using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Quest
{
    public static class QuestManager
    {
        private static List<QuestForm> _forms = new List<QuestForm>(8); // 엑셀 파싱 + 가공으로 얻은 Quest 데이터

        public static void InitializeQuestForms(QuestSheet sheet)
        {
            if (sheet == null)
            {
                throw new NullReferenceException("Cannot find sheet while InitializeQuestForms");
            }

            _forms.Clear();

            foreach (var element in sheet.Sheet1)
            {
                _forms.Add(QuestFormConverter.Convert(element));
            }

            Debug.Log($"Initialized {_forms.Count} Quests");
        }

        public static QuestForm GetForm(int index)
        {
            if (index < 0 || index >= _forms.Count)
            {
                Debug.LogWarning($"Cannot get QuestForm : invalid index {index}");
                return null;
            }

            return _forms[index];
        }
    }
}