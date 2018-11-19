using UnityEngine;

namespace ZAI {

    public class BattleActionPlayAnim:BattleAction {

        BattleActionPlayAnimData _data;

        public BattleActionPlayAnim(AICharacter aiChar, BattleActionScript actionScript, BattleActionPlayAnimData actionPlayAnimData):base(aiChar, actionScript, actionPlayAnimData) {
            _data = actionPlayAnimData;
        }

        public override void OnStart() {
            _aiCharacter.PlayAnimation(_data.animName);
        }
    }
}

