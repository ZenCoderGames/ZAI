
namespace ZAI {

    public class BTDynamicSelectorNode:BTCompositeNode {
        int _numChildren;
        int _prevEvaluatingChildIdx;
        BTNode _storedBlockingChild;
        BTNode _blockingNode;

        public BTDynamicSelectorNode (BTDynamicSelectorNodeData btDynamicSelectorNodeData, BTree bTree, BTNode parent):base(btDynamicSelectorNodeData, bTree, parent) {
            _addToNextQueueIfRunning = false;
        }

        protected override void Initialize () {
            base.Initialize ();

            ResetStoredBlockingChild();
            _numChildren = _children.Length;
        }

        void ResetStoredBlockingChild() {
            if(_storedBlockingChild!=null) {
                _blockingNode.OnAddedToNextStack -= OnBlockingNodeAddedToNextStack;
                _storedBlockingChild.OnCompleted -= OnStoredChildCompleted;
            }
            _prevEvaluatingChildIdx = -1;
            _storedBlockingChild = null;
            _blockingNode = null;
        }

        protected override void Update () {
            UpdateFromChild(0);
        }

        void UpdateFromChild(int childIdx) {
            int i = childIdx;
            for(; i<_numChildren; ++i) {
                bool isBlockingChild = (_prevEvaluatingChildIdx==i);
                BTNode currentChild = _children[i];
                if(!isBlockingChild) {
                    if(currentChild.IsDone()) {
                        currentChild.SetToReEvalThisFrame();
                    }
                    currentChild.Tick();
                }
                if(currentChild.IsSuccess()) {
                    if(_prevEvaluatingChildIdx>i) {
                        _storedBlockingChild.Interrupt();
                        _bTree.PopFromCurrentStack();
                    }
                    else if(isBlockingChild) {
                        ResetStoredBlockingChild();
                    }
                    SetState(STATE.SUCCESS);
                    return;
                }
                else if(currentChild.IsFailure()) {
                    if(isBlockingChild) {
                        ResetStoredBlockingChild();
                    }
                    continue;
                }
                else if(currentChild.IsRunning() || currentChild.HasAPendingChild()) {
                    if(_storedBlockingChild==null) {
                        _prevEvaluatingChildIdx = i;
                        _storedBlockingChild = currentChild;
                        _bTree.InterjectNodeAfterTopInStack(this);
                        _storedBlockingChild.OnCompleted += OnStoredChildCompleted;
                        _blockingNode = _bTree.PeekTopOfStack();
                        _blockingNode.OnAddedToNextStack += OnBlockingNodeAddedToNextStack;
                    }
                    return;
                }
            }

            if(i==_numChildren) {
                SetState(STATE.FAILURE);
            }
        }

        void OnBlockingNodeAddedToNextStack(BTNode btNode) {
            _bTree.InterjectNodeAfterTopInStack(this);
        }

        void OnStoredChildCompleted(BTNode btNode, STATE completionState) {
            UpdateFromChild(_prevEvaluatingChildIdx);
        }
    }

}