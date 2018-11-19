using System;
using WyrmTale;

namespace ZAI {

    public class BTNodeData {
        public string Id { get { return _id; } }
        string _id;

        public enum TYPE { INVALID, ACTION, DECORATOR, COMPOSITE }

        public BTNodeData (JSON js) {
            _id = js.ToString("id");
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
            else {
                UnityEngine.Debug.Log("Error: Invalid Node Type: " + baseTypeStr);
            }
            return null;
        }
    }

}