using WyrmTale;
using System;

namespace ZAI {

    public static class BTGameActionNode {

        public static BTNode CreateActionNode(BTNodeData btNodeData, BTree bTree, BTNode parent) {
            if(btNodeData is BTMoveToPointNodeData) {
                return new BTMoveToPointNode(btNodeData as BTMoveToPointNodeData, bTree, parent);
            }
            else if(btNodeData is BTScaleNodeData) {
                return new BTScaleNode(btNodeData as BTScaleNodeData, bTree, parent);
            }
            else if(btNodeData is BTWaitNodeData) {
                return new BTWaitNode(btNodeData as BTWaitNodeData, bTree, parent);
            }

            return null;
        }
    }

}