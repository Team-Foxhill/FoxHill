using FoxHill.Core.Dialogue;
using FoxHill.Core.Utils;
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
        [SerializeField] private int _number;
        [SerializeField] private DialogueHUD _dialogueHUD;

        private void Awake()
        {
            _dialogueHUD ??= GetComponentInChildren<DialogueHUD>();
        }

        private void Start()
        {
            _questIndices = QuestManager.InitializeNPCQuests(_number);
            _dialogueHUD.ToggleUI(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PLAYER)
            {
                PlayerQuestManager playerQuest = collision.GetComponentInChildren<PlayerQuestManager>();

                foreach (int index in _questIndices)
                {
                    if (playerQuest.TryStartQuest(index) == true)
                    {
                        _dialogueHUD.SetText($"퀘스트{index}를 시작합니다.");
                        _dialogueHUD.ToggleUI(true);
                    }
                    if (playerQuest.IsQuestCompleted(index) == true)
                    {
                        _dialogueHUD.SetText($"퀘스트{index}를 완료했습니다.");
                        _dialogueHUD.ToggleUI(true);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerRepository.LAYER_PLAYER)
            {
                _dialogueHUD.SetText("");
                _dialogueHUD.ToggleUI(false);
            }
        }
    }
}