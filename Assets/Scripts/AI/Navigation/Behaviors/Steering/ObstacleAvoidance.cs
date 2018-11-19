using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZAI {

    public class ObstacleAvoidance:BaseBehavior {
        public float detectionBoxWidth;
    	public float detectionBoxLength;
    	
        float _detectionBoxLength, _halfBoxWidth;
        float _dist;
        Vector3 _dirnVec;
        List<Obstacle> _listOfObstacles;

        public override void InitForFrame()  {
            base.InitForFrame();

            // Scale the detection box based on velocity
            float velocityMult = _steeringCharacter.Velocity.sqrMagnitude/(_steeringCharacter.maxSpeed*_steeringCharacter.maxSpeed);
            _detectionBoxLength = detectionBoxLength + velocityMult * detectionBoxLength;
            _listOfObstacles = SteeringManager.Instance.GetObstacles();
        }
    	
    	// Update is called once per frame
        public override void UpdateForFrame()  {
    		Vector3 obstacleLocalPos;
            _halfBoxWidth = detectionBoxWidth/2;

            float closestObstacleDistance = _detectionBoxLength * 2;
    		
    		Vector3 closestObstacleLocalPos = Vector3.zero;
    		Obstacle closestObstacle = null;

            int obstacleCount = _listOfObstacles.Count;
            for(int i=0; i<obstacleCount; ++i) {
                Obstacle obstacle = _listOfObstacles[i];
                _dirnVec = obstacle.Position - _steeringCharacter.Position;	
    			_dist = _dirnVec.magnitude - obstacle.radius;
                obstacleLocalPos = _steeringCharacter.AICharacter.GetLocalSpacePoint(obstacle.Position);

                if(showDebug) {
                    obstacle.SetDebugColor(Color.red);
                }
    			
                float obstacleForwardPos = obstacleLocalPos.z;
    			// Distance and Facing checks
                if(_dist<=_detectionBoxLength && obstacleForwardPos>=0) {
                    float collisionRadius = obstacle.radius + _halfBoxWidth;
    				
                    // Check for possible collision = true
    				if(obstacleLocalPos.x < collisionRadius) {
    					// Find the intersection points with the extended radius
                        // compute distance from t to circle intersection point
                        // dt = sqrt( R² - L²)
    					float sqrtPart = Mathf.Sqrt(collisionRadius*collisionRadius - obstacleLocalPos.x*obstacleLocalPos.x);
                        float intersectionPoint = obstacleForwardPos - sqrtPart;

    					// Special case: if intersection point is behind the character
    					if(intersectionPoint<=0) {
                            intersectionPoint = obstacleForwardPos + sqrtPart;
                        }

                        if(closestObstacle==null || intersectionPoint<closestObstacleDistance) {
                            closestObstacle = obstacle;
                            closestObstacleLocalPos = obstacleLocalPos;
                            closestObstacleDistance = intersectionPoint;
                        }
    				}
    			}
    		}
    		
    		// Calculate the steering force now that we have the closest obstacle
    		if(closestObstacle!=null) {
                int horFactor = closestObstacleLocalPos.x>0?-1:1;
                _dirnVec = horFactor * _steeringCharacter.Side;

                _dirnVec *= _steeringCharacter.maxSpeed;

                if(showDebug) {
                    closestObstacle.SetDebugColor(Color.cyan);
                }

                _dist = (closestObstacle.Position - _steeringCharacter.Position).sqrMagnitude;
                float forceMultiplier = (closestObstacle.radius*closestObstacle.radius)/_dist;

                steeringForce = _dirnVec.normalized * _steeringCharacter.maxSpeed * forceMultiplier - _steeringCharacter.Velocity;
    		}
    	}
    	
    	override public string GetName() {
    		return "Obstacle Avoidance";
    	}

        override protected bool CanExecuteThisFrame() {
            return true;
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            Gizmos.DrawLine(_steeringCharacter.Position-_halfBoxWidth*_steeringCharacter.Side,
                            _steeringCharacter.Position+_detectionBoxLength*_steeringCharacter.Heading-_halfBoxWidth*_steeringCharacter.Side);
            Gizmos.DrawLine(_steeringCharacter.Position, _steeringCharacter.Position+detectionBoxLength*_steeringCharacter.Heading);
            Gizmos.DrawLine(_steeringCharacter.Position+_halfBoxWidth*_steeringCharacter.Side,
                            _steeringCharacter.Position+_detectionBoxLength*_steeringCharacter.Heading+_halfBoxWidth*_steeringCharacter.Side);
        }
    }

}
