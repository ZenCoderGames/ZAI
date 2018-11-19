using UnityEngine;
using WyrmTale;
using System.Collections.Generic;

namespace ZAI {

    public class BTree : DecisionBrain {
        public string id;

        BTreeData _bTreeData;
        BTNode _btRoot;

        Stack<BTNode> _nextFrameStack, _currentFrameStack;
        BTNode _currentNode;
        bool _isInitialized;

        override public void Init(AICharacter aiCharacter) {
            base.Init(aiCharacter);

            if(BTreeManager.Instance.IsInitialized) {
                OnReadyToRegister();
            }
            else {
                BTreeManager.Instance.OnInitialized += OnReadyToRegister;
            }
        }

        void OnReadyToRegister() {
            BTreeData bTreeData = BTreeManager.Instance.GetBTreeData(id);
            if(bTreeData!=null) {
                Setup(bTreeData);
            }
        }

        void Setup(BTreeData bTreeData) {
            _bTreeData = bTreeData;
            _btRoot = BTNode.CreateNode(_bTreeData.Root, this, null);
            _currentFrameStack = new Stack<BTNode>();
            _nextFrameStack = new Stack<BTNode>();
            _isInitialized = true;
        }

        void Update() {
            if(!_isInitialized) {
                return;
            }

            PopulateCurrentStack();

            while(!IsStackEmpty(_currentFrameStack)) {
                _currentNode = _currentFrameStack.Pop();
                _currentNode.Tick();
            }
        }

        void StartTreeFromRoot() {
            PushToNextStack(_btRoot);
            Debug.Log("StartTreeFromRoot");
        }

        #region STACK OPERATIONS
        void PopulateCurrentStack() {
            if(IsStackEmpty(_nextFrameStack)) {
                StartTreeFromRoot();
            }

            while(!IsStackEmpty(_nextFrameStack)) {
                _currentFrameStack.Push(_nextFrameStack.Pop());
            }
        }

        public bool IsStackEmpty(Stack<BTNode> stack) {
            return stack.Count==0;
        }

        public void PushToNextStack(BTNode btNode) {
            _nextFrameStack.Push(btNode);
        }

        public BTNode PeekTopOfStack() {
            return _nextFrameStack.Peek();
        }

        public BTNode PopFromCurrentStack() {
            return _currentFrameStack.Pop();
        }

        public void InterjectNodeAfterTopInStack(BTNode btNode) {
            BTNode currentTop = _nextFrameStack.Pop();
            _nextFrameStack.Push(btNode);
            _nextFrameStack.Push(currentTop);
        }
        #endregion
    }

}
