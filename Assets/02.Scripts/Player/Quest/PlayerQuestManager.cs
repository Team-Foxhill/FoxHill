using FoxHill.Core;
using FoxHill.Core.Dialogue;
using FoxHill.Items;
using FoxHill.Quest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FoxHill.Player.Quest
{
    public class PlayerQuestManager : MonoBehaviour
    {
        public bool IsQuestInProgress => _currentIndex != NOT_IN_PROGRESS;
        [HideInInspector] public UnityEvent OnQuestStatusUpdated;

        [SerializeField] public PlayerManager PlayerManager { get; private set; }
        [SerializeField] private GlobalDialogue _globalDialogue;

        private const int NOT_IN_PROGRESS = -1;

        [SerializeField] private int _currentIndex = NOT_IN_PROGRESS;
        private QuestForm _currentQuest;
        private Dictionary<Objective, bool> _currentObjectives = new Dictionary<Objective, bool>(3);
        private Dictionary<int, bool> _gotReward = new Dictionary<int, bool>(3);

        // Objective용 변수
        // Kill
        private int _monsterKillCount = 0; 
        // Deliever
        private int _deliverItemIndex = 0;
        private bool _hasDelivered = false;

        private void Awake()
        {
            PlayerManager ??= GetComponentInParent<PlayerManager>();
            _globalDialogue ??= FindFirstObjectByType<GlobalDialogue>();
        }

        private void Start()
        {
            PlayerManager.OnReset?.AddListener(this.OnReset);
        }

        private void OnReset()
        {
            _currentQuest = null;
            _currentIndex = NOT_IN_PROGRESS;
            _currentObjectives.Clear();
            _globalDialogue ??= FindFirstObjectByType<GlobalDialogue>();
            _gotReward.Clear();
        }

        public bool TryStartQuest(int questNumber, int parameter = default)
        {
            // 현재 진행중인 퀘스트가 있다면
            if (_currentIndex != NOT_IN_PROGRESS)
            {
                return false;
            }

            // 이미 진행 완료 후 보상까지 수령한 퀘스트라면
            if (_gotReward.ContainsKey(questNumber) == true
                && _gotReward[questNumber] == true)
            {
                return false;
            }

            if (QuestManager.TryGetQuest(questNumber, out var quest) == false)
            {
                return false;
            }

            foreach (var preCondition in quest.PreCondition)
            {
                switch (preCondition.Type)
                {
                    case PreConditionType.HaveItem:
                        break;
                    case PreConditionType.NotHaveItem:
                        break;
                    case PreConditionType.MinPlayerLevel:
                        {
                            parameter = PlayerManager.Stat.Level;
                        }
                        break;
                    case PreConditionType.MaxPlayerLevel:
                        {
                            parameter = PlayerManager.Stat.Level;
                        }
                        break;
                    case PreConditionType.ClearQuest:
                        break;
                    case PreConditionType.NotClearQuest:
                        break;
                }

                if (StartQuest(questNumber, parameter) == true)
                {
                    _globalDialogue ??= FindFirstObjectByType<GlobalDialogue>();
                    _globalDialogue.StartDialogue(_currentQuest.StartDialogue);

                    if (_gotReward.ContainsKey(questNumber) == false)
                    {
                        _gotReward.Add(questNumber, false);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void ShowSuccessDialogue()
        {
            if (_currentIndex == NOT_IN_PROGRESS)
                return;

            _globalDialogue ??= FindFirstObjectByType<GlobalDialogue>();
            _globalDialogue.StartDialogue(_currentQuest.SuccessDialogue);
        }

        public bool IsQuestCompleted(int questNumber, int parameter = default)
        {
            // 이미 진행 완료 후 보상까지 수령한 퀘스트라면 판단 범위 X
            if (_gotReward.ContainsKey(questNumber) == true 
                && _gotReward[questNumber] == true)
            {
                return false;
            }

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

                StartCoroutine(C_StartTrackingQuest(_currentQuest));
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator C_StartTrackingQuest(QuestForm quest)
        {
            bool isCleared = false;

            foreach (var objective in quest.Objective)
            {
                _currentObjectives.Add(objective, false);

                // Objective 설정
                switch (objective.Type)
                {
                    case ObjectiveType.Collect:
                        break;
                    case ObjectiveType.Kill:
                        {
                            PlayerManager.OnKillMonster?.AddListener(OnKillMonster);
                        }
                        break;
                    case ObjectiveType.Deliver:
                        {
                            if(DropManager.Instance.TryGetItem((int)objective.Value1, out var itemPrefab) == true)
                            {
                                if(itemPrefab.TryGetComponent(out Item item) == true)
                                {
                                    if(PlayerManager.Inventory.PushItem(item) == true)
                                    {
                                        _deliverItemIndex = (int)objective.Value1;
                                        PlayerManager.OnEncounterNPC?.AddListener(OnEncounterNPC);
                                    }
                                    else
                                    {
                                        DebugFox.LogError($"Failed to get {item}");
                                    }
                                }
                            }

                        }
                        break;
                    case ObjectiveType.Escort:
                        break;
                    case ObjectiveType.Talk:
                        { 
                            _currentObjectives[objective] = true;
                        }
                        break;
                }
            }

            OnQuestStatusUpdated?.Invoke();

            // 퀘스트 완료 조건 만족 확인
            while (isCleared == false)
            {
                if (PlayerManager.IsPaused == true)
                {
                    yield return new WaitUntil(() => PlayerManager.IsPaused == false);
                }

                isCleared = true;
                int objectiveCount = _currentObjectives.Count;
                for (int i = 0; i < objectiveCount; i++)
                {
                    if(_currentObjectives.TryGetValue(_currentQuest.Objective[i], out bool value) == true)
                    {
                        if (value == true)
                            continue;
                    }
                    else
                    {
                        DebugFox.LogError($"Failed to get objective {_currentQuest.Objective[i]}");
                        continue;
                    }

                    switch (_currentQuest.Objective[i].Type)
                    {
                        case ObjectiveType.Collect:
                            break;
                        case ObjectiveType.Kill:
                            {
                                if (QuestManager.IsQuestCompleted(_currentIndex, _monsterKillCount) == false)
                                {
                                    isCleared = false;
                                }
                                else
                                {
                                    _currentObjectives[_currentQuest.Objective[i]] = true;
                                }
                            }
                            break;
                        case ObjectiveType.Deliver:
                            {
                                if (_hasDelivered == false)
                                {
                                    isCleared = false;
                                }
                                else
                                {
                                    _currentObjectives[_currentQuest.Objective[i]] = true;
                                }
                            }
                            break;
                        case ObjectiveType.Escort:
                            break;
                        case ObjectiveType.Talk:
                            {
                                _currentObjectives[_currentQuest.Objective[i]] = true;
                            }
                            break;
                    }
                }

                yield return null;
            }

            // 퀘스트 성공
            QuestManager.SetQuestResult(_currentIndex, true);
        }

        private void OnKillMonster()
        {
            _monsterKillCount++;
            OnQuestStatusUpdated?.Invoke();
        }

        private void OnEncounterNPC(int npcIndex)
        {
            if(_currentIndex == NOT_IN_PROGRESS)
            {
                return;
            }    

            if(npcIndex == _currentQuest.EndNPC)
            {
                if (PlayerManager.Inventory.HasItem(_deliverItemIndex) == true)
                {
                    _hasDelivered = true;
                }
            }
        }

        public void OnClearQuest()
        {
            foreach (var objective in _currentObjectives.Keys)
            {
                switch (objective.Type)
                {
                    case ObjectiveType.Collect:
                        break;
                    case ObjectiveType.Kill:
                        {
                            PlayerManager.OnKillMonster?.RemoveListener(OnKillMonster);
                            _monsterKillCount = 0;
                        }
                        break;
                    case ObjectiveType.Deliver:
                        {
                            PlayerManager.OnEncounterNPC?.RemoveListener(OnEncounterNPC);
                            PlayerManager.Inventory.UseItem(_deliverItemIndex);
                            _deliverItemIndex = 0;
                            _hasDelivered = false;
                        }
                        break;
                    case ObjectiveType.Escort:
                        break;
                    case ObjectiveType.Talk:
                        break;
                }
            }

            if(QuestManager.TryGetQuestResult(_currentIndex, out List<Reward> rewards, out List<Penalty> penalties) == true)
            {
                PlayerManager.OnClearQuest?.Invoke(rewards);
            }

            _gotReward[_currentIndex] = true;

            _currentObjectives.Clear();
            _currentIndex = NOT_IN_PROGRESS;
            _currentQuest = null;

            OnQuestStatusUpdated?.Invoke();
        }

        public bool GetQuestStatus(out ObjectiveType type, ref float value1, ref float value2) // UI에 전달할 정보를 제공하는 함수
        {
            if (_currentIndex == NOT_IN_PROGRESS)
            {
                type = default;
                return false;
            }

            foreach (var objective in _currentObjectives.Keys)
            {
                switch (objective.Type)
                {
                    case ObjectiveType.Collect:
                        break;
                    case ObjectiveType.Kill:
                        {
                            value1 = _monsterKillCount; // 현재 달성 수치
                            value2 = objective.Value2; // 총 목표 수치
                            type = ObjectiveType.Kill;
                        }
                        return true;
                    case ObjectiveType.Deliver:
                        {
                            value1 = objective.Value1;
                            type = ObjectiveType.Deliver;
                        }
                        return true;
                    case ObjectiveType.Escort:
                        break;
                    case ObjectiveType.Talk:
                        break;
                }
            }

            type = default;
            return false;
        }
    }
}