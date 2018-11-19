
namespace ZAI {

    public class BTDecoratorNode:BTNode {
        protected BTNode _child;

        public BTDecoratorNode (BTDecoratorNodeData btDecoratorNodeData, BTree bTree, BTNode parent):base(bTree, parent) {
            if(btDecoratorNodeData.Child!=null) {
                _child = BTNode.CreateNode(btDecoratorNodeData.Child, bTree, this);
            }
        }

        public static BTNode CreateDecoratorNode(BTNodeData btNodeData, BTree bTree, BTNode parent) {
            if(btNodeData is BTRepeatNodeData) {
                return new BTRepeatNode(btNodeData as BTRepeatNodeData, bTree, parent);
            }
            else if(btNodeData is BTNegatorNodeData) {
                return new BTNegatorNode(btNodeData as BTNegatorNodeData, bTree, parent);
            }
            else if(btNodeData is BTSubTreeNodeData) {
                return new BTSubTreeNode(btNodeData as BTSubTreeNodeData, bTree, parent);
            }

            return null;
        }

        protected override void Terminate () {
            base.Terminate ();

            _child.ResetState();
        }
    }

}