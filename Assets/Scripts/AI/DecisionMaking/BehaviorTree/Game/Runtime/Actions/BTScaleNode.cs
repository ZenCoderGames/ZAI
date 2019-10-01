using UnityEngine;

namespace ZAI {

    public class BTScaleNode:BTActionNode {
        BTScaleNodeData _btScaleNodeData;

        public BTScaleNode (BTScaleNodeData btScaleNodeData, BTree bTree, BTNode parent):base(btScaleNodeData, bTree, parent) {
            _btScaleNodeData = btScaleNodeData;
        }

        protected override void Update () {
            float scaleAmount = _btScaleNodeData.scaleSpeed * Time.deltaTime;
            float distance = Vector3.Distance(_bTree.AICharacter.LocalScale, _btScaleNodeData.scaleAmount);
            if(Vector3.Distance(_bTree.AICharacter.LocalScale, _btScaleNodeData.scaleAmount)<=Mathf.Abs(scaleAmount*2)) {
                SetState(STATE.SUCCESS);
            }
            else {
                _bTree.AICharacter.SetLocalScale(_bTree.AICharacter.LocalScale + Vector3.one * scaleAmount);
            }
        }
    }

}