using UnityEngine;

namespace ZAI {

    public class BTWaitNode:BTActionNode {
        BTWaitNodeData _btWaitNodeData;
        float _startTime;

        public BTWaitNode (BTWaitNodeData btWaitNodeData, BTree bTree, BTNode parent):base(btWaitNodeData, bTree, parent) {
            _btWaitNodeData = btWaitNodeData;
        }

        protected override void Initialize () {
            base.Initialize ();

            _startTime = Time.time;
        }

        protected override void Update () {
            if(Time.time-_startTime > _btWaitNodeData.timeToWait) {
                SetState(STATE.SUCCESS);
            }
        }
    }
   
}