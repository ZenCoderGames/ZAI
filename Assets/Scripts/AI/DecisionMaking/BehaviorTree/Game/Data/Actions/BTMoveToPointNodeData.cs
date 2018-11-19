using WyrmTale;
using System;
using UnityEngine;

namespace ZAI {

    public class BTMoveToPointNodeData:BTActionNodeData {
        public Vector3 point;
        public float speed;

        public BTMoveToPointNodeData(JSON js):base(js) {
            JSON paramsJS = js.ToJSON("nodeParams");
            float[] posValues = paramsJS.ToArray<float>("pointPosition");
            if(posValues.Length==3) {
                point = new Vector3(posValues[0], posValues[1], posValues[2]);
            }
            else {
                Debug.Log("Error: MoveToPointNode needs a pointPosition of Vector3 type");
            }

            speed = paramsJS.ToFloat("speed");
        }

    }

}