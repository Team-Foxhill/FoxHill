using System;

namespace FoxHill.Tower
{
    /// <summary>
    /// Sheet를 ExcelImporter하여 얻은 MonsterForm 형태의 sheet를 게임 내에서 사용하기 위한 MonsterForm 형태로 Convert 하는 클래스.
    /// 현재는 string -> enum 하나만 변경하지만 추후 확장시 어디서 수정하는지 식별 용이하도록 별도 클래스로 관리.
    /// </summary>
    public static class TowerFormConverter
    {
        public static TowerForm Convert(TowerFormRaw sheet)
        {
            return new TowerForm
            {
                TowerIndexNumber = sheet.TowerIndexNumber,
                TowerType = ParseMonsterType(sheet.TowerType),
                TowerName = sheet.TowerName,
                MaxHp = sheet.MaxHp,
                AttackSpeed = sheet.AttackSpeed,
                AttackRange = sheet.AttackRange,
                Power = sheet.Power,
                Defense = sheet.Defense
            };
        }

        private static TowerType ParseMonsterType(string towerType)
        {
            if (Enum.TryParse(towerType, out TowerType towerEnumType) == false)
            {
                throw new ArgumentException($"Failed to parse towerType: {towerType}");
            }

            return towerEnumType;
        }


    }
}