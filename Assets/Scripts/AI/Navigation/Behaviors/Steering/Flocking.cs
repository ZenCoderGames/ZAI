using UnityEngine;
using System.Collections.Generic;

namespace ZAI {

    public class Flocking:BaseBehavior {
    	public float neighborRadius;
    	
        [Header ("Seperation")]
    	public bool useSeperation;
    	public float seperationWeight;
        Vector3 _seperationForce;
    	
        [Header ("Alignment")]
    	public bool useAlignment;
    	public float alignmentWeight;
        Vector3 _alignmentForce;
    	
        [Header ("Cohesion")]
    	public bool useCohesion;
    	public float cohesionWeight;
        Vector3 _centerOfMass;
        Vector3 _cohesionForce;

        [Header ("Teams")]
        public bool onlyWithGroup;
        public int groupId;
    	
    	List<SteeringCharacter> _listOfNeighbors;
        Vector3 _dirn;
        int _totalCountOfNeighbors;
    	
        override protected void Init() {
            base.Init();

    		_listOfNeighbors = new List<SteeringCharacter>();
    	}

    	public override void InitForFrame () {		
            base.InitForFrame();

            if(showDebug) {
                for(int i=0; i<_totalCountOfNeighbors; ++i) {
                    _listOfNeighbors[i].SetColor(Color.white);
                }
            }
            _listOfNeighbors.Clear();
            SteeringManager.Instance.GetNeighbors(ref _listOfNeighbors, _steeringCharacter, neighborRadius, onlyWithGroup, groupId);
            _totalCountOfNeighbors = _listOfNeighbors.Count;
    	}

        public override void UpdateForFrame() {
    		if(_listOfNeighbors.Count==0) 
    			return;
    		
            if(showDebug) {
                for(int i=0; i<_totalCountOfNeighbors; ++i) {
    				_listOfNeighbors[i].SetColor(Color.green);
    			}
    		}
    		
    		if(useSeperation) {
                // Seperation is the combined force pushing away from its neighbors based on neighbor distance
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
    		}
    		
    		if(useAlignment) {
    			// Alignment is the force pushing a character to try and line up with its neighbors
                // It is based on the average velocity of all neighbors
    			_alignmentForce = Vector3.zero;
                for(int i=0; i<_totalCountOfNeighbors; ++i) {
                    _alignmentForce += _listOfNeighbors[i].Velocity;
    			}
                _alignmentForce /= _totalCountOfNeighbors;
    			
                steeringForce += _alignmentForce.normalized * alignmentWeight;
    		}
    		
    		if(useCohesion) {
    			// Cohesion is the force pushing a character towards the center of mass of all its neighbors
                _centerOfMass = Vector3.zero;
                for(int i=0; i<_totalCountOfNeighbors; ++i) {
                    _centerOfMass += _listOfNeighbors[i].Position;
    			}
                _centerOfMass /= _totalCountOfNeighbors;
                _cohesionForce = _centerOfMass - _steeringCharacter.Position;

                steeringForce += _cohesionForce.normalized * cohesionWeight;
    		}

            steeringForce -= _steeringCharacter.Velocity;
    	}
    	
        override public string GetName() {
            return "Flocking";
        }

        override protected bool CanExecuteThisFrame() {
            return true;
        }

        override protected void DebugDrawOnRun() {
            base.DebugDrawOnRun();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_steeringCharacter.Position, neighborRadius);
        }
    }

}
