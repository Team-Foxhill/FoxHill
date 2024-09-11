namespace FoxHill.Player.Data
{
    public class PlayerStat // TODO : 인터페이스 적용
    {
        public float Hp;
        public float Exp;
        public float MoveSpeed;
        public float NormalAttackSpeed;
        public float NormalAttackDamage;
        public int Level;

        public void LoadData(PlayerData data)
        {
            Hp = data.Hp;
            Exp = data.Exp;
            MoveSpeed = data.MoveSpeed;
            NormalAttackSpeed = data.NormalAttackSpeed;
            NormalAttackDamage = data.NormalAttackDamage;
            Level = data.Level;
        }
    }
}