
using UnityEngine;

namespace ZAI {

    [RequireComponent(typeof(SteeringCharacter))]
    public abstract class BaseBehavior:MonoBehaviour {
        public float aoe; // A sphere around the AI object - 0 -> means not taken into consideration
    	public float weight;
        public int priority;
        public float probability;
        public bool isPaused;

        [HideInInspector]
        public Vector3 steeringForce;
        public bool showDebug;

        protected SteeringCharacter _steeringCharacter;
        protected bool _canUpdateThisFrame;
    	
        void Start() {
            Init();
        }

        virtual protected void Init() {
    		_steeringCharacter = GetComponent<SteeringCharacter>();
            _steeringCharacter.AddBehavior(this);
    	}

    	virtual public void InitForFrame() {
            steeringForce = Vector3.zero;
            _canUpdateThisFrame = CanExecuteThisFrame();
        }

    	abstract public void UpdateForFrame();

        public bool CanUpdateThisFrame() {
            return _canUpdateThisFrame && !isPaused;
        }

        virtual protected bool CanExecuteThisFrame() {
            return IsWithinAOE();
        }
    	
    	abstract public string GetName();

        virtual public bool IsWithinAOE() { return aoe==0; }

        protected bool CheckAOE(Vector3 myPos, Vector3 targetPos) {
            if(aoe==0) return true;
            if((myPos-targetPos).sqrMagnitude<=(aoe*aoe)) return true;
            return false;
        }

        public void ApplySteeringScaleFactor(float val) { 
            steeringForce *= val;
        }

        void OnDrawGizmos() {
            if(showDebug) {
                DrawDebugAlways();
                if(Application.isPlaying) {
                    DebugDrawOnRun();
                }
            }
        }

        virtual protected void DebugDrawOnRun() {
            if(_steeringCharacter==null)
                return;

            Gizmos.color = Color.green;

            if(steeringForce.sqrMagnitude==0)
                return;

            float lengthMultiplier = steeringForce.magnitude;
            if(lengthMultiplier < 2)
                lengthMultiplier = 5;

            Gizmos.DrawLine(_steeringCharacter.Position, _steeringCharacter.Position + steeringForce.normalized * lengthMultiplier);
        }

        virtual protected void DrawDebugAlways() {}
    }

}

