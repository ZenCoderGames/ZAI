
namespace ZAI {

    public class BTNegatorNode:BTDecoratorNode {

        public BTNegatorNode (BTNegatorNodeData btNegatorNodeData, BTree bTree, BTNode parent):base(btNegatorNodeData, bTree, parent) {
            
        }

        protected override void Update () {
            SetState(STATE.PENDING_CHILD);
            _child.OnCompleted += OnChildCompleted;
            _child.Tick();
        }

        void OnChildCompleted(BTNode childNode, STATE completionState) {
            childNode.OnCompleted -= OnChildCompleted;
            if(completionState == STATE.SUCCESS) {
                SetState(STATE.FAILURE);
            }
            else if(completionState == STATE.FAILURE) {
                SetState(STATE.SUCCESS);
            }
        }
    }

}