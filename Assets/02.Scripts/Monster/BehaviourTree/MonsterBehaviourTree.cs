using FoxHill.Core;
using FoxHill.Core.Pause;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class MonsterBehaviourTree : MonoBehaviour, IPausable
    {
        public Blackboard Blackboard { get; private set; }
        public Stack<Node> NodeStack = new Stack<Node>(); // DFS (깊이우선탐색) 추적할 노드들을 쌓아놓기위함.
        public Root RootNode;
        public bool isRunning;
        private bool _isPaused;

        private void Update()
        {
            if (isRunning)
                return;

            isRunning = true;
            StartCoroutine(C_Tick());
        }

        private void OnDestroy()
        {
            PauseManager.Unregister(this);
        }

        /// <summary>
        /// 트리 탐색하는 단위
        /// </summary>
        IEnumerator C_Tick()
        {
            NodeStack.Push(RootNode);

            // 탐색할 노드가 남았으면 계속 탐색
            while (NodeStack.Count > 0)
            {
                if (Blackboard.IsDead)
                {
                    yield break;
                }
                while (_isPaused)
                {
                    yield return new WaitUntil(() => _isPaused == false);
                }
                // 가장 최근에 등록한 노드 탐색
                Node current = NodeStack.Pop();
                Result result = current.Invoke();

                // 현재 탐색중인 노드의 탐색이 끝나지 않았으므로 다음 프레임에 재탐색을 하기위해 스택에 되돌림
                if (result == Result.Running)
                {
                    NodeStack.Push(current);
                    yield return null;
                }
            }

            isRunning = false;
        }

        #region Builder
        protected Node _current;
        private Stack<Composite> _builingCompositeStack = new Stack<Composite>(); // 자식 빌드를 모두 완료하지 못한 스택을 차례로 쌓아둠

        public virtual MonsterBehaviourTree Build(SouthBossMonsterController controller)
        {
            Blackboard = new Blackboard(gameObject);
            Blackboard.Controller = controller;
            RootNode = new Root(this);
            _current = RootNode;
            PauseManager.Register(this);
            return this;
        }

        public MonsterBehaviourTree Selector()
        {
            Composite composite = new Selector(this);
            Attach(_current, composite);
            return this;
        }

        public MonsterBehaviourTree Sequence()
        {
            Composite composite = new Sequence(this);
            Attach(_current, composite);
            return this;
        }

        public MonsterBehaviourTree Parallel(int successCount)
        {
            Composite composite = new Parallel(this, successCount);
            Attach(_current, composite);
            return this;
        }

        public MonsterBehaviourTree Decorator(Func<bool> condition)
        {
            Node node = new Decorator(this, condition);
            Attach(_current, node);
            return this;
        }

        public MonsterBehaviourTree Execution(Func<Result> execute)
        {
            Node node = new Execution(this, execute);
            Attach(_current, node);
            return this;
        }

        public MonsterBehaviourTree Seek(MonsterBehaviourTree tree, float maxDistanceFromOrigin, Vector2 checkDirection, float radius, float angle, LayerMask targetLayer, float chaseDistance, float stoppingDistance, float moveStartDistance, float moveSpeed, float directionUpdateInterval)
        {
            Node node = new Seek(this, maxDistanceFromOrigin, checkDirection, radius, angle, targetLayer, chaseDistance, stoppingDistance, moveStartDistance, moveSpeed, directionUpdateInterval);
            Attach(_current, node);
            return this;
        }

        public MonsterBehaviourTree FinishCurrentComposite()
        {
            if (_builingCompositeStack.Count > 0)
                _builingCompositeStack.Pop();
            else
                throw new Exception("Theres no composite to build..");

            if (_builingCompositeStack.Count > 0)
                _current = _builingCompositeStack.Peek();

            return this;
        }

        protected void Attach(Node parent, Node child)
        {
            if (parent is IParent)
                ((IParent)parent).Attach(child);
            else
                throw new System.Exception($"Can't attach child at {_current} node.");

            if (child is IParent)
            {
                if (child is Composite)
                    _builingCompositeStack.Push((Composite)child);

                _current = child;
            }
            else
            {
                _current = _builingCompositeStack.Count > 0 ? _builingCompositeStack.Peek() : null;
            }
        }
        #endregion

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }
    }
}