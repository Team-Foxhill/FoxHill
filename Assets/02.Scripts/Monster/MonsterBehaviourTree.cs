using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class MonsterBehaviourTree : MonoBehaviour
    {
        public Blackboard blackboard { get => _blackboard; set {
                if (_blackboard != null)
                    throw new InvalidOperationException("Blackboard can only be set once.");
                _blackboard = value ?? throw new ArgumentNullException(nameof(value)); // 처음 한 번만 외부에서 셋할 수 있음.
            } }
        public Stack<Node> stack = new Stack<Node>(); // 후입선출 방식을 통해 깊이 우선 탐색 방식을 구현.
        public Root root;
        public bool isRunning;
        private Blackboard _blackboard;


        private void Update()
        {
            if (isRunning) // 동작중일 경우.
                return; // 이미 코루틴이 시작된 것이므로 반환.

            isRunning = true;
            StartCoroutine(C_Tick());
        }

        /// <summary>
        /// 탐색을 실행하는 코루틴.
        /// </summary>
        /// <returns></returns>
        private IEnumerator C_Tick()
        {
            while (stack.Count > 0) // 탐색할 노드가 남아있는 경우에,
            {
                Node current = stack.Pop(); // 스택에서 가장 최근에 추가된 노드를 탐색할 노드로 설정.
                Result result = current.Invoke(); // 현재 탐색한 노드의 결과 확인.
                // 만일 탐색이 진행중인 경우,
                if (result == Result.Running)
                {
                    stack.Push(current); // 현재 선택된 노드를 스택에 돌려놓음.
                    yield return null;
                }
            }
            isRunning = false;
        }
    }
}