using FoxHill.Quest;
using UnityEngine;

namespace FoxHill.Player.Quest
{
    public class PlayerQuestManager
    {
        private const int NOT_IN_PROGRESS = 0;

        private int _currentIndex = NOT_IN_PROGRESS;
        private QuestForm _currentQuest;

        public bool TryStartQuest(int questNumber, int parameter = default)
        {
            // 현재 진행중인 퀘스트가 있다면
            if (_currentIndex != NOT_IN_PROGRESS)
            {
                return false;
            }

            // 퀘스트 시작 조건 만족하면
            if (QuestManager.CheckPreCondition(questNumber, parameter) == true)
            {
                if (QuestManager.StartQuest(questNumber, out _currentQuest) == false)
                {
                    Debug.LogError($"Failed to start quest {questNumber}");

                    return false;
                }

                _currentIndex = _currentQuest.QuestNumber;
                Debug.Log($"Started quest {questNumber}");

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}