using UnityEngine;

namespace ZAI {

    public class Seek:BaseBehavior {
    	public Vector3 targetPos;
    	public GameObject seekTarget;
        Transform _seekTransform;

    	public bool withArrival;
    	public float arrivalDistance;

        Vector3 _seekDirn;
        float _dist;

        override protected void Init() {
            base.Init();

            if(seekTarget!=null) {
                _seekTransform = seekTarget.transform;
            }
        }

        override public void InitForFrame() {
            if(_seekTransform!=null) {
                targetPos = _seekTransform.position;    
            }

            base.InitForFrame();
        }
    	
        public override void UpdateForFrame() {
            _seekDirn = targetPos - _steeringCharacter.Position;
            if(withArrival) {
                _dist = _seekDirn.magnitude;
            }
            _seekDirn.Normalize();
            if(withArrival && _dist < arrivalDistance) {
                steeringForce = (_seekDirn * _steeringCharacter.maxSpeed * _dist/arrivalDistance) - _steeringCharacter.Velocity;
            }
            else {
                steeringForce = (_seekDirn * _steeringCharacter.maxSpeed) - _steeringCharacter.Velocity;
            }
    	}
    	
    	override public bool IsWithinAOE() {
            return CheckAOE(_steeringCharacter.Position, targetPos);
    	}

        override public string GetName() {
            return "Seek";
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            if(withArrival) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(targetPos, arrivalDistance);
            }
        }
    }

}
