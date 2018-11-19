using UnityEngine;

namespace ZAI {

    [RequireComponent(typeof(AICharacter))]
    public class DecisionBrain:MonoBehaviour {
        public AICharacter AICharacter { get { return _aiCharacter; } }
        protected AICharacter _aiCharacter;

        virtual public void Init(AICharacter aiCharacter) {
            _aiCharacter = aiCharacter;
        }
    }

}