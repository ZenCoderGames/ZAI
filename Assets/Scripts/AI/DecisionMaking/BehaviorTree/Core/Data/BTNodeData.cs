using System;
using WyrmTale;

namespace ZAI {

    public class BTNodeData {
        public string Id { get { return _id; } }
        string _id;

        public enum TYPE { INVALID, ROOT, ACTION, DECORATOR, COMPOSITE }
        public TYPE type;

        public BTNodeData (JSON js, TYPE nodeType) {
            _id = js.ToString("id");
            type = nodeType;
        }

        public static BTNodeData CreateNodeData(JSON js) {
            string baseTypeStr = js.ToString("baseType");
            TYPE baseType = (TYPE)Enum.Parse(typeof(TYPE), baseTypeStr);
            if(baseType == TYPE.ACTION) {
                return BTActionNodeData.CreateActionNodeData(js);
            }
            else if(baseType == TYPE.DECORATOR) {
                return BTDecoratorNodeData.CreateDecoratorNodeData(js);
            }
            else if(baseType == TYPE.COMPOSITE) {
                return BTCompositeNodeData.CreateCompositeNodeData(js);
            }
            else if(baseType == TYPE.ROOT) {
                BTNodeData rootNodeData = BTDecoratorNodeData.CreateRootNodeData(js);
                rootNodeData.type = TYPE.ROOT;
                return rootNodeData;
            }
            else {
                UnityEngine.Debug.Log("Error: Invalid Node Type: " + baseTypeStr);
            }
            return null;
        }
    }

}