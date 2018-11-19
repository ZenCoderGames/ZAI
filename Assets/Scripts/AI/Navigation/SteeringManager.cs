/**********************************************************************************
 * Class:   AIManager
 * Purpose: This class is the manager of all the AICharacters in the scene.
 *          It is the one global singleton that keeps a reference to obstacles,
 *          navgrids, space partitions, flow-fields etc.
 *          Make sure a version of this always exists in the scene where
 *          AICharacters are used.
 * ********************************************************************************/

using UnityEngine;
using System.Collections.Generic;

namespace ZAI {
    
    public class SteeringManager : MonoBehaviour {
    		
    	static SteeringManager _instance;
    	public static SteeringManager Instance { 
            get { 
                if(_instance==null) {
                    GameObject obj = new GameObject("Steering Manager");
                    SteeringManager steeringManager = obj.AddComponent<SteeringManager>();
                    return steeringManager;
                }
                else {
                    return _instance; 
                }
            } 
        }

        public bool useXYPlane;

    	List<SteeringCharacter> _listOfAICharacters;
        List<Obstacle> _listOfObstacles;
        List<Wall> _listOfWalls;
        FlowField _flowField;
        CellSpacePartition _cellSpacePartition;
        NavGrid _navGrid;

        public bool IsInitialized { get { return _isInitialized; } }
        bool _isInitialized;
        public event System.Action OnInitialized;
    	
    	void Awake() {
    		_instance = this;

    		_listOfAICharacters = new List<SteeringCharacter>();
    		_listOfObstacles = new List<Obstacle>();
            _listOfWalls = new List<Wall>();
    	}

    	void OnDestroy() {
    		_instance = null;
    	}

        void Start() {
            Init();
        }

        void Init() {
            if(OnInitialized!=null) {
                OnInitialized();
            }
            _isInitialized = true;
        }
    	
    	void Update() {
    		int count = _listOfAICharacters.Count;
            // Init Tick
    		for(int i=0; i<count; ++i) {
    			_listOfAICharacters[i].InitForTick();
    		}
            // Update Tick
    		for(int i=0; i<count; ++i) {
    			_listOfAICharacters[i].UpdateForTick();	
    		}
            if(_cellSpacePartition!=null) {
                _cellSpacePartition.UpdateGrid();
            }
            // Late Update Tick
            for(int i=0; i<count; ++i) {
                _listOfAICharacters[i].LateUpdateForTick(); 
            }
    	}

        void LateUpdate() {
            int count = _listOfAICharacters.Count;
            for(int i=0; i<count; ++i) {
                _listOfAICharacters[i].EndOfFrameUpdate(); 
            }
        }
    	
    	public void AddCharacter(SteeringCharacter aiChar) {
    		_listOfAICharacters.Add(aiChar);
    	}
    	
    	public void RemoveCharacter(SteeringCharacter aiChar) {
    		_listOfAICharacters.Remove(aiChar);	
    	}

        public List<SteeringCharacter> GetCharacters() {
            return _listOfAICharacters;
        }
    	
        // Note: When calling this when CellSpacePartitioning is on, it only gets characters around the
        //       neighboring cells of a character's cell for performance reasons
        public void GetNeighbors(ref List<SteeringCharacter> listOfNeighbors, SteeringCharacter currentChar, float radius, bool onlyWithGroup=false, int groupId=0) {
            List<SteeringCharacter> charsToEvaluate = null;
            if(_cellSpacePartition!=null) {
                charsToEvaluate = _cellSpacePartition.GetCharactersAroundMe(currentChar);
            }
            else {
                charsToEvaluate = _listOfAICharacters;
            }

    		Vector3 dist;
            int count = charsToEvaluate.Count;
    		SteeringCharacter aiChar = null;
    		float radSquare = radius * radius;	
    		for(int i=0; i<count; ++i) {
                aiChar = charsToEvaluate[i];
                if(aiChar==currentChar) {
                    continue;
                }

                if(onlyWithGroup) {
                    if(aiChar.groupId!=groupId) {
                        continue;
                    }
                }

    			dist = aiChar.Position - currentChar.Position;
    			if(dist.sqrMagnitude<radSquare) {
    				listOfNeighbors.Add(aiChar);
    			}
    		}
    	}

        #region OBSTACLES
        public void AddObstacle(Obstacle obstacle) {
            _listOfObstacles.Add(obstacle);
        }

        public void RemoveObstacle(Obstacle obstacle) {
            _listOfObstacles.Remove(obstacle);
        }

        public List<Obstacle> GetObstacles() {
            return _listOfObstacles;
        }
        #endregion

        #region WALLS
        public void AddWall(Wall wall) {
            _listOfWalls.Add(wall);
        }

        public void RemoveWall(Wall wall) {
            _listOfWalls.Remove(wall);
        }

        public List<Wall> GetWalls() {
            return _listOfWalls;
        }
        #endregion

        #region FLOW_FIELD
        public void RegisterFlowField(FlowField flowField) {
            _flowField = flowField;
        }

        public FlowField GetFlowField() {
            return _flowField;
        }
        #endregion

        #region NON_PENETRATION
        public void DoNonPenetrationLogic(SteeringCharacter aiCharacter) {
            List<SteeringCharacter> listOfCharacters = null;

            if(_cellSpacePartition!=null) {
                listOfCharacters = _cellSpacePartition.GetCharactersAroundMe(aiCharacter);
            }
            else {
                listOfCharacters = _listOfAICharacters;
            }

            int aiCount = listOfCharacters.Count;
            SteeringCharacter currentChar = null;
            for(int i=0; i<aiCount; ++i) {
                currentChar = listOfCharacters[i];

                if(currentChar==aiCharacter)
                    continue;

                Vector3 dirn = aiCharacter.Position - currentChar.Position;
                float dist = dirn.magnitude;
                dirn.Normalize();
                float overlap = currentChar.radius + aiCharacter.radius - dist;
                if(overlap > 0) {
                    aiCharacter.ForceSetPosition(aiCharacter.Position + dirn * overlap);
                }
            }
        }
        #endregion

        #region CELL_SPACE_PARTITION
        public void RegisterCellSpacePartition(CellSpacePartition cellSpacePartition) {
            _cellSpacePartition = cellSpacePartition;
        }
        #endregion

        #region NAV_GRID
        public void RegisterNavGrid(NavGrid navGrid) {
            _navGrid = navGrid;
        }

        public NavGrid GetNavGrid() {
            return _navGrid;
        }
        #endregion
    }

}
