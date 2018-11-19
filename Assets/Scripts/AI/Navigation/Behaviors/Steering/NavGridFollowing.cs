using UnityEngine;

namespace ZAI {

    public class NavGridFollowing:BaseBehavior {
        public GameObject target;
        public Vector3 targetPos;
        public float arrivalDistance;

        NavGridPath _navGridPath;
        float _dist;
        Vector3 _seekDirn;
        Vector3 _nextTargetPos;
        bool _isDestinationPos;

        override public void InitForFrame() {
            if(_navGridPath==null) {
                NavGrid navGrid = SteeringManager.Instance.GetNavGrid();
                if(navGrid!=null) {
                    _navGridPath = new NavGridPath(navGrid, _steeringCharacter);
                }
            }
            else {
                if(target!=null) {
                    targetPos = target.transform.position;
                }
                _nextTargetPos = _navGridPath.GetNextPos(targetPos, ref _isDestinationPos);
            }

            base.InitForFrame();
        }

        public override void UpdateForFrame() {
            _seekDirn = _nextTargetPos - _steeringCharacter.Position;
            if(_isDestinationPos) {
                _dist = _seekDirn.magnitude;
            }
            _seekDirn.Normalize();
            if(_isDestinationPos && _dist < arrivalDistance) {
                steeringForce = (_seekDirn * _steeringCharacter.maxSpeed * _dist/arrivalDistance) - _steeringCharacter.Velocity;
            }
            else {
                steeringForce = (_seekDirn * _steeringCharacter.maxSpeed) - _steeringCharacter.Velocity;
            }
        }

        override protected bool CanExecuteThisFrame() {
            return _navGridPath!=null;
        }

        override public string GetName() {
            return "NavGridFollowing";
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            if(_navGridPath!=null) {
                _navGridPath.DebugDrawOnRun();
            }
        }
    }

}
