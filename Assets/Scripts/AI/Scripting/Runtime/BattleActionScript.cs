using System.Collections.Generic;
using WyrmTale;
using UnityEngine;

namespace ZAI {

    public class BattleActionScript {
        BattleActionScriptContainer _scriptContainer;
        BattleActionScriptData _actionScriptData;

        public string Id { get { return _actionScriptData.id; } }
        public bool HasStarted { get { return _hasStarted; } }
        public bool IsCompleted { get { return _hasCompleted; } }
        public int CurrentFrame { get { return _currentFrame; } }

        List<BattleAction> _listOfBattleActions;
        System.Action _onCompletedFunc;
        int  _currentFrame, _totalFrames, _realtimeGameFrame;
        float _currentFrameFloat;
        float _speed;
        bool _hasStarted, _hasCompleted;
        string _breakOutCurrentScript;

        public static float TARGET_FPS = 60;

        public BattleActionScript(AICharacter aiChar, BattleActionScriptContainer scriptContainer, BattleActionScriptData actionScriptData) {
            _scriptContainer = scriptContainer;
            _actionScriptData = actionScriptData;

            _speed = actionScriptData.speed;
            _listOfBattleActions = new List<BattleAction>();
            for (int i=0; i<actionScriptData.listOfActionData.Count; ++i) {
                BattleAction battleAction = BattleActionTypes.GetBattleAction(aiChar, this, actionScriptData.listOfActionData[i]);
                _listOfBattleActions.Add(battleAction);
            }
        }

        void Init() {
            _totalFrames = Mathf.FloorToInt(_actionScriptData.totalFrames * GetTimeScale());
            float currentFramePercent = (float)_currentFrame / (float)_totalFrames;
            _currentFrameFloat = (currentFramePercent * (float)_totalFrames);
            _currentFrame = Mathf.FloorToInt(_currentFrameFloat);

            for (int i=0; i<_listOfBattleActions.Count; ++i)
                _listOfBattleActions [i].Init (this);
        }

        public float GetTimeScale() {
            return Mathf.Round((1.0f / _speed) * 100.0f) / 100.0f;
        }

        void Reset() {
            BattleAction currentAction = null;
            int battleActionCount = _listOfBattleActions.Count;

            for (int i=0; i< battleActionCount; ++i) {
                currentAction = _listOfBattleActions[i];
                currentAction.Reset();
            }

            _hasCompleted = false;
            _hasStarted = false;
        }

        public void Start(System.Action scriptCompletedFunc=null) {
            Reset();
            _onCompletedFunc = scriptCompletedFunc;
            _hasStarted = true;
            _currentFrame = 0;
            _currentFrameFloat = 0;
            _realtimeGameFrame = 0;
            _breakOutCurrentScript = null;

            // update first frame immediately
            Update(0);
        }

        public void Update(float deltaTime) {
            if (_actionScriptData.totalFrames <= 0 || _hasCompleted) {
                return;
            }

            for(int i=0; i<_listOfBattleActions.Count; ++i) {
                BattleAction currentAction = _listOfBattleActions[i];

                // If its already executed, ignore this action
                if(currentAction.HasExecuted)
                    continue;

                // If action hasn't started and passes the conditions to execute
                if(!currentAction.HasStarted) {
                    bool startAction = _currentFrame >= currentAction.Frame;
                    if(startAction) {
                        currentAction.Start();

                        if (currentAction.Pause) {
                            Debug.Break();
                        }
                    }
                }

                // Shouldn't be an else as it could update to hasStarted this frame
                if(currentAction.HasStarted) {
                    currentAction.Update();
                    if (currentAction.Debug) {
                       currentAction.DrawDebug();
                    }

                    if(currentAction.EndFrame>0 && _currentFrame >= currentAction.EndFrame) {
                        currentAction.End();
                    }
                }
            }

            // used for branching out of the current timeline
            if (!string.IsNullOrEmpty(_breakOutCurrentScript)) {
                ResetAllActions();
                _scriptContainer.PlayScript (_breakOutCurrentScript, _onCompletedFunc);
                _breakOutCurrentScript = null;
                return;
            }

            if(_currentFrame >= _totalFrames && !_hasCompleted) {
                EndActionScript();
            }

            // Increment timeline frame (FPS independent)
            if (deltaTime > 0) {
                _currentFrameFloat += 1.0f / (((1.0f / deltaTime) / TARGET_FPS));
                _currentFrame = Mathf.FloorToInt(_currentFrameFloat);
            }

            // TODO: Investigate If frame rate is low, does it skip frames ?

            _realtimeGameFrame++;
        }

        void EndActionScript () {
            _hasCompleted = true;

            ResetAllActions();

            if (_onCompletedFunc != null)
                _onCompletedFunc();

            _breakOutCurrentScript = null;
        }

        void ResetAllActions() {
            for (int i=0; i<_listOfBattleActions.Count; ++i) {
                if (_listOfBattleActions[i].HasStarted) {
                    _listOfBattleActions[i].Reset();
                }
            }
        }

        public void Interrupt() {
            for (int i=0; i<_listOfBattleActions.Count; ++i) {
                if(_listOfBattleActions[i].HasStarted && !_listOfBattleActions[i].HasExecuted) {
                    _listOfBattleActions[i].Interrupt();
                }
            }
            Reset ();
        }
    }
}

