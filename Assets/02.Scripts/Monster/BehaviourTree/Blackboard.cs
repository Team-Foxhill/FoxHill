using UnityEngine;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 각각의 노드가 서로의 데이터를 공유할 수 있는 칠판 역할을 하는 공간.
    /// </summary>
    public class Blackboard
    {
        public Blackboard(GameObject owner)
        {
            Transform = owner.transform;
        }

        public Transform Transform { get; set; } // 블랙보드 소유자의 트랜스폼.
        public Vector2 OriginPosition { get; set; } // 각 컨트롤러의 Awake에서 설정해줘야 함.
        public Transform Target { get; set; } // 목표물의 트랜스폼.
        public SouthBossMonsterController Controller { get; set; }
        public bool IsNextActionReady { get; set; }
        public bool IsDead { get; set; }
        public bool IsStunInvoked { get; set; }
        public bool IsStunRunning { get; set; }
    }
}
