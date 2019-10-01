using WyrmTale;
using System.Collections.Generic;
using System;

namespace ZAI {

    public class BTCompositeNodeData:BTNodeData {

        public enum COMPOSITE_TYPE { INVALID, SEQUENCER, SELECTOR, PARALLEL, DYNAMIC_SELECTOR }

        public List<BTNodeData> Children { get { return _children; } }
        List<BTNodeData> _children;

        public BTCompositeNodeData(JSON js):base(js, TYPE.COMPOSITE) {
            _children = new List<BTNodeData>();

            JSON paramsJS = js.ToJSON("nodeParams");
            JSON[] childrenJS = paramsJS.ToArray<JSON>("children");
            for(int i=0; i<childrenJS.Length; ++i) {
                JSON nodeJS = childrenJS[i];
                BTNodeData childNode = BTNodeData.CreateNodeData(nodeJS);
                if(childNode!=null) {
                    AddChild(childNode);
                }
            }
        }

        public static BTNodeData CreateCompositeNodeData(JSON js) {
            string nodeTypeStr = js.ToString("nodeType");
            COMPOSITE_TYPE nodeType = (COMPOSITE_TYPE)Enum.Parse(typeof(COMPOSITE_TYPE), nodeTypeStr);
            if(nodeType == COMPOSITE_TYPE.SEQUENCER) {
                return new BTSequenceNodeData(js);
            }
            else if(nodeType == COMPOSITE_TYPE.SELECTOR) {
                return new BTSelectorNodeData(js);
            }
            else if(nodeType == COMPOSITE_TYPE.PARALLEL) {
                return new BTParallelNodeData(js);
            }
            else if(nodeType == COMPOSITE_TYPE.DYNAMIC_SELECTOR) {
                return new BTDynamicSelectorNodeData(js);
            }
            else {
                UnityEngine.Debug.Log("Error: Invalid Decorator Node Type: " + nodeTypeStr);
            }
            return null;
        }

        public void AddChild(BTNodeData btNode) {
            _children.Add(btNode);
        }
    }

}