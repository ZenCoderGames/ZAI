using WyrmTale;
using UnityEngine;

namespace ZAI {
    
    public static class BattleActionTypes {
        public enum TYPE {
            INVALID, PLAY_ANIM
        }

        public static BattleActionData GetBattleActionData(BattleActionScriptData scriptData, JSON js, int actionIndex) {
            string actionTypeString = js.ToString("type");
            if (actionTypeString.StartsWith("#")) {
                // Ignore
                return null;
            }
            TYPE actionType = (TYPE)System.Enum.Parse(typeof(TYPE), actionTypeString);

            switch (actionType) {
                case BattleActionTypes.TYPE.PLAY_ANIM: return new BattleActionPlayAnimData(js);
            }

            Debug.LogWarning("Invalid BattleAction tag detected in script (" + scriptData.id + " action #" + (actionIndex + 1) + "): " + actionType);
            return null;
        }

        public static BattleAction GetBattleAction(AICharacter aiChar, BattleActionScript script, BattleActionData data) {
            switch (data.type) {
                case TYPE.PLAY_ANIM: return new BattleActionPlayAnim(aiChar, script, data as BattleActionPlayAnimData);
            }

            Debug.LogWarning("Invalid BattleActionData type detected: " + data.type);
            return null;
        }
    }
}
