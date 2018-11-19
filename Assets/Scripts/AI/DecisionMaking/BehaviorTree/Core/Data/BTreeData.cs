using UnityEngine;
using WyrmTale;

namespace ZAI {
    
    public class BTreeData {
        public string Id { get { return _id; } }
        string _id;
        public BTNodeData Root { get { return _btRootData; } }
        BTNodeData _btRootData;

        public BTreeData(JSON js) {
            _id = js.ToString("id");
            JSON bTreeJS = js.ToJSON("bTree");
            _btRootData = BTNodeData.CreateNodeData(bTreeJS);
        }
    }

}
