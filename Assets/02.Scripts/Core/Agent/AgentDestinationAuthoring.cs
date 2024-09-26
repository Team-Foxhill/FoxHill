using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

namespace FoxHill.Core.Agent
{
    [RequireComponent(typeof(AgentAuthoring))]
    [DisallowMultipleComponent]
    internal class AgentDestinationAuthoring : MonoBehaviour
    {
        public Transform Target;
        public float Radius;
        public bool EveryFrame;

        private void Start()
        {
            AuthoringData();
        }

        private void OnEnable()
        {
            AuthoringData();
        }

        private void AuthoringData()
        {
            var agent = transform.GetComponent<AgentAuthoring>();
            var body = agent.EntityBody;
            body.Destination = Target.position;
            body.IsStopped = false;
            agent.EntityBody = body;
            EveryFrame = false;
        }

        private void Update()
        {
            if (!EveryFrame)
                return;
            AuthoringData();
        }
    }
}
