using FoxHill.Core;
using FoxHill.Quest;
using TMPro;
using UnityEngine;

namespace FoxHill.Player.Quest
{
    public class PlayerQuestUI : MonoBehaviour
    {
        [SerializeField] private PlayerQuestManager _playerQuestManager;

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _statusText;

        private void Awake()
        {
            _playerQuestManager ??= FindFirstObjectByType<PlayerQuestManager>();

            _titleText ??= transform.GetChild(0).Find("Text (TMP)_QuestTitle").GetComponent<TMP_Text>();
            _statusText ??= transform.GetChild(0).Find("Text (TMP)_QuestStatus").GetComponent<TMP_Text>();
        }

        private void Start()
        {
            _playerQuestManager.PlayerManager.OnReset?.AddListener(OnReset);
            _playerQuestManager.OnQuestStatusUpdated?.AddListener(SetText);

            SetText();
        }

        private void SetText()
        {
            switch (GameManager.Instance.Language.CurrentLanguage)
            {
                case Core.Settings.LanguageManager.LanguageType.Korean:
                    {
                        if(_playerQuestManager.IsQuestInProgress == false)
                        {
                            _titleText.text = "진행중인 임무가 없습니다.";
                            _statusText.text = "";
                        }
                        else
                        {
                            float value1 = -1, value2 = -1;
                            if(_playerQuestManager.GetQuestStatus(out ObjectiveType type, ref value1, ref value2) == true)
                            {
                                _titleText.text = $"임무 : {type}";
                                
                                if(value2 == -1)
                                {
                                    _statusText.text = value1.ToString();
                                }
                                else if(value1 != -1 && value2 != -1)
                                {
                                    _statusText.text = $"{value1} / {value2}";
                                }
                            }
                        }
                    }
                    break;
                case Core.Settings.LanguageManager.LanguageType.English:
                    {
                        if (_playerQuestManager.IsQuestInProgress == false)
                        {
                            _titleText.text = "No active quests.";
                            _statusText.text = "";
                        }
                        else
                        {
                            float value1 = -1, value2 = -1;
                            if (_playerQuestManager.GetQuestStatus(out ObjectiveType type, ref value1, ref value2) == true)
                            {
                                _titleText.text = $"Quest : {type}";

                                if (value2 == -1)
                                {
                                    _statusText.text = value1.ToString();
                                }
                                else if (value1 != -1 && value2 != -1)
                                {
                                    _statusText.text = $"{value1} / {value2}";
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void OnReset()
        {
            _playerQuestManager.OnQuestStatusUpdated?.AddListener(SetText);
        }
    }
}