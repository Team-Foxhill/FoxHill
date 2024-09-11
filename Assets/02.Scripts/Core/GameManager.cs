using FoxHill.Quest;
using UnityEngine;

namespace FoxHill.Core
{
    /// <summary>
    /// 게임의 전체적인 flow를 관리하는 singleton 클래스
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private QuestSheet _questSheet; // Quest 명세가 담긴 엑셀 시트를 ExcelImporter로 import한 ScriptableObject

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            QuestManager.InitializeQuestForms(_questSheet);
        }
    }
}