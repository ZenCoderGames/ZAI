using WyrmTale;

namespace ZAI {

    public class BTSubTreeNodeData:BTDecoratorNodeData {
        public string treeId;

        public BTSubTreeNodeData(JSON js):base(js) {
            JSON paramsJS = js.ToJSON("nodeParams");
            treeId = paramsJS.ToString("treeId");
        }
    }

}