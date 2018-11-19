using UnityEngine;

namespace ZAI {

    public class Wander : BaseBehavior {
    	public float leadCircleOffset;
    	public float leadCircleRadius;
    	public float rateOfChangeAngle;
        public Vector3 wanderAxis = Vector3.one;
        public float recalculateFrequency = 0.25f;

        Vector3 _velDirn;
        float _currentWanderAngle;
        Vector3 _circleCenter;
        Vector3 _pointOnCircle;
        Vector3 _desiredWanderPoint;
        Vector3 _wanderDirn;
        float _recalculateTimer;

        public override void UpdateForFrame() {	
            // This is the circle infront of the ai character
            _circleCenter = _steeringCharacter.Position + _steeringCharacter.Velocity.normalized * leadCircleOffset;
            // This is the angle that is incremented that changes where the character turns towards
            _currentWanderAngle += Random.Range(-rateOfChangeAngle, rateOfChangeAngle) * Time.deltaTime;

            if(_recalculateTimer==0 || (Time.time - _recalculateTimer > recalculateFrequency)) {
                _pointOnCircle = new Vector3(leadCircleRadius * Mathf.Cos(_currentWanderAngle) * Mathf.Sin(_currentWanderAngle) * wanderAxis.x,
                    leadCircleRadius * Mathf.Sin(_currentWanderAngle) * Mathf.Sin(_currentWanderAngle) * wanderAxis.y, 
                    leadCircleRadius * Mathf.Cos(_currentWanderAngle) * wanderAxis.z);

                _recalculateTimer = Time.time;
            }

            _desiredWanderPoint = _circleCenter + _pointOnCircle;
            _wanderDirn = _desiredWanderPoint - _steeringCharacter.Position;
            steeringForce = _wanderDirn.normalized * _steeringCharacter.maxSpeed - _steeringCharacter.Velocity;
    	}
    	
    	override public string GetName() {
    		return "Wander";
    	}
    	
        override protected bool CanExecuteThisFrame() {
            return true;
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_circleCenter, leadCircleRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_desiredWanderPoint, 0.25f);
        }
    }

}
