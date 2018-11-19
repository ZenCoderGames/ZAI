using UnityEngine;

namespace ZAI {
    
    public class ScriptController:MonoBehaviour {
        BattleActionScriptContainer _scriptContainer;

        public string scriptContainerPath;

        public void Init(AICharacter aiCharacter) {
            BattleActionScriptContainerData scriptContainerData = new BattleActionScriptContainerData(scriptContainerPath);
            _scriptContainer = new BattleActionScriptContainer(aiCharacter, scriptContainerData);
        }

        public void PlayScript(string scriptId, System.Action onScriptCompletedFunc = null) {
            _scriptContainer.PlayScript(scriptId, onScriptCompletedFunc);
        }

        public void Tick(float deltaTime) {
            _scriptContainer.Update(deltaTime);
        }

        [ContextMenu("TestScript")]
        public void PlayTestScript() {
            PlayScript("SIMPLE_TEST");
        }
    }

}