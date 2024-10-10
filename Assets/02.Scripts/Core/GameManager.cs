using FoxHill.Core.Settings;
using FoxHill.Monster;
using FoxHill.Quest;
using FoxHill.Tower;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace FoxHill.Core
{
    /// <summary>
    /// 게임의 전체적인 flow를 관리하는 singleton 클래스
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public LanguageManager Language { get; private set; }

        [SerializeField] private LocalizationSettings _localizationSettings;
        [SerializeField] private MonsterData _monsterSheet; // Monster 명세가 담긴 엑셀 시트를 ExcelImporter로 import한 ScriptableObject.
        [SerializeField] private TowerData _towerSheet; // Tower 명세가 담긴 엑셀 시트를 ExcelImporter로 import한 ScriptableObject.
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
            
            Language = new LanguageManager(_localizationSettings);
            MonsterDataManager.InitializeMonsterForms(_monsterSheet);
            TowerDataManager.InitializeMonsterForms(_towerSheet);
            QuestManager.InitializeQuestForms(_questSheet);
        }
    }
}