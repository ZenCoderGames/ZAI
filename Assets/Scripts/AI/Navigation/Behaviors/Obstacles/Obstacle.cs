using UnityEngine;

namespace ZAI {

    public class Obstacle:MonoBehaviour {
    	public float radius;

        public Vector3 Position { get { return _transform.position; } }

        Transform _transform;

        void Awake() {
            _transform = this.transform;
        }
    	
        void Start() {
            if(SteeringManager.Instance.IsInitialized) {
                Init();
            }
            else {
                SteeringManager.Instance.OnInitialized += Init;
            }
        }

    	void Init() {
            SteeringManager.Instance.AddObstacle(this);
    	}

        public void CleanUp() {
            if(SteeringManager.Instance)
                SteeringManager.Instance.RemoveObstacle(this);
        }

        void OnDestroy() {
            CleanUp();
        }
    	
        #region DEBUG
        public bool showDebugGizmos;
        Color _defaultDebugColor = Color.red;
        Color _currentDebugColor = Color.red;

        void OnDrawGizmos() {
            if(showDebugGizmos) {
                Gizmos.color = _currentDebugColor;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }

        public void ResetDebugColor() {
            _currentDebugColor = _defaultDebugColor;
        }

        public void SetDebugColor(Color color) {
            _currentDebugColor = color;
        }
        #endregion
    }

}