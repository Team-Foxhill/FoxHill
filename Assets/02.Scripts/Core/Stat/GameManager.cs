using FoxHill.Monster;
using UnityEngine;

namespace FoxHill.Core
{
    /// <summary>
    /// 게임의 전체적인 flow를 관리하는 singleton 클래스
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private MonsterSheet _monsterSheet; // Monster 명세가 담긴 엑셀 시트를 ExcelImporter로 import한 ScriptableObject.

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

            MonsterDataManager.InitializeMonsterForms(_monsterSheet);
        }
    }
}