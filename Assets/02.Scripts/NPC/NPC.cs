using FoxHill.Core;
using FoxHill.Core.Dialogue;
using FoxHill.Core.Utils;
using FoxHill.Player;
using FoxHill.Player.Quest;
using FoxHill.Quest;
using System.Collections.Generic;
using UnityEngine;

namespace FoxHill.NPC
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class NPC : MonoBehaviour
    {
        private List<int> _questIndices = new List<int>();
        private int _availableQuestCount = 0;

        [SerializeField] private int _number;
        [SerializeField] private DialogueHUD _dialogueHUD;

        private void Awake()
        {
            _dialogueHUD ??= GetComponentInChildren<DialogueHUD>();
        }

        private void Start()
        {
            _questIndices = QuestManager.InitializeNPCQuests(_number);
            _availableQuestCount = _questIndices.Count;
            _dialogueHUD.ToggleUI(true);

            switch (GameManager.Instance.Language.CurrentLanguage)
            {
                case Core.Settings.LanguageManager.LanguageType.Korean:
                    {
                        _dialogueHUD.SetText($"실행할 수 있는 임무가 {_availableQuestCount}개 있네.");
                    }
                    break;
                case Core.Settings.LanguageManager.LanguageType.English:
                    {
                        _dialogueHUD.SetText($"There are {_availableQuestCount} missions available.");
                    }
                    break;
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PLAYER)
            {
                PlayerQuestManager playerQuest = collision.GetComponentInChildren<PlayerQuestManager>();
                playerQuest.PlayerManager.OnEncounterNPC?.Invoke(_number);

                foreach (int index in _questIndices)
                {
                    if (playerQuest.IsQuestInProgress == false
                        && playerQuest.TryStartQuest(index) == true)
                    {
                        switch (GameManager.Instance.Language.CurrentLanguage)
                        {
                            case Core.Settings.LanguageManager.LanguageType.Korean:
                                {
                                    _dialogueHUD.SetText($"임무 {index}를 시작합니다.");
                                }
                                break;
                            case Core.Settings.LanguageManager.LanguageType.English:
                                {
                                    _dialogueHUD.SetText($"Commencing mission {index}.");
                                }
                                break;
                        }
                        _availableQuestCount--;
                        _dialogueHUD.ToggleUI(true);

                        break;
                    }
                    if (playerQuest.IsQuestInProgress == true
                        && playerQuest.IsQuestCompleted(index) == true)
                    {
                        switch (GameManager.Instance.Language.CurrentLanguage)
                        {
                            case Core.Settings.LanguageManager.LanguageType.Korean:
                                {
                                    _dialogueHUD.SetText($"임무 {index}를 완료했습니다.");
                                }
                                break;
                            case Core.Settings.LanguageManager.LanguageType.English:
                                {
                                    _dialogueHUD.SetText($"Quest {index} completed.");
                                }
                                break;
                        }
                        _dialogueHUD.ToggleUI(true);
                        playerQuest.ShowSuccessDialogue();
                        playerQuest.OnClearQuest();

                        break;
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PLAYER)
            {
                switch (GameManager.Instance.Language.CurrentLanguage)
                {
                    case Core.Settings.LanguageManager.LanguageType.Korean:
                        {
                            _dialogueHUD.SetText($"실행할 수 있는 임무가 {_availableQuestCount}개 있네.");
                        }
                        break;
                    case Core.Settings.LanguageManager.LanguageType.English:
                        {
                            _dialogueHUD.SetText($"There are {_availableQuestCount} missions available.");
                        }
                        break;
                }
            }
        }
    }
}