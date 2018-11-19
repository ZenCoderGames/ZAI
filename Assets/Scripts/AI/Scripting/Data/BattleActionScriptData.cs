using System.Collections.Generic;
using WyrmTale;

namespace ZAI {
    
    public class BattleActionScriptData {
        public string id;
        public int totalFrames;
        public float speed;
        public List<BattleActionData> listOfActionData;
        public Dictionary<string, BattleActionData> dictOfActionData;

        public BattleActionScriptData(JSON actionScriptJS) {
            id = actionScriptJS.ToString("id");
            totalFrames = actionScriptJS.ToInt("totalFrames");
            speed = actionScriptJS.ToFloat("speed", 1);

            listOfActionData = new List<BattleActionData>();
            dictOfActionData = new Dictionary<string, BattleActionData>();

            JSON[] actionsJS = actionScriptJS.ToArray<JSON>("actions");
            for(int i=0; i<actionsJS.Length; ++i) {
                CreateAction(actionsJS[i], i);
            }
        }

        void CreateAction(JSON js, int actionIndex, string id="") {
            BattleActionData actionData = BattleActionTypes.GetBattleActionData(this, js, actionIndex);

            if (actionData != null) {
                listOfActionData.Add(actionData);
            }
        }
    }
}