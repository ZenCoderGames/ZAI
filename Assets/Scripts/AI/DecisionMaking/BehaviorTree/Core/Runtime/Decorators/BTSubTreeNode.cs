
namespace ZAI {

    public class BTSubTreeNode:BTDecoratorNode {
        BTSubTreeNodeData _btSubTreeNodeData;

        public BTSubTreeNode (BTSubTreeNodeData btSubTreeNodeData, BTree bTree, BTNode parent):base(btSubTreeNodeData, bTree, parent) {
            _btSubTreeNodeData = btSubTreeNodeData;
            BTreeData btTreeData = BTreeManager.Instance.GetBTreeData(_btSubTreeNodeData.treeId);
            if(btTreeData!=null) {
                _child = BTNode.CreateNode(btTreeData.Root, bTree, this);
            }
            else {
                UnityEngine.Debug.Log("Error: Invalid Subtree id: " + _btSubTreeNodeData.treeId);
            }
        }

        protected override void Update () {
            SetState(STATE.PENDING_CHILD);
            _child.OnCompleted += OnChildCompleted;
            _child.Tick();
        }

        void OnChildCompleted(BTNode childNode, STATE completionState) {
            childNode.OnCompleted -= OnChildCompleted;
            if(completionState == STATE.SUCCESS) {
                SetState(STATE.SUCCESS);
            }
            else if(completionState == STATE.FAILURE) {
                SetState(STATE.FAILURE);
            }
        }
    }

}