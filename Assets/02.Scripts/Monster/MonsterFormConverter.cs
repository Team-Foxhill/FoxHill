using FoxHill.Core;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace FoxHill.Monster
{
    /// <summary>
    /// Sheet를 ExcelImporter하여 얻은 MonsterForm 형태의 sheet를 게임 내에서 사용하기 위한 MonsterForm 형태로 Convert 하는 클래스.
    /// 현재는 string -> enum 하나만 변경하지만 추후 확장시 어디서 수정하는지 식별 용이하도록 별도 클래스로 관리.
    /// </summary>
    public static class MonsterFormConverter
    {
        public static MonsterForm Convert(MonsterFormRaw sheet)
        {
            return new MonsterForm
            {
                MonsterIndexNumber = sheet.MonsterIndexNumber,
                MonsterName = sheet.MonsterName,
                MonsterType = ParseMonsterType(sheet.MonsterType),
                MaxHp = sheet.MaxHp,
                MoveSpeed = sheet.MoveSpeed,
                Power = sheet.Power,
                Defense = sheet.Defense,
                DropGroupIndexNumber = sheet.DropGroupIndexNumber
            };
        }

        private static MonsterType ParseMonsterType(string monsterType)
        {
            if (Enum.TryParse(monsterType, out MonsterType monsterEnumType) == false)
            {
                throw new ArgumentException($"Failed to parse MonsterType: {monsterType}");
            }

            return monsterEnumType;
        }


    }
}