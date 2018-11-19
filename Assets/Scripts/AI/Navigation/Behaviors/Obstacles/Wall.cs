using UnityEngine;

namespace ZAI {

    public class Wall:MonoBehaviour {
        public float width, height;
        public Vector3 avoidDirn;

        public Vector3 Position { get { return _transform.position; } }

        Transform _transform;
        Vector3 _p2, _p3;

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
            if(avoidDirn.x!=0) {
                Vector3 heightDirn = Vector3.forward;
                if(SteeringManager.Instance.useXYPlane) {
                    heightDirn = Vector3.up;
                }
                    
                _p2 = _transform.position + (avoidDirn * width/2) - (heightDirn * height/2);
                _p3 = _transform.position + (avoidDirn * width/2) + (heightDirn * height/2);
            }
            else if(avoidDirn.z!=0 || avoidDirn.y!=0) {
                _p2 = _transform.position + (Vector3.right * width/2) + (avoidDirn * height/2);
                _p3 = _transform.position - (Vector3.right * width/2) + (avoidDirn * height/2);
            }
            SteeringManager.Instance.AddWall(this);
        }

        public void CleanUp() {
            if(SteeringManager.Instance)
                SteeringManager.Instance.RemoveWall(this);
        }

        void OnDestroy() {
            CleanUp();
        }

        public bool CheckLineIntersect(Vector3 p0, Vector3 p1, ref float distOnHit, ref float overlapDist, ref Vector3 hitPoint) {
            // Get A,B of first line - points : ps1 to pe1
            float A1 = p1.z - p0.z;
            float B1 = p0.x - p1.x;
            // Get A,B of second line - points : ps2 to pe2
            float A2 = _p3.z - _p2.z;
            float B2 = _p2.x - _p3.x;
            // Get delta and check if the lines are parallel
            float delta = A1*B2 - A2*B1;
            if(delta == 0) return false;
            // Get C of first and second lines
            float C2 = A2*_p2.x + B2*_p2.z;
            float C1 = A1*p0.x + B1*p0.z;

            //invert delta to make division cheaper
            float invdelta = 1/delta;

            Vector3 intersectionPoint = new Vector3( (B2*C1 - B1*C2)*invdelta, p0.y, (A1*C2 - A2*C1)*invdelta );
            float distFromInstersection = Vector3.Distance(intersectionPoint, p0);
            float lineLength = Vector3.Distance(p1, p0);

            if(distFromInstersection < lineLength) {
                hitPoint = intersectionPoint;
                distOnHit = distFromInstersection;
                overlapDist = lineLength - distFromInstersection;
                return true;
            }

            return false;
        }

        #region DEBUG
        public bool showDebugGizmos;
        Color _defaultDebugColor = Color.red;
        Color _currentDebugColor = Color.red;

        void OnDrawGizmos() {
            if(showDebugGizmos && SteeringManager.Instance!=null) {
                Gizmos.color = _currentDebugColor;
                if(SteeringManager.Instance.useXYPlane)
                    Gizmos.DrawCube(transform.position, new Vector3(width, height, 0));
                else
                    Gizmos.DrawCube(transform.position, new Vector3(width, 0, height));
                Gizmos.color = Color.green;
                ZUtils.DrawGizmoArrow(transform.position, avoidDirn * 5, SteeringManager.Instance.useXYPlane);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, 0.5f);
                Gizmos.DrawSphere(_p2, 0.5f);
                Gizmos.DrawSphere(_p3, 0.5f);
                Gizmos.DrawLine(_p2, _p3);
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