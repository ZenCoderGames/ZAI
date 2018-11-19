using WyrmTale;
using System;
using UnityEngine;

namespace ZAI {

    public class BTWaitNodeData:BTActionNodeData {
        public float timeToWait;

        public BTWaitNodeData(JSON js):base(js) {
            JSON paramsJS = js.ToJSON("nodeParams");
            timeToWait = paramsJS.ToFloat("timeToWait");
        }

    }

}