using WyrmTale;

namespace ZAI {

    public class BTRepeatNodeData:BTDecoratorNodeData {
        public int numTimes;

        public BTRepeatNodeData(JSON js):base(js) {
            JSON paramsJS = js.ToJSON("nodeParams");
            numTimes = paramsJS.ToInt("numTimes");
        }
    }

}