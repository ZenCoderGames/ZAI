using UnityEngine;
using System.Collections.Generic;

namespace ZAI {

    public class LeaderFollowing:BaseBehavior {
        public SteeringCharacter leader;
        public float neighborRadius;

        public float seperationWeight;
        public float arrivalDistance;
        public Vector3 offset;

        List<SteeringCharacter> _listOfNeighbors;
        Vector3 _dirn;
        int _totalCountOfNeighbors;
        Vector3 _seperationForce;
        Vector3 _leaderFuturePos;
        float _dist;

        override protected void Init() {
            base.Init();

            _listOfNeighbors = new List<SteeringCharacter>();
        }

        public override void InitForFrame () {      
            base.InitForFrame();

            _listOfNeighbors.Clear();
            SteeringManager.Instance.GetNeighbors(ref _listOfNeighbors, _steeringCharacter, neighborRadius);
            _totalCountOfNeighbors = _listOfNeighbors.Count;
        }

        public override void UpdateForFrame() {
            if(showDebug) {
                for(int i=0; i<_totalCountOfNeighbors; ++i) {
                    _listOfNeighbors[i].SetColor(Color.green);
                }
            }

            // Evasion from leader future pos if nearby
            _dirn = leader.Position + leader.Velocity - _steeringCharacter.Position;
            _dist = _dirn.magnitude;
            if(_dist < arrivalDistance) {
                _dirn.Normalize();
                // Check for head on collision
                float relativeHeading = Vector3.Dot(_dirn, _steeringCharacter.Heading); // 1 -> same direction, -1 -> opposite dirn

                bool isTargetHeadingTowardsMe = relativeHeading < 0.5f;

                if(isTargetHeadingTowardsMe) {
                    steeringForce += -_dirn + leader.Side;
                    steeringForce *= _steeringCharacter.maxSpeed * arrivalDistance/_dist;
                    steeringForce -= _steeringCharacter.Velocity;
                    return;
                }
            }

            // Seperation
            _seperationForce = Vector3.zero;
            for(int i=0; i<_totalCountOfNeighbors; ++i)
            {
                _dirn = _steeringCharacter.Position - _listOfNeighbors[i].Position;

                if(_dirn.sqrMagnitude==0) {
                    _dirn = _steeringCharacter.Side;
                }

                _seperationForce += 1/_dirn.magnitude * _dirn.normalized;
            }

            steeringForce += _seperationForce.normalized * seperationWeight;

            // Arrival 
            _dirn = leader.Position + leader.Side * offset.x + leader.Up * offset.y + leader.Heading * offset.z - _steeringCharacter.Position;
            steeringForce += (_dirn.normalized * _steeringCharacter.maxSpeed * _dirn.magnitude/arrivalDistance);

            steeringForce -= _steeringCharacter.Velocity;
        }

        override public string GetName() {
            return "Leader Following";
        }

        override protected bool CanExecuteThisFrame() {
            return true;
        }
    }

}
