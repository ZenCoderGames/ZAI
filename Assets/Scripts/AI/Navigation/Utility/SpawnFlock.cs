using UnityEngine;
using System.Collections.Generic;

namespace ZAI {

// This script is a utility script to set all the flock properties
// in a more user-friendly way.
// This is not needed but can speed testing of flock behaviors
    public class SpawnFlock : MonoBehaviour {
    	float timer;
    	Flocking flocking;
    	
    	public GameObject CreaturePrefab;
        public GameObject seekTarget;
        public SteeringCharacter evadeTarget;
    	
    	public float weight;
    	public float aoe;
    	public int priority;
    	
    	public float neighborRadius;
    	
    	public bool useSeperation;
    	public float seperationWeight;
    	Vector3 seperationForce;
    	
    	public bool useAlignment;
    	public float alignmentWeight;
    	Vector3 alignmentForce;
    	
    	public bool useCohesion;
    	public float cohesionWeight;
    	Vector3 cohesionForce;

        public bool onlyWithGroup;
        public int groupId;

    	Transform _transform;

    	bool _isCompleted;

        public int totalToSpawn = 10;

    	// Use this for initialization
    	void Start () {
    		_transform = this.transform;

            Init(totalToSpawn);
    	}

    	public void Init(int numUnits) {
            for(int i=0; i<numUnits; ++i)
                createCreature(i);
    	}
    	
        void createCreature(int i)
    	{
    		GameObject flockCharGO = Instantiate(CreaturePrefab) as GameObject;
    		Transform flockGOTransform = flockCharGO.transform;
            if(SteeringManager.Instance.useXYPlane)
                flockGOTransform.position = _transform.position + Vector3.up * (i/5) + Vector3.right * (i%5);
            else
                flockGOTransform.position = _transform.position + Vector3.forward * (i/5) + Vector3.right * (i%5);

            // Group Setup
            flockCharGO.GetComponent<SteeringCharacter>().groupId = groupId;
    		
    		// Set up seek
            flockCharGO.GetComponent<Seek>().seekTarget = seekTarget;

            // Set up evade
            Evade evade = flockCharGO.GetComponent<Evade>();
            if(evade!=null) {
                evade.targetCharacter = evadeTarget;
            }
    		
    		// Set up flocking
    		flocking = flockCharGO.GetComponent<Flocking>();
    		flocking.weight = weight;
    		flocking.aoe = aoe;
    		flocking.priority = priority;
    		flocking.neighborRadius = neighborRadius;
    		flocking.useSeperation = useSeperation;
    		flocking.seperationWeight = seperationWeight;
    		flocking.useAlignment = useAlignment;
    		flocking.alignmentWeight = alignmentWeight;
    		flocking.useCohesion = useCohesion;
    		flocking.cohesionWeight = cohesionWeight;
            flocking.onlyWithGroup = onlyWithGroup;
            flocking.groupId = groupId;
    	}

        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }

}
