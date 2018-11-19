using UnityEngine;

namespace ZAI {

    public class Interpose:BaseBehavior {
        public SteeringCharacter[] aiCharacterList;

        public float arrivalDistance;

        Vector3 _seekDirn;
        float _dist;
        Vector3 _posMidPoint;

        public override void UpdateForFrame() {
            _posMidPoint = Vector3.zero;
            int numChars = aiCharacterList.Length;
            for(int i=0; i<numChars; ++i) {
                _posMidPoint += aiCharacterList[i].Position;
            }
            _posMidPoint /= numChars;
            float timeAhead = Vector3.Distance(_steeringCharacter.Position, _posMidPoint) / _steeringCharacter.maxSpeed;
            _posMidPoint = Vector3.zero;
            for(int i=0; i<numChars; ++i) {
                _posMidPoint += aiCharacterList[i].Position + aiCharacterList[i].Velocity * timeAhead;
            }
            _posMidPoint /= numChars;

            // Arrival at the position
            _seekDirn = _posMidPoint - _steeringCharacter.Position;
            _dist = _seekDirn.magnitude;
            _seekDirn.Normalize();
            steeringForce = (_seekDirn * _steeringCharacter.maxSpeed * _dist/arrivalDistance) - _steeringCharacter.Velocity;
        }

        override public string GetName() {
            return "Interpose";
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_posMidPoint, 0.5f);
        }
    }

}
