using WyrmTale;
using System.Collections.Generic;
using System;

namespace ZAI {

    public class BTDecoratorNodeData:BTNodeData {

        public enum DECORATOR_TYPE { INVALID, REPEAT, NEGATOR, SUB_TREE }

        public BTNodeData Child { get { return _child; } }
        BTNodeData _child;

        public BTDecoratorNodeData(JSON js):base(js) {
            JSON paramsJS = js.ToJSON("nodeParams");
            if(paramsJS.Contains("child")) {
                JSON childNodeJS = paramsJS.ToJSON("child");
                BTNodeData childNode = BTNodeData.CreateNodeData(childNodeJS);
                if(childNode!=null) {
                    AddChild(childNode);
                }
            }
        }

        public static BTNodeData CreateDecoratorNodeData(JSON js) {
            string nodeTypeStr = js.ToString("nodeType");
            DECORATOR_TYPE nodeType = (DECORATOR_TYPE)Enum.Parse(typeof(DECORATOR_TYPE), nodeTypeStr);
            if(nodeType == DECORATOR_TYPE.REPEAT) {
                return new BTRepeatNodeData(js);
            }
            else if(nodeType == DECORATOR_TYPE.NEGATOR) {
                return new BTNegatorNodeData(js);
            }
            else if(nodeType == DECORATOR_TYPE.SUB_TREE) {
                return new BTSubTreeNodeData(js);
            }
            else {
                UnityEngine.Debug.Log("Error: Invalid Decorator Node Type: " + nodeTypeStr);
            }
            return null;
        }

        public void AddChild(BTNodeData btNode) {
            _child = btNode;
        }
    }

}