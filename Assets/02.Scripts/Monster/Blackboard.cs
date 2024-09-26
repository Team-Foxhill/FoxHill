using ProjectDawn.Navigation.Hybrid;
using UnityEngine;
using UnityEngine.AI;

namespace FoxHill.Monster.AI
{
    /// <summary>
    /// 각각의 노드가 서로의 데이터를 공유할 수 있는 칠판 역할을 하는 공간.
    /// </summary>
    public class Blackboard
    {
        public Blackboard(GameObject owner)
        {
            transform = owner.transform;
            agent = owner.GetComponent<AgentAuthoring>();
                }

        // 소유자.
        public Transform transform { get; set; }
        public AgentAuthoring agent { get; set; }
        // 목표물.
        public Transform target { get; set; }
    }
}
