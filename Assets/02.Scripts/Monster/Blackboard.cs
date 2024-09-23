using ProjectDawn.Navigation;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 각각의 노드가 서로의 데이터를 공유할 수 있는 칠판 역할을 하는 공간.
    /// </summary>
    public class Blackboard
    {
        public Blackboard() {
                }

        // 소유자.
        public Transform transform { get; set; }
        public AgentAuthoring agentAuthoring { get; set; }

        // 목표물.
        public Transform target { get; set; }
    }
}
