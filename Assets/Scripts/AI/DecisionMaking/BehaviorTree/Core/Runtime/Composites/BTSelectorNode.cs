
namespace ZAI {

    public class BTSelectorNode:BTCompositeNode {
        int _numChildren;
        int _currentChildIdx;

        public BTSelectorNode (BTSelectorNodeData btSelectorNodeData, BTree bTree, BTNode parent):base(btSelectorNodeData, bTree, parent) {

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
                SetState(STATE.SUCCESS);
            }
            else if(completionState == STATE.FAILURE) {
                _currentChildIdx++;
                if(_currentChildIdx==_numChildren) {
                    SetState(STATE.FAILURE);
                }
                else {
                    Update();
                }
            }
        }
    }

}