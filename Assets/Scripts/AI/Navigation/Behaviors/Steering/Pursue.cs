using UnityEngine;

namespace ZAI {

    public class Pursue:BaseBehavior {
        public SteeringCharacter targetCharacter;

        public bool withArrival;
        public float arrivalDistance;

        public Vector3 offset;

        Vector3 _targetPosition;
        Vector3 _seekDirn;
        Vector3 _pursueVector;
        float _relativeHeading, _relativePursueHeading;
        float _lookAheadMultiplier;
        float _dist;

        const float TOWARDS_CONE = 0.95f;

        public override void UpdateForFrame() {
            _targetPosition = targetCharacter.Position + targetCharacter.Side * offset.x + targetCharacter.Up * offset.y + targetCharacter.Heading * offset.z;
            _pursueVector = _targetPosition - _steeringCharacter.Position;
            // Dot: 1 -> ahead, -1 -> behind, 0 -> perpendicular
            _relativePursueHeading = Vector3.Dot(_pursueVector, _steeringCharacter.Heading);
            _relativeHeading = Vector3.Dot(targetCharacter.Heading, _steeringCharacter.Heading);

            bool isTargetAheadOfMe = _relativePursueHeading > 0;
            bool isTargetHeadingTowardsMe = _relativeHeading < -TOWARDS_CONE;

            // check if the target is ahead of you and heading towards you, then dont need prediction
            if(isTargetAheadOfMe && isTargetHeadingTowardsMe) {
                steeringForce = seek(_targetPosition);
            }
            else {
                // if not heading towards you, find a good pursue look ahead multiplier
                _lookAheadMultiplier = _pursueVector.magnitude/(targetCharacter.maxSpeed+_steeringCharacter.maxSpeed);
                _targetPosition += targetCharacter.Velocity * _lookAheadMultiplier;
                steeringForce = seek(_targetPosition);
            }
    	}
    	
    	Vector3 seek(Vector3 targetPos) {
            _seekDirn = targetPos - _steeringCharacter.Position;
            if(withArrival) {
                _dist = _seekDirn.magnitude;
            }
    		_seekDirn.Normalize();
            if(withArrival && _dist < arrivalDistance) {
                return((_seekDirn * _steeringCharacter.maxSpeed * _dist/arrivalDistance) - _steeringCharacter.Velocity);
            }
            else {
    		    return((_seekDirn * _steeringCharacter.maxSpeed) - _steeringCharacter.Velocity);
            }
    	}
    	
    	override public string GetName() {
    		return "Pursue";
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

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_targetPosition, 0.5f);
        }
    }

}