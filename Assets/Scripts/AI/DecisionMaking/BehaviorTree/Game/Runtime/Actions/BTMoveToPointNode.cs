using UnityEngine;

namespace ZAI {

    public class BTMoveToPointNode:BTActionNode {
        BTMoveToPointNodeData _btMoveToPointNodeData;
        Vector3 _dirn;

        public BTMoveToPointNode (BTMoveToPointNodeData btMoveToPointNodeData, BTree bTree, BTNode parent):base(btMoveToPointNodeData, bTree, parent) {
            _btMoveToPointNodeData = btMoveToPointNodeData;
        }

        protected override void Initialize () {
            base.Initialize ();

            _dirn = (_btMoveToPointNodeData.point - _bTree.AICharacter.Position).normalized;
        }

        protected override void Update () {
            float movementAmount = _btMoveToPointNodeData.speed * Time.deltaTime;
            if(Vector3.Distance(_bTree.AICharacter.Position, _btMoveToPointNodeData.point)<=movementAmount) {
                SetState(STATE.SUCCESS);
            }
            else {
                _bTree.AICharacter.SetPosition(_bTree.AICharacter.Position + _dirn * movementAmount);
            }
        }
    }

}