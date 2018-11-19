using UnityEngine;

namespace ZAI {

    public class AICharacter:MonoBehaviour {
        DecisionBrain _decisionBrain;
        SteeringCharacter _steeringCharacter;
        ScriptController _scriptController;

        Transform _transform;
        Animator _animator;

        public Vector3 Position     { get { return _transform.position; } }
        public Quaternion Rotation  { get { return _transform.rotation; } }
        public Vector3 Forward      { get { return _transform.forward; } }
        public Vector3 Right        { get { return _transform.right; } }
        public Vector3 Up           { get { return _transform.up; } }
        public Vector3 LocalScale   { get { return _transform.localScale; } }

        void Awake() {
            _decisionBrain = GetComponent<DecisionBrain>();
            _steeringCharacter = GetComponent<SteeringCharacter>();
            _scriptController = GetComponent<ScriptController>();

            _transform = this.transform;
            _animator = GetComponent<Animator>();
        }

        void Start() {
            if(_decisionBrain!=null) {
                _decisionBrain.Init(this);
            }
            if(_steeringCharacter!=null) {
                _steeringCharacter.Init(this);
            }
            if(_scriptController!=null) {
                _scriptController.Init(this);
            }
        }

        void Update() {
            if(_scriptController!=null) {
                _scriptController.Tick(Time.deltaTime);
            }
        }

        public void SetPosition(Vector3 position) {
            _transform.position = position;
        }

        public void SetRotation(Quaternion rotation) {
            _transform.rotation = rotation;
        }

        public Vector3 GetLocalSpacePoint(Vector3 pos) {
            return _transform.InverseTransformPoint(pos);
        }

        public Vector3 GetWorldSpacePoint(Vector3 pos) {
            return _transform.TransformPoint(pos);
        }

        public void SetLocalScale(Vector3 scale) {
            _transform.localScale = scale;
        }

        public void PlayScript(string scriptId) {
            if(_scriptController!=null) {
                _scriptController.PlayScript(scriptId);
            }
        }

        public void PlayAnimation(string animId) {
            if(_animator!=null) {
                _animator.Play(animId);
            }
        }
    }

}