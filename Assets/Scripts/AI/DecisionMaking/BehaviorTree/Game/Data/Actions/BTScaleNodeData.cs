using WyrmTale;
using System;
using UnityEngine;

namespace ZAI {

    public class BTScaleNodeData:BTActionNodeData {
        public Vector3 scaleAmount;
        public float scaleSpeed;

        public BTScaleNodeData(JSON js):base(js) {
            JSON paramsJS = js.ToJSON("nodeParams");
            float[] scaleValues = paramsJS.ToArray<float>("scaleAmount");
            if(scaleValues.Length==3) {
                scaleAmount = new Vector3(scaleValues[0], scaleValues[1], scaleValues[2]);
            }
            else {
                Debug.Log("Error: Scale needs a scaleAmount of Vector3 type");
            }

            scaleSpeed = paramsJS.ToFloat("scaleSpeed");
        }

    }

}