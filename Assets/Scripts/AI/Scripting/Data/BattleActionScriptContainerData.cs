using System.Collections.Generic;
using WyrmTale;
using UnityEngine;

namespace ZAI {
    
    public class BattleActionScriptContainerData {

        public List<BattleActionScriptData> actionScriptDataList;
        public Dictionary<string, BattleActionScriptData> actionScriptDataDictionary;

        public BattleActionScriptContainerData(string fileName) {
            TextAsset fileAsset = Resources.Load(fileName) as TextAsset;
            JSON js = new JSON();
            js.serialized = fileAsset.text;

            JSON[] scriptsJS = js.ToArray<JSON>("scripts");

            actionScriptDataList = new List<BattleActionScriptData>();
            actionScriptDataDictionary = new Dictionary<string, BattleActionScriptData>();

            for (int i = 0; i < scriptsJS.Length; ++i) {
                JSON scriptJS = scriptsJS[i];
                BattleActionScriptData scriptData = new BattleActionScriptData(scriptJS);
                if (!actionScriptDataDictionary.ContainsKey(scriptData.id)) {
                    actionScriptDataList.Add(scriptData);
                    actionScriptDataDictionary.Add(scriptData.id, scriptData);
                }
                else {
                    Debug.Log("Error: Duplicate script Id " + scriptData.id);
                }
            }
        }

        public BattleActionScriptData GetScript(string id) {
            if (actionScriptDataDictionary.ContainsKey(id)) {
                return actionScriptDataDictionary[id];
            }
            return null;
        }

    }
}

