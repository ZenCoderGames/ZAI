using WyrmTale;
using System;

namespace ZAI {

    public class BTActionNodeData:BTNodeData {

        // If needed, can add some core one's here
        public enum ACTION_TYPE { INVALID }

        public BTActionNodeData(JSON js):base(js) {
        }

        public static BTNodeData CreateActionNodeData(JSON js) {
            return BTGameActionData.CreateActionNode(js);
        }
    }

}