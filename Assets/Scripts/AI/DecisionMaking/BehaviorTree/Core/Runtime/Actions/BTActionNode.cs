
namespace ZAI {

    public class BTActionNode:BTNode {

        public BTActionNode (BTActionNodeData btActionNodeData, BTree bTree, BTNode parent):base(bTree, parent) {
            
        }

        public static BTNode CreateActionNode(BTNodeData btNodeData, BTree bTree, BTNode parent) {
            // If needed, can add more core actions here

            return BTGameActionNode.CreateActionNode(btNodeData, bTree, parent);
        }
    }

}