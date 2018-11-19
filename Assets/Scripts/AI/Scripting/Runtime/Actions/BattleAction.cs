using UnityEngine;

namespace ZAI {

    public class BattleAction {
        protected AICharacter _aiCharacter;

        protected BattleActionScript _actionScript;
        BattleActionData _actionData;

        protected int _frame, _framestoRun;
        bool _hasStarted, _hasExecuted;

        public bool Pause { get { return _actionData.pause; } }
        public bool Debug { get { return _actionData.debug; } }
        public bool HasStarted { get { return _hasStarted; } }
        public bool HasExecuted { get { return _hasExecuted; } }
        public int Frame { get { return _frame; } }
        public int EndFrame { get { return _frame + _framestoRun; } }

        public BattleAction(AICharacter aiChar, BattleActionScript actionScript, BattleActionData actionData) {
            _aiCharacter = aiChar;
            _actionScript = actionScript;
            _actionData = actionData;
        }

        virtual public void Init(BattleActionScript battleActionScript) {
            _frame = Mathf.FloorToInt(_actionData.frame * battleActionScript.GetTimeScale());
            _framestoRun = Mathf.FloorToInt(_actionData.framesToRun * battleActionScript.GetTimeScale());
        }

        public void Start() {
            _hasStarted = true;
            _hasExecuted = false;
            OnStart();
        }

        public void Update() {
            OnUpdate();
        }

        public void Interrupt() {
            OnInterrupted();
            ResetIfPossible ();
        }

        public void Reset() {
            if(_hasStarted) {
                ResetIfPossible();
            }
            _hasStarted = false;
            _hasExecuted = false;
        }

        public void End() {
            _hasExecuted = true;
            ResetIfPossible();
        }

        void ResetIfPossible() {
            if(_actionData.playTerminateLogic) {
                OnTerminate();
            }
        }

        virtual public void OnStart() {}
        virtual public void OnUpdate() {}
        // This is used to handle logic needed when an action is interrupted
        virtual protected void OnInterrupted() {}
        // This is used to handle logic needed when an action ends or is interrupted
        virtual protected void OnTerminate() {}
        virtual public void DrawDebug() {}
    }
}

