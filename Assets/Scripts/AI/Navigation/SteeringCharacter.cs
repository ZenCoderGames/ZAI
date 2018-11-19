using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZAI {

    [RequireComponent(typeof(AICharacter))]
    public class SteeringCharacter:MonoBehaviour {
        public float radius = 0.5f;
    	public float mass = 5;
    	public float maxForce = 5;
        public float maxSpeed = 5;
        public float maxTurnAngle = 5;
        public bool useNonPenetrationLogic;

        public enum FORCE_COMBINATION {PRIORITIZED_WEIGHTED_SUM, PRIORITIZED_SKIP, PRIORITIZED_DITHER, WEIGTHED_SUM}
        public FORCE_COMBINATION forceCombinationMethod;

        public int groupId;

        public bool isPaused;
        public bool showDebugGizmos;

        public Vector3 Position { get { return _position; } }
        public Vector3 Heading { get { return _heading; } }
        public Vector3 Side { get { return _side; } }
        public Vector3 Up { get { return _up; } }
        public Vector3 Velocity { get { return _velocity; } }
        public Quaternion Rotation { get { return _rotation; } }

        public AICharacter AICharacter { get { return _aiCharacter; } }
        protected AICharacter _aiCharacter;

        Vector3 _heading;
        Vector3 _side;
        Vector3 _up;
        Vector3 _steeringForce;
        Vector3 _acceleration;
        Vector3 _velocity;
        Vector3 _position;
        Quaternion _rotation;
        List<BaseBehavior> _currentBehaviors;
        int _totalActiveBehaviors;
        float _weightScaleFactor;

        // Optimizations
        float _maxForceSquared, _maxSpeedSquared;
        float _velocityMagnitude;
        Vector3 _velocityDirn;

        const float ZERO_VELOCITY_APPROX = 0.0001f;

    	void Awake() {
    		_currentBehaviors = new List<BaseBehavior>();

    		_velocity = Vector3.zero;
            _maxForceSquared = maxForce * maxForce;
            _maxSpeedSquared = maxSpeed * maxSpeed;
    	}
    	
        public void Init(AICharacter aiCharacter) {
            _aiCharacter = aiCharacter;

            if(SteeringManager.Instance.IsInitialized) {
                OnReadyToRegister();
            }
            else {
                SteeringManager.Instance.OnInitialized += OnReadyToRegister;
            }
    	}

        void OnReadyToRegister() {
            SteeringManager.Instance.AddCharacter(this);
        }

        public void CleanUp() {
            if(SteeringManager.Instance)
                SteeringManager.Instance.RemoveCharacter(this);
        }

        void OnDestroy() {
            CleanUp();
        }

        public void AddBehavior(BaseBehavior b) {
            _currentBehaviors.Add(b);
            _currentBehaviors.Sort(SortBehavior);
        }

        public void RemoveBehavior(BaseBehavior b) {
            _currentBehaviors.Remove(b);
        }

        int SortBehavior(BaseBehavior b1, BaseBehavior b2) {
            return b2.priority - b1.priority;
        }

    	public void InitForTick() {
            if(isPaused)
                return;

            _position = _aiCharacter.Position;
            _rotation = _aiCharacter.Rotation;

            _heading = _aiCharacter.Forward;
            _side = _aiCharacter.Right;
            _up = _aiCharacter.Up;

    		BaseBehavior b = null;
    		for(int i=0; i<_currentBehaviors.Count; ++i) {
    			b = _currentBehaviors[i];
                if (!b.isPaused) {
    				b.InitForFrame();
    			}
    		}
    	}	

    	public void UpdateForTick() {
            if(isPaused)
                return;

    		if (_currentBehaviors.Count>0) {
    			_totalActiveBehaviors = 0;
    			BaseBehavior b = null;
                // Get total active behaviors
    			for(int i=0; i<_currentBehaviors.Count; ++i) {
    				b = _currentBehaviors[i];
                    if (b.CanUpdateThisFrame()) {
    					_totalActiveBehaviors++;
    				}
    			}
    			
    			if (_totalActiveBehaviors>0) {
    				// Calculate the weight scale factor based on total active
    				_weightScaleFactor = _totalActiveBehaviors>0?_currentBehaviors.Count/_totalActiveBehaviors:1;
    				_steeringForce = Vector3.zero;

    				for(int i=0; i<_currentBehaviors.Count; ++i) {
    					b = _currentBehaviors[i];
                        if (b.CanUpdateThisFrame()) {
                            // Calculate the steering force by each behavior
                            b.UpdateForFrame();
                            bool hasSteeringForce = b.steeringForce.sqrMagnitude>0;
                            if(hasSteeringForce) {
                                if(UsePrioritizedWeightSum()) {
                                    AccumulateForceForWeightedPriority(b);
                                }
                                else if(UsePriortizedSkip()) {
                                    _steeringForce += b.steeringForce;
                                    break;
                                }
                                else if(UsePriortizedDither()) {
                                    float randomChance = Random.Range(0.0f, 1.0f);
                                    if(randomChance<b.probability) {
                                        _steeringForce += b.steeringForce;
                                        break;
                                    }
                                }
                                else if(UseWeightedSum()) {
                                    _steeringForce += b.steeringForce * b.weight;
                                }
                            }
    					}
    				}
                    // Steering Force
                    ClampSteeringForce();
                    // Acceleration
                    _acceleration = _steeringForce/mass;
                    // Velocity
                    _velocity += _acceleration;
                    // Position
                    if(_velocity.sqrMagnitude>ZERO_VELOCITY_APPROX) {
                        ClampVelocity();
                        _position += _velocity * Time.deltaTime;
                        if(SteeringManager.Instance.useXYPlane) {
                            _rotation = Quaternion.LookRotation(_velocity, -Vector3.forward);
                        }
                        else {
                            _rotation = Quaternion.LookRotation(_velocity);
                        }
                    }
    			}
    		}
    	}

        public void LateUpdateForTick() {
            if(isPaused)
                return;
            
            if(useNonPenetrationLogic) {
                SteeringManager.Instance.DoNonPenetrationLogic(this);
            }
        }

        public void EndOfFrameUpdate() {
            if(isPaused)
                return;

            _aiCharacter.SetPosition(_position);
            _aiCharacter.SetRotation(_rotation);
        }
    	
        #region FORCE_COMBINATION
        // Add force only till max force
        // Return true if there is still capacity to add
        // Return false if max force has been reached
        bool AccumulateForceForWeightedPriority(BaseBehavior b)
    	{
            Vector3 force = b.steeringForce;
            // Apply scaling weight factor with individual weight
            b.ApplySteeringScaleFactor(_weightScaleFactor * b.weight);

    		float combinedForceMagnitude = _steeringForce.magnitude;
    		
    		float remainingForceMagnitude = maxForce - combinedForceMagnitude;
    		
    		if(remainingForceMagnitude<=0) return false;
    		
    		float currentForceMagnitude = force.magnitude;
    				
    		if(currentForceMagnitude <= remainingForceMagnitude) {
    			_steeringForce += force;
    		}
    		else {
    			force.Normalize();
    			Vector3 newForce = force * remainingForceMagnitude;
    			_steeringForce += newForce;
                return false;       
    		}
    		
    		return true;
    	}
        #endregion

    	#region HELPERS
        public void ForceSetPosition(Vector3 pos) {
            _position = pos;
        }

        void ClampSteeringForce() {
            if(_steeringForce.sqrMagnitude > _maxForceSquared) {
                _steeringForce = _steeringForce.normalized * maxForce;
            }
        }

    	void ClampVelocity() {
            Vector3 velocityDirn = _velocity.normalized;
            if(_velocity.sqrMagnitude > _maxSpeedSquared) {
                _velocity = velocityDirn * maxSpeed;
                _velocityMagnitude = maxSpeed;
    		}
            else {
                _velocityMagnitude = _velocity.magnitude;
            }

            _velocity = Vector3.RotateTowards(_heading, velocityDirn, maxTurnAngle * Time.deltaTime, 0) * _velocityMagnitude;
    	}
    	
    	public void SetColor(Color c) {
    		GetComponent<Renderer>().material.color = c;	
    	}
    	#endregion

        #region FORCE_COMBINATIONS
        public bool UsePriortizedSkip() {
            return forceCombinationMethod == FORCE_COMBINATION.PRIORITIZED_SKIP;
        }

        public bool UsePrioritizedWeightSum() {
            return forceCombinationMethod == FORCE_COMBINATION.PRIORITIZED_WEIGHTED_SUM;
        }

        public bool UsePriortizedDither() {
            return forceCombinationMethod == FORCE_COMBINATION.PRIORITIZED_DITHER;
        }

        public bool UseWeightedSum() {
            return forceCombinationMethod == FORCE_COMBINATION.WEIGTHED_SUM;
        }
        #endregion

        void OnDrawGizmos() {
            if(Application.isPlaying && showDebugGizmos) {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_position, _position + _steeringForce);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_position, _position + _velocity);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(_position, radius);
            }
        }
    }

}