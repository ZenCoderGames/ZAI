using WyrmTale;
using System;

namespace ZAI {
    
    public static class BTGameActionData {

        public enum TYPE { INVALID, MOVE_TO_POINT, SCALE, WAIT }

        public static BTNodeData CreateActionNode(JSON js) {
            string nodeTypeStr = js.ToString("nodeType");
            TYPE nodeType = (TYPE)Enum.Parse(typeof(TYPE), nodeTypeStr);
            if(nodeType == TYPE.MOVE_TO_POINT) {
                return new BTMoveToPointNodeData(js);
            }
            else if(nodeType == TYPE.SCALE) {
                return new BTScaleNodeData(js);
            }
            else if(nodeType == TYPE.WAIT) {
                return new BTWaitNodeData(js);
            }
            else {
                UnityEngine.Debug.Log("Error: Invalid Action Node Type: " + nodeTypeStr);
            }
            return null;
        }
    }

}