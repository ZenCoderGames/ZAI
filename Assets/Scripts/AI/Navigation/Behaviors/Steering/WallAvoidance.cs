using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZAI {

    public class WallAvoidance:BaseBehavior {
        public float feelerLength;
        public float feelerAngle;

        List<Wall> _listOfWalls;
        int _numWalls;

        float _adjustedFeelerLength;
        Vector3[] _feelers;
        Vector3 _startPos;

        override protected void Init() {
            base.Init();

            _feelers = new Vector3[3];
        }

        public override void InitForFrame()  {
            base.InitForFrame();

            // Scale the detection box based on velocity
            _adjustedFeelerLength = feelerLength + feelerLength/2 * _steeringCharacter.Velocity.magnitude/_steeringCharacter.maxSpeed;
            _startPos = _steeringCharacter.Position;
            _feelers[0] = _startPos + _steeringCharacter.Heading * _adjustedFeelerLength;
            if(SteeringManager.Instance.useXYPlane) {
                _feelers[1] = _startPos + ZUtils.RotateByAngleOnZAxis(_steeringCharacter.Heading, feelerAngle) * feelerLength;
                _feelers[2] = _startPos + ZUtils.RotateByAngleOnZAxis(_steeringCharacter.Heading, -feelerAngle) * feelerLength;
            }
            else {
                _feelers[1] = _startPos + ZUtils.RotateByAngleOnYAxis(_steeringCharacter.Heading, feelerAngle) * feelerLength;
                _feelers[2] = _startPos + ZUtils.RotateByAngleOnYAxis(_steeringCharacter.Heading, -feelerAngle) * feelerLength;
            }

            _listOfWalls = SteeringManager.Instance.GetWalls();
            _numWalls = _listOfWalls.Count;
        }

        public override void UpdateForFrame()  {
            bool hasCollided = false;
            for(int i=0; i<_feelers.Length; ++i) {
                float closestDistance = int.MaxValue;
                float overlapDistance = 0;
                Vector3 closestPoint = Vector3.zero;
                Wall closestWall = null;
                for(int j=0; j<_numWalls; ++j) {
                    Vector3 hitPoint = Vector3.zero;
                    float hitDistance = 0;
                    if(_listOfWalls[j].CheckLineIntersect(_startPos, _feelers[i], ref hitDistance, ref overlapDistance, ref hitPoint)) {
                        if(closestDistance > hitDistance) {
                            closestDistance = hitDistance;
                            closestPoint = hitPoint;
                            closestWall = _listOfWalls[j];
                        }
                    }
                }

                if(closestWall!=null) {
                    steeringForce += closestWall.avoidDirn * overlapDistance;
                    hasCollided = true;
                }
            }

            if(hasCollided) {
                steeringForce *= _steeringCharacter.maxSpeed;
            }
        }

        override public string GetName() {
            return "Wall Avoidance";
        }

        override protected bool CanExecuteThisFrame() {
            return true;
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_startPos, _feelers[0]);
            Gizmos.DrawLine(_startPos, _feelers[1]);
            Gizmos.DrawLine(_startPos, _feelers[2]);
        }
    }

}
