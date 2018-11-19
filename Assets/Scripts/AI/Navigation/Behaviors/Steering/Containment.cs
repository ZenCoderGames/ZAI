using UnityEngine;

namespace ZAI {

    public class Containment : BaseBehavior {
        public Vector3 startPosition;

        Vector3 _towardsCenter;

        public override void UpdateForFrame() { 
            _towardsCenter = startPosition - _steeringCharacter.Position;
            float percentFromEdge = _towardsCenter.sqrMagnitude/(aoe*aoe);

            steeringForce = _towardsCenter.normalized * percentFromEdge * _steeringCharacter.maxSpeed;
        }

        override public string GetName() {
            return "Containment";
        }

        override protected bool CanExecuteThisFrame() {
            return aoe>0;
        }

        override protected void DrawDebugAlways() {
            if(aoe>0) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(startPosition, aoe);
            }
        }
    }

}