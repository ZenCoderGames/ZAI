using UnityEngine;
using System.Collections.Generic;

namespace ZAI {

    public class Hide:BaseBehavior {
        public SteeringCharacter targetCharacter;
        public float distanceFromEdge = 5;
        public float arrivalDistance;

        Vector3 _seekDirn;
        float _dist;
        List<Vector3> _hidePoints;

        override protected void Init() {
            base.Init();

            _hidePoints = new List<Vector3>();
        }

        override public void InitForFrame() {
            _hidePoints.Clear();

            // Find hide points
            List<Obstacle> obstacleList = SteeringManager.Instance.GetObstacles();
            int numObstacles = obstacleList.Count;
            for(int i=0; i<numObstacles; ++i) {
                Vector3 dirn = obstacleList[i].Position - targetCharacter.Position;
                _hidePoints.Add(obstacleList[i].Position + dirn.normalized * (obstacleList[i].radius + distanceFromEdge));
            }

            _hidePoints.Sort(SortByClosest);

            base.InitForFrame();
        }

        int SortByClosest(Vector3 p1, Vector3 p2) {
            float distP1 = (_steeringCharacter.Position - p1).sqrMagnitude;
            float distP2 = (_steeringCharacter.Position - p2).sqrMagnitude;
            return distP1.CompareTo(distP2);
        }

        public override void UpdateForFrame() {
            if(_hidePoints.Count>0) {
                // Arrival at the closest hide position
                Vector3 destinationHidePoint = _hidePoints[0];
                _seekDirn = destinationHidePoint - _steeringCharacter.Position;
                _dist = _seekDirn.magnitude;
                _seekDirn.Normalize();
                steeringForce = (_seekDirn * _steeringCharacter.maxSpeed * _dist/arrivalDistance) - _steeringCharacter.Velocity;
            }
        }

        override protected bool CanExecuteThisFrame() {
            if(targetCharacter==null)
                return false;

            return base.CanExecuteThisFrame();
        }

        override public bool IsWithinAOE() {
            return CheckAOE(_steeringCharacter.Position, targetCharacter.Position);
        }

        override public string GetName() {
            return "Hide";
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            if(aoe>0) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(_steeringCharacter.Position, aoe);
            }

            if(_hidePoints.Count>0) {
                Gizmos.color = Color.cyan;
                for(int i=0; i<_hidePoints.Count; ++i) {
                    Gizmos.DrawWireSphere(_hidePoints[i], 0.25f);
                }
            }
        }
    }

}
