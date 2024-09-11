using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Quest
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private QuestSheet _sheet; // Quest 명세가 담긴 엑셀 시트를 ExcelImporter로 import한 ScriptableObject
        [SerializeField] private List<QuestForm> _forms; // 엑셀 파싱 + 가공으로 얻은 Quest 데이터

        private void Awake()
        {
            InitializeQuestForms(_sheet);
        }

        private void InitializeQuestForms(QuestSheet sheet)
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
        }
    }
}