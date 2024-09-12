namespace FoxHill.Monster
{
    public enum MonsterType
    {
        PathFollowMonster,
        PlayerFollowMonster,
        BehaviourTreeMonster
    }
    /// <summary>
    /// Excel Importer 사용 위해 자료형 변경한 클래스.
    /// 실제 게임 내에서는 MonsterForm으로 변환하여 사용.
    /// </summary>
    [System.Serializable]
    public class MonsterFormRaw
    {
        public int MonsterIndexNumber;
        public string MonsterType;
        public string MonsterName;
        public float MaxHp;
        public float MoveSpeed;
        public float Power;
        public float Defense;
        public int DropGroupIndexNumber;
    }

    public class MonsterForm
    {
        public int MonsterIndexNumber;
        public MonsterType MonsterType;
        public string MonsterName;
        public float MaxHp;
        public float MoveSpeed;
        public float Power;
        public float Defense;
        public int DropGroupIndexNumber;
    }
}