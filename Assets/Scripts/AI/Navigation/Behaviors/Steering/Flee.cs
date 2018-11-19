using UnityEngine;

namespace ZAI {

    public class Flee:BaseBehavior {
    	public Vector3 targetPos;
        public GameObject fleeTarget;
        Transform _fleeTransform;

        Vector3 _fleeDirn;
    	
        override protected void Init() {
            base.Init();

            if(fleeTarget!=null) {
                _fleeTransform = fleeTarget.transform;
                targetPos = _fleeTransform.position;
            }
        }

        override public void InitForFrame() {
            if(_fleeTransform!=null) {
                targetPos = _fleeTransform.position;    
            }

            base.InitForFrame();
        }

        public override void UpdateForFrame() {
            _fleeDirn = _steeringCharacter.Position - targetPos;
            _fleeDirn.Normalize();      
            steeringForce = (_fleeDirn * _steeringCharacter.maxSpeed) - _steeringCharacter.Velocity;
    	}
    	
    	override public bool IsWithinAOE() {
            return CheckAOE(_steeringCharacter.Position, targetPos);
    	}

        override public string GetName() {
            return "Flee";
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            if(aoe>0) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(targetPos, aoe);
            }
        }
    }

}
