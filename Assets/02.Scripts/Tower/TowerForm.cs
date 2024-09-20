namespace FoxHill.Tower
{

    public enum TowerType
    {
        AttackTower,
        DefenseTower,
        BuffTower
    }

    /// <summary>
    /// Excel Importer 사용 위해 자료형 변경한 클래스.
    /// 실제 게임 내에서는 TowerForm으로 변환하여 사용.
    /// </summary>
    [System.Serializable]
    public class TowerFormRaw
    {
        public int TowerIndexNumber;
        public string TowerType;
        public string TowerName;
        public float MaxHp;
        public float AttackSpeed;
        public float AttackRange;
        public float Power;
        public float Defense;
    }

    public class TowerForm
    {
        public int TowerIndexNumber;
        public TowerType TowerType;
        public string TowerName;
        public float MaxHp;
        public float AttackSpeed;
        public float AttackRange;
        public float Power;
        public float Defense;
    }
}
