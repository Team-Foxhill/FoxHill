using UnityEngine;

namespace FoxHill.Player.Skill
{
    public enum SkillType
    {
        AzureDragon,
        VermilionBird,
        WhiteTiger,
        BlackTortoise
    }

    [CreateAssetMenu(fileName = "SkillData", menuName = "Data/Create SkillData")]
    public class SkillData : ScriptableObject
    {
        public SkillType Type;
        public float Power;
        public float Cooldown;
        public float Duration;
        public float Speed;
    }
}