
namespace ZAI {

    public class BTRepeatNode:BTDecoratorNode {
        BTRepeatNodeData _btRepeatNodeData;
        int _currentCount;

        public BTRepeatNode (BTRepeatNodeData btRepeatNodeData, BTree bTree, BTNode parent):base(btRepeatNodeData, bTree, parent) {
            _btRepeatNodeData = btRepeatNodeData;
        }

        protected override void Initialize () {
            base.Initialize ();

            _currentCount = 0;
        }

        protected override void Update () {
            SetState(STATE.PENDING_CHILD);
            _child.OnCompleted += OnChildCompleted;
            _child.Tick();
        }

        void OnChildCompleted(BTNode childNode, STATE completionState) {
            childNode.OnCompleted -= OnChildCompleted;
            if(completionState == STATE.SUCCESS) {
                _currentCount++;
                if(_currentCount<_btRepeatNodeData.numTimes) {
                    _child.ResetState();
                    _child.OnCompleted += OnChildCompleted;
                    _bTree.PushToNextStack(_child);
                }
                else {
                    SetState(STATE.SUCCESS);
                }
            }
            else if(completionState == STATE.FAILURE) {
                SetState(STATE.FAILURE);
            }
        }
    }

}