using UnityEngine;

namespace ZAI {

    public class FlowFieldFollowing:BaseBehavior {
        public Vector3 targetPos;
        public float arrivalDistance;
     
        FlowField _flowField;
        float _dist;
        Vector3 _seekDirn;
        bool _useArrival;

        override public void InitForFrame() {
            if(_flowField!=null) {
                if(_flowField.IsValid()) {
                    targetPos = _flowField.GetFuturePosition(_steeringCharacter.Position, targetPos, ref _useArrival);
                }
            }
            else {
                _flowField = SteeringManager.Instance.GetFlowField();
            }

            base.InitForFrame();
        }

        public override void UpdateForFrame() {
            _seekDirn = targetPos - _steeringCharacter.Position;
            if(_useArrival) {
                _dist = _seekDirn.magnitude;
            }
            _seekDirn.Normalize();
            if(_useArrival && _dist < arrivalDistance) {
                steeringForce = (_seekDirn * _steeringCharacter.maxSpeed * _dist/arrivalDistance) - _steeringCharacter.Velocity;
            }
            else {
                steeringForce = (_seekDirn * _steeringCharacter.maxSpeed) - _steeringCharacter.Velocity;
            }
        }

        override protected bool CanExecuteThisFrame() {
            return _flowField!=null && _flowField.IsValid();
        }

        override public string GetName() {
            return "FlowFieldFollowing";
        }
    }

}
