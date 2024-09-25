using UnityEngine;

namespace FoxHill.Player.Skill
{
    public class SkillModel
    {
        public bool IsValid { get; private set; }
        public SkillType Type { get; private set; }
        public float Power { get; private set; }
        public float Cooldown { get; private set; }
        public float Duration { get; private set; }
        public float Speed { get; private set; }

        public SkillModel(SkillData data) 
        { 
            IsValid = false;
            Type = data.Type;
            Power = data.Power;
            Cooldown = data.Cooldown;
            Duration = data.Duration;
            Speed = data.Speed;
        }
    }
}
