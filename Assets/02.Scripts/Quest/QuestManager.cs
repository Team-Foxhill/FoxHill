using System;
using System.Collections.Generic;
using System.Data.Common;
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
        private static Dictionary<int, QuestStatus> _status = new Dictionary<int, QuestStatus>(8); // 퀘스트들의 진행 상황

        /// <summary>
        /// quest sheet를 QuestForm의 형태로 변환하여 List에 저장하는 메소드
        /// </summary>
        /// <param name="sheet">ExcelImporter로 excel sheet를 변환하여 얻은 ScriptableObject</param>
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

        /// <summary>
        /// 지정한 questNumber의 QuestForm을 반환하는 메소드.
        /// 수행 가능 여부를 판단하지 않습니다.
        /// </summary>
        public static bool TryGetQuest(int questNumber, out QuestForm quest)
        {
            if (_forms.TryGetValue(questNumber, out quest) == false)
            {
                Debug.LogWarning($"Quest number {questNumber} not found.");
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 지정한 questNumber의 Quest의 시작 가능 여부를 반환하는 메소드
        /// </summary>
        /// <returns>시작 가능하다면 true, 유효하지 않은 questNumber이거나 시작 조건을 만족하지 않는다면 false</returns>
        public static bool CheckPreCondition(int questNumber, int parameter)
        {
            if (TryGetQuest(questNumber, out QuestForm quest) == false)
            {
                return false;
            }

            _status.TryGetValue(questNumber, out QuestStatus status);

            if (status != QuestStatus.NotStarted)
                return false;

            foreach (var preCondition in quest.PreCondition)
            {
                switch (preCondition.Type)
                {
                    case PreConditionType.HaveItem:
                        {
                            if (CheckPreCondition_HaveItem(parameter, preCondition.Value) == false)
                                return false;
                        }
                        break;
                    case PreConditionType.NotHaveItem:
                        {
                            if (CheckPreCondition_NotHaveItem(parameter, preCondition.Value) == false)
                                return false;
                        }
                        break;
                    case PreConditionType.MinPlayerLevel:
                        {
                            if (CheckPreCondition_MinPlayerLevel(parameter, preCondition.Value) == false)
                                return false;
                        }
                        break;
                    case PreConditionType.MaxPlayerLevel:
                        {
                            if (CheckPreCondition_MaxPlayerLevel(parameter, preCondition.Value) == false)
                                return false;
                        }
                        break;
                    case PreConditionType.ClearQuest:
                        {
                            if (CheckPreCondition_ClearQuest(parameter) == false)
                                return false;
                        }
                        break;
                    case PreConditionType.NotClearQuest:
                        {
                            if (CheckPreCondition_NotClearQuest(parameter) == false)
                                return false;
                        }
                        break;
                }
            }

            return true;
        }

        public static bool StartQuest(int questNumber, out QuestForm quest)
        {
            if (TryGetQuest(questNumber, out quest) == false)
            {
                return false;
            }

            _status[questNumber] = QuestStatus.InProgress;
            return true;
        }

        /// <summary>
        /// 퀘스트의 완료 조건을 만족했는지 판단 후 결과를 반환하는 메소드.
        /// QuestStatus는 TimeLimit 등을 비교하여 호출한 쪽에서 CheckQuestResult() 이용하여 설정해야 합니다.
        /// </summary>
        /// <param name="questNumber">판단하고자 하는 퀘스트의 QuestNumber</param>
        /// <returns>Quest 성공 여부</returns>
        public static bool IsQuestCompleted(int questNumber, int parameter)
        {
            if (TryGetQuest(questNumber, out QuestForm quest) == false)
            {
                return false;
            }

            _status.TryGetValue(questNumber, out QuestStatus status);

            if (status != QuestStatus.InProgress)
                return false;

            foreach (var objective in quest.Objective)
            {
                switch (objective.Type)
                {
                    case ObjectiveType.Collect:
                        {
                            if (CheckObjective_Collect(parameter, (int)objective.Value2) == false)
                                return false;
                        }
                        break;
                    case ObjectiveType.Kill:
                        {
                            if (CheckObjective_Kill(parameter, (int)objective.Value2) == false)
                                return false;
                        }
                        break;
                    case ObjectiveType.Deliver:
                        {
                            if (CheckObjective_Deliver(parameter, (int)objective.Value1) == false)
                                return false;
                        }
                        break;
                    case ObjectiveType.Escort:
                        {
                            if (CheckObjective_Escort(parameter, (int)objective.Value2) == false)
                                return false;
                        }
                        break;
                    case ObjectiveType.Talk:
                        {
                            if (CheckObjective_Talk(parameter, quest.EndNPC) == false)
                                return false;
                        }
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// 보상 혹은 패널티에 대한 정보를 지급하는 메소드.
        /// 호출하는 클래스에서 IsQuestCompleted로 판단 후 요구
        /// </summary>
        /// <param name="questNumber">퀘스트의 QuestNumber</param>
        /// <param name="rewards"></param>
        /// <returns></returns>
        public static bool TryGetQuestResult(int questNumber, out List<Reward> rewards, out List<Penalty> penalties)
        {
            if (TryGetQuest(questNumber, out QuestForm quest) == false)
            {
                Debug.LogWarning($"Quest number {questNumber} not found.");
                rewards = null;
                penalties = null;
                return false;
            }
            else
            {
                if (_status[questNumber] == QuestStatus.Cleared)
                {
                    rewards = quest.Reward;
                    penalties = null;
                }
                else if (_status[questNumber] == QuestStatus.Failed)
                {
                    rewards = null;
                    penalties = quest.Penalty;
                }
                else
                {
                    rewards = null;
                    penalties = null;
                    Debug.LogError($"Quest status of quest({questNumber}) is invalid to get reward / penalty : {_status[questNumber]}.");
                }
                return true;
            }
        }

        /// <summary>
        /// 퀘스트의 Status를 설정합니다.
        /// 퀘스트의 성공 여부는 호출한 곳에서 판단해서 hasSucceeded에 지정해야 합니다.
        /// </summary>
        /// <param name="questNumber">퀘스트의 QuestNumber</param>
        /// <param name="hasSucceeded">지정하고자 하는 QuestStatus</param>
        private static bool SetQuestResult(int questNumber, bool hasSucceeded)
        {
            if (TryGetQuest(questNumber, out QuestForm quest) == false)
            {
                return false;
            }

            if (hasSucceeded == true)
                _status[questNumber] = QuestStatus.Cleared;
            else
                _status[questNumber] = QuestStatus.Failed;

            return true;
        }

        #region Check Objective
        private static bool CheckObjective_Collect(int itemCount, int target)
        {
            return (itemCount >= target);
        }

        private static bool CheckObjective_Kill(int killCount, int target)
        {
            return (killCount >= target);
        }

        private static bool CheckObjective_Deliver(int hitCount, int target)
        {
            return (hitCount < target);
        }

        private static bool CheckObjective_Escort(int hitCount, int target)
        {
            return (hitCount < target);
        }

        private static bool CheckObjective_Talk(int talkNPC, int target)
        {
            return (talkNPC == target);
        }
        #endregion


        #region Check PreCondition
        private static bool CheckPreCondition_HaveItem(int itemCount, int target)
        {
            // TODO : Item 시스템 구축 후 제작 가능 :  Dictionary.containskey 같은거로 구현하면 될듯
            return true;
        }

        private static bool CheckPreCondition_NotHaveItem(int killCount, int target)
        {
            // TODO : Item 시스템 구축 후 제작 가능 :  Dictionary.containskey 같은거로 구현하면 될듯
            return true;
        }

        private static bool CheckPreCondition_MinPlayerLevel(int playerLevel, int target)
        {
            return (playerLevel >= target);
        }

        private static bool CheckPreCondition_MaxPlayerLevel(int playerLevel, int target)
        {
            return (playerLevel < target);
        }

        private static bool CheckPreCondition_ClearQuest(int questNumber)
        {
            if (TryGetQuest(questNumber, out QuestForm quest) == false)
            {
                return false;
            }

            return (_status[questNumber] == QuestStatus.Cleared);
        }

        private static bool CheckPreCondition_NotClearQuest(int questNumber)
        {
            if (TryGetQuest(questNumber, out QuestForm ques) == false)
            {
                return false;
            }

            return (_status[questNumber] == QuestStatus.Cleared);
        }
        #endregion
    }
}