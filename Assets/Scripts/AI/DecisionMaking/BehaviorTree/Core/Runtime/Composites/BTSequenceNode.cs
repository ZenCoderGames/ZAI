
namespace ZAI {

    public class BTSequenceNode:BTCompositeNode {
        int _numChildren;
        int _currentChildIdx;

        public BTSequenceNode (BTSequenceNodeData btSequenceNodeData, BTree bTree, BTNode parent):base(btSequenceNodeData, bTree, parent) {
            
        }

        protected override void Initialize () {
            base.Initialize ();

            _currentChildIdx = 0;
            _numChildren = _children.Length;
        }

        protected override void Update () {
            BTNode currentChild = _children[_currentChildIdx];
            SetState(STATE.PENDING_CHILD);
            currentChild.OnCompleted += OnChildCompleted;
            currentChild.Tick();
        }

        void OnChildCompleted(BTNode childNode, STATE completionState) {
            childNode.OnCompleted -= OnChildCompleted;
            if(completionState == STATE.SUCCESS) {
                _currentChildIdx++;
                if(_currentChildIdx==_numChildren) {
                    SetState(STATE.SUCCESS);
                }
                else {
                    Update();
                }
            }
            else if(completionState == STATE.FAILURE) {
                SetState(STATE.FAILURE);
            }
        }
    }

}