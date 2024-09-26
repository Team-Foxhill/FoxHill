using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FoxHill.Monster.AI
{
    public class MonsterBehaviourTreeBuilder : IBuilder
    {
        private MonsterBehaviourTree _tree;
        private Node _currentNode;
        private Stack<Composite> _buildingCompositeStack = new Stack<Composite>(); // 자식 빌드를 모두 완료하지 못한 Composite만 차례대로 스택에 쌓아 둠. -> 완료되면 Pop으로 제거, Peek로 상위 Composite 읽어오도록 제작.

        public MonsterBehaviourTreeBuilder()
        {
            _tree = new MonsterBehaviourTree();
            _currentNode = _tree.root = new Root(_tree);
            _buildingCompositeStack.Push((Composite) _currentNode);
        }

        public MonsterBehaviourTree Build()
        {
            if (_buildingCompositeStack.Count > 0)
            {
                _tree.blackboard = new Blackboard(_tree.gameObject);
                _tree.root = new Root(_tree);
                _currentNode = _tree.root;
            }
            return _tree;
        }

        public IBuilder Selector()
        {
            Composite selector = new Selector(_tree); // 현재 행동 트리에 대한 새로운 Selector 생성.
            Attach(_currentNode, selector); // 생성된 Selector를 현재 IParent의 자식으로 추가.
            return this; // 메서드 체이닝을 위해 빌더 인스턴스 반환.
        }

        public IBuilder Parallel(int successCount)
        {
            Composite parallel = new Parallel(_tree, successCount); // 현재 행동 트리에 대한 새로운 Parallel 생성.
            Attach(_currentNode, parallel); // 생성된 Parallel을 현재 IParent의 자식으로 추가.
            return this; // 메서드 체이닝을 위해 빌더 인스턴스 반환.
        }

        public IBuilder Sequence()
        {
            Composite sequence = new Sequence(_tree); // 현재 행동 트리에 대한 새로운 Sequence 생성.
            Attach(_currentNode, sequence); // 생성된 Sequence를 현재 IParent의 자식으로 추가.
            return this; // 메서드 체이닝을 위해 빌더 인스턴스 반환.
        }

        public IBuilder Execution(Func<Result> execute)
        {
            Node node = new Execution(_tree, execute); // 현재 행동 트리에 대한 새로운 Execution 생성.
            Attach(_currentNode, node); // 생성된 Execution을 현재 IParent의 자식으로 추가.
            return this; // 메서드 체이닝을 위해 빌더 인스턴스 반환.

        }

        public IBuilder Decorator(Func<bool> condition)
        {
            Node node = new Decorator(_tree, condition); // 현재 행동 트리에 대한 새로운 Decorator 생성.
            Attach(_currentNode, node); // 생성된 Decorator를 현재 IParent의 자식으로 추가.
            return this; // 메서드 체이닝을 위해 빌더 인스턴스 반환.
        }

        public IBuilder Seek(float radius, float angle, int targetmask, float MaxDistance)
        {
            Node node = new Seek(_tree, radius, angle, targetmask, MaxDistance); // 현재 행동 트리에 대한 새로운 Seek 생성.
            Attach(_currentNode, node); // 생성된 Seek를 현재 IParent의 자식으로 추가.
            return this; // 메서드 체이닝을 위해 빌더 인스턴스 반환.
        }

        /// <summary>
        /// 현재 작업 중인 Composite의 구성을 완료하고 상위 레벨로 돌아감.
        /// </summary>
        /// <returns>현재 <see cref="IBuilder"/> 인스턴스.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IBuilder FinishCurrentComposite()
        {
            if (_buildingCompositeStack.Count > 0)
                _buildingCompositeStack.Pop(); // 현재 Composite를 스택에서 제거하여 완료 처리.
            else
                throw new InvalidOperationException("현재 완료할 Composite가 없습니다. FinishCurrentComposite를 호출하기 전에 Composite를 먼저 생성해야 합니다."); // 스택이 비어있으면 예외 발생.

            if (_buildingCompositeStack.Count > 0)
                _currentNode = _buildingCompositeStack.Peek(); // 현재 작업 Node를 상위 레벨의 Composite로 변경하기 위해 스택 최상위 Node를 제거하지 않고 값을 읽어옴.

            return this; // 현재 IBuilder 인스턴스를 반환하여 메서드 체이닝 구현.
        }

        /// <summary>
        /// 입력받은 대로 부모와 자식 Node/Composite를 설정.
        /// </summary>
        /// <param name="parent">부모 Node/Composite</param>
        /// <param name="child">자식 Node/Composite</param>
        /// <exception cref="System.Exception"></exception>
        private void Attach(Node parent, Node child)
        {
            if (parent is IParent)
                ((IParent)parent).Attach(child); // 부모가 IParent를 구현하고 있다면 자식 Node 추가.
            else
                throw new System.Exception($"{_currentNode} Node에는 자식을 붙여넣을 수 없습니다."); // 부모가 IParent가 아니면 예외 발생.

            if (child is IParent)
            {
                if (child is Composite)
                    _buildingCompositeStack.Push((Composite)child); // 자식이 Composite라면 스택에 추가하여 하위 레벨 추가.(여러 노드를 추가할 수 있도록.)

                _currentNode = child; // 현재 작업 Node를 새로 추가된 자식으로 변경.
            }
            else
            {
                _currentNode = _buildingCompositeStack.Count > 0 ? _buildingCompositeStack.Peek() : null; // 자식이 일반 Node면 현재 작업 Node를 상위 Composite로 설정하거나 null로 설정.
            }
        }
    }
}
