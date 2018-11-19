using UnityEngine;
using System.Collections;

namespace ZAI {

    public class Evade:BaseBehavior {
        public SteeringCharacter targetCharacter;
        Vector3 _fleeDirn;
        Vector3 _evadeVector;
    	float _lookAheadMultiplier;
    	
    	public override void UpdateForFrame () {
            _evadeVector = _steeringCharacter.Position - targetCharacter.Position;
            _lookAheadMultiplier = _evadeVector.magnitude/(targetCharacter.maxSpeed+_steeringCharacter.maxSpeed);     
            steeringForce = flee(targetCharacter.Position + targetCharacter.Velocity * _lookAheadMultiplier);
    	}
    	
    	Vector3 flee(Vector3 targetPos) {
            _fleeDirn = _steeringCharacter.Position - targetPos;
    		_fleeDirn.Normalize();
    		return((_fleeDirn * _steeringCharacter.maxSpeed) - _steeringCharacter.Velocity);
    	}
    	
    	override public string GetName() {
    		return "Evade";
    	}

        override protected bool CanExecuteThisFrame() {
            if(targetCharacter==null)
                return false;

            return base.CanExecuteThisFrame();
        }
    	
    	override public bool IsWithinAOE() {
            return CheckAOE(_steeringCharacter.Position, targetCharacter.Position);
    	}

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            if(aoe>0) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(targetCharacter.Position, aoe);
            }
        }
    }

}
