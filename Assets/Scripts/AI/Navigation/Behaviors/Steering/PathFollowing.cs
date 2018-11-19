using UnityEngine;
using System.Collections.Generic;

namespace ZAI {

    public class PathFollowing:BaseBehavior {
    	public List<Vector3> listOfPathPoints;
        public bool useArrival;
        public float arrivalDistance;
    	public bool loop;
    	public float distanceToSwitch;
    	
        int _currentPathNum;
        Vector3 _targetPos;
        float _dist;
        Vector3 _seekDirn;
        bool _isLastPoint;
    		
        override protected void Init() {
            base.Init();

    		_currentPathNum = 0;
    	}

        public override void InitForFrame() {
            _targetPos = listOfPathPoints[_currentPathNum];
            _seekDirn = _targetPos - _steeringCharacter.Position;
            _dist = _seekDirn.magnitude;
            _isLastPoint = (_currentPathNum==listOfPathPoints.Count-1);

            base.InitForFrame();
        }
    	
        public override void UpdateForFrame() {
            if(!loop && _isLastPoint && useArrival && _dist<arrivalDistance) {
                steeringForce = (_seekDirn.normalized * _steeringCharacter.maxSpeed * _dist/arrivalDistance) - _steeringCharacter.Velocity;
            }
            else if(_dist>distanceToSwitch){
                steeringForce = (_seekDirn.normalized * _steeringCharacter.maxSpeed) - _steeringCharacter.Velocity;
    		}
    		else {
                if(!_isLastPoint) {
    			    _currentPathNum++;
                }
                else if(loop){
        			_currentPathNum = 0;
                }
    		}
    	}
    	
    	override public string GetName() {
    		return "Path Following";
    	}
    	
        override protected bool CanExecuteThisFrame() {
            return listOfPathPoints.Count>0;
        }

        void OnDrawGizmos() {
            if(showDebug) {
                Gizmos.color = Color.green;
                for(int i=0; i<listOfPathPoints.Count; ++i) {
                    Gizmos.DrawSphere(listOfPathPoints[i], 0.5f);
                }
            }
        }
    }

}
