using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.Quest
{
    public static class QuestManager
    {
        public enum QuestStatus
        { 
            NotStarted, // 퀘스트 시작 이전
            InProgress, // 퀘스트 중
            Cleared,    // 퀘스트 성공
            Failed      // 퀘스트 실패
        }

        private static Dictionary<int, QuestForm> _forms = new Dictionary<int, QuestForm>(8); // 엑셀 파싱 + 가공으로 얻은 Quest 데이터 : {QuestNumber, QuestForm}
        private static Dictionary<int, QuestStatus> _status = new Dictionary<int, QuestStatus>(8); // 현재 수행중인 퀘스트의 인덱스
        private static int _currentIndex = -1;

        public static void InitializeQuestForms(QuestSheet sheet)
        {
            if (sheet == null)
            {
                throw new NullReferenceException("Cannot find sheet while InitializeQuestForms");
            }

            _forms.Clear();

            foreach (var element in sheet.Sheet1)
            {
                var form = QuestFormConverter.Convert(element);
                _forms.Add(form.QuestNumber, form);
                _status.Add(form.QuestNumber, QuestStatus.NotStarted);
            }
            
            Debug.Log($"Initialized {_forms.Count} Quests");
        }
    }
}