
namespace ZAI {

    public class BTNode {
        public enum STATE { UNINITIALIZED, RUNNING, PENDING_CHILD, SUCCESS, FAILURE, INTERRUPTED }
        STATE _currentState;

        protected BTree _bTree;
        protected BTNode _parent;
        protected bool _addToNextQueueIfRunning;
        protected bool _wasInterrupted;

        public event System.Action<BTNode, STATE> OnCompleted;
        public event System.Action<BTNode> OnAddedToNextStack;

        public BTNode (BTree bTree, BTNode parent) {
            _bTree = bTree;
            _parent = parent;
            _addToNextQueueIfRunning = true;

            SetState(STATE.UNINITIALIZED);
        }

        public static BTNode CreateNode(BTNodeData btNodeData, BTree bTree, BTNode parent) {
            if(btNodeData is BTActionNodeData) {
                return BTActionNode.CreateActionNode(btNodeData, bTree, parent);
            }
            else if(btNodeData is BTDecoratorNodeData) {
                return BTDecoratorNode.CreateDecoratorNode(btNodeData, bTree, parent);
            }
            else if(btNodeData is BTCompositeNodeData) {
                return BTCompositeNode.CreateCompositeNode(btNodeData, bTree, parent);
            }

            return null;
        }

        public STATE Tick() {
            if(IsUninitialized()) {
                Initialize();
            }
            if(IsRunning()) {
                Update();
            }
            if(_addToNextQueueIfRunning && IsRunning()) {
                _bTree.PushToNextStack(this);
                if(OnAddedToNextStack!=null) {
                    OnAddedToNextStack(this);
                }
            }

            return _currentState;
        }

        virtual protected void Initialize() {
            SetState(STATE.RUNNING);
        }

        virtual protected void Update() {}

        virtual protected void Terminate() {
            if(OnCompleted!=null) {
                OnCompleted(this, _currentState);
            }
            if(_parent==null) {
                ResetState();
            }
        }

        virtual public void Interrupt() {
            _wasInterrupted = true;
            SetState(STATE.FAILURE);
        }

        public void ResetState() {
            _wasInterrupted = false;
            SetState(STATE.UNINITIALIZED);
        }

        public void SetToReEvalThisFrame() {
            SetState(STATE.RUNNING);
        }

        public STATE GetState() {
            return _currentState;
        }

        protected void SetState(STATE newState) {
            _currentState = newState;

            if(IsSuccess() || IsFailure()) {
                Terminate();
            }
        }

        public bool IsUninitialized() {
            return _currentState == STATE.UNINITIALIZED;
        }

        public bool IsRunning() {
            return _currentState == STATE.RUNNING;
        }

        public bool HasAPendingChild() {
            return _currentState == STATE.PENDING_CHILD;
        }

        public bool IsDone() {
            return IsSuccess() || IsFailure();
        }

        public bool IsSuccess() {
            return _currentState == STATE.SUCCESS;
        }

        public bool IsFailure() {
            return _currentState == STATE.FAILURE;
        }
    }

}