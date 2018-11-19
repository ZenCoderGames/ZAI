using System.Collections.Generic;
using WyrmTale;
using UnityEngine;

namespace ZAI {

    public class BattleActionScriptContainer {
        Dictionary<string, BattleActionScript> _dictOfBattleActionScripts;
        BattleActionScript _currentBattleActionScript;
        System.Action _scriptCompletedFunc;

        private bool _isPaused;

        public BattleActionScriptContainer(AICharacter aiChar, BattleActionScriptContainerData scriptContainerData) {
            _dictOfBattleActionScripts = new Dictionary<string, BattleActionScript>();
            for (int i = 0; i < scriptContainerData.actionScriptDataList.Count; ++i) {
                BattleActionScriptData scriptData = scriptContainerData.actionScriptDataList[i];
                BattleActionScript script = new BattleActionScript(aiChar, this, scriptData);
                _dictOfBattleActionScripts.Add(script.Id, script);
            }
        }

        public void PlayScript(string scriptId, System.Action scriptCompletedFunc = null) {
            if(_dictOfBattleActionScripts.ContainsKey(scriptId)) {
                _currentBattleActionScript = _dictOfBattleActionScripts[scriptId];
                _currentBattleActionScript.Start(scriptCompletedFunc);
            }
            else {
                Debug.Log("Error: Invalid script id: " + scriptId);
            }
        }

        public void Update(float deltaTime) {
            if (_currentBattleActionScript != null && !_currentBattleActionScript.IsCompleted) {
                _currentBattleActionScript.Update(deltaTime);
            }
        }

        public void Interrupt() {
            if (_currentBattleActionScript != null) {
                _currentBattleActionScript.Interrupt();
            }

            _currentBattleActionScript = null;  
        }
    }
}

