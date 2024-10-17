using FoxHill.Core.Dialogue;
using FoxHill.Quest;
using System.Data.Common;
using UnityEngine;

namespace FoxHill.Player.Quest
{
    public class PlayerQuestManager : MonoBehaviour
    {
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private GlobalDialogue _globalDialogue;

        private const int NOT_IN_PROGRESS = -1;

        [SerializeField] private int _currentIndex = NOT_IN_PROGRESS;
        private QuestForm _currentQuest;

        private void Awake()
        {
            _playerManager ??= GetComponentInParent<PlayerManager>();
            _globalDialogue ??= FindFirstObjectByType<GlobalDialogue>();    
        }

        public bool TryStartQuest(int questNumber, int parameter = default)
        {
            // 현재 진행중인 퀘스트가 있다면
            if (_currentIndex != NOT_IN_PROGRESS)
            {
                return false;
            }

            if(QuestManager.TryGetQuest(questNumber, out var quest) == false)
            {
                return false;
            }
            
            foreach ( var preCondition in quest.PreCondition)
            {
                switch (preCondition.Type)
                {
                    case PreConditionType.HaveItem:
                        break;
                    case PreConditionType.NotHaveItem:
                        break;
                    case PreConditionType.MinPlayerLevel:
                        {
                            parameter = _playerManager.Stat.Level;
                        }
                        break;
                    case PreConditionType.MaxPlayerLevel:
                        {
                            parameter = _playerManager.Stat.Level;
                        }
                        break;
                    case PreConditionType.ClearQuest:
                        break;
                    case PreConditionType.NotClearQuest:
                        break;
                }

                if(StartQuest(questNumber, parameter) == true)
                {
                    _globalDialogue.StartDialogue(_currentQuest.StartDialogue);
                }
            }

            return true;
        }

        public bool IsQuestCompleted(int questNumber, int parameter = default)
        {
            return QuestManager.IsQuestCompleted(questNumber, parameter);
        }

        private bool StartQuest(int questNumber, int parameter)
        {
            // 퀘스트 시작 조건 만족하면
            if (QuestManager.CheckPreCondition(questNumber, parameter) == true)
            {
                if (QuestManager.StartQuest(questNumber, out _currentQuest) == false)
                {
                    return false;
                }

                _currentIndex = _currentQuest.QuestNumber;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}