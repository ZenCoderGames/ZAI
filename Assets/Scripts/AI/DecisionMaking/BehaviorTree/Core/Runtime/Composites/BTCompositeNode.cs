using System.Collections.Generic;

namespace ZAI {

    public class BTCompositeNode:BTNode {
        protected BTNode[] _children;

        public BTCompositeNode (BTCompositeNodeData btCompositeNodeData, BTree bTree, BTNode parent):base(bTree, parent) {
            List<BTNodeData> childrenData = btCompositeNodeData.Children;
            _children = new BTNode[childrenData.Count];
            for(int i=0; i<childrenData.Count; ++i) {
                _children[i] = BTNode.CreateNode(childrenData[i], bTree, this);
            }
        }

        public static BTNode CreateCompositeNode(BTNodeData btNodeData, BTree bTree, BTNode parent) {
            if(btNodeData is BTSequenceNodeData) {
                return new BTSequenceNode(btNodeData as BTSequenceNodeData, bTree, parent);
            }
            else if(btNodeData is BTSelectorNodeData) {
                return new BTSelectorNode(btNodeData as BTSelectorNodeData, bTree, parent);
            }
            else if(btNodeData is BTParallelNodeData) {
                return new BTParallelNode(btNodeData as BTParallelNodeData, bTree, parent);
            }
            else if(btNodeData is BTDynamicSelectorNodeData) {
                return new BTDynamicSelectorNode(btNodeData as BTDynamicSelectorNodeData, bTree, parent);
            }

            return null;
        }

        protected override void Terminate () {
            base.Terminate ();

            for(int i=0; i<_children.Length; ++i) {
                _children[i].ResetState();
            }
        }
    }

}