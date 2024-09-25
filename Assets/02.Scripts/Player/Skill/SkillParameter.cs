using UnityEngine;

namespace FoxHill.Player.Skill
{
    public struct SkillParameter
    {
        public Vector2 Direction;
        public float Power;
        /// <summary>
        /// 특정 물체를 따라가는 형태의 스킬에 사용
        /// </summary>
        public Transform Transform; 
    }
}