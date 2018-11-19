
namespace ZAI {

    public class BTParallelNode:BTCompositeNode {
        int _numChildren;
        int _numSuccess, _numFailure;

        public BTParallelNode (BTParallelNodeData btParallelNodeData, BTree bTree, BTNode parent):base(btParallelNodeData, bTree, parent) {

        }

        protected override void Initialize () {
            base.Initialize ();

            _numChildren = _children.Length;
        }

        protected override void Update () {
            SetState(STATE.PENDING_CHILD);
            BTNode currentChild = null;
            for(int i=0; i<_numChildren; ++i) {
                currentChild = _children[i];
                currentChild.OnCompleted += OnChildCompleted;
                currentChild.Tick();
            }
        }

        void OnChildCompleted(BTNode childNode, STATE completionState) {
            childNode.OnCompleted -= OnChildCompleted;
            if(completionState == STATE.SUCCESS) {
                _numSuccess++;
            }
            else if(completionState == STATE.FAILURE) {
                _numFailure++;
            }

            if(_numSuccess==_numChildren) {
                SetState(STATE.SUCCESS);
            }
            else if(_numFailure==_numChildren) {
                SetState(STATE.FAILURE);
            }
            else if(_numSuccess+_numFailure==_numChildren) {
                SetState(STATE.FAILURE);
            }
        }
    }

}