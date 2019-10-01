using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

namespace ZAI {

    public class NavGrid : MonoBehaviour {
        public int rows, cols;
        public float cellSize;
        public bool refreshObstacles;
        public bool showGrid;
        public bool showDebug;

        public class Cell:FastPriorityQueueNode {
            public int row;
            public int col;
            public int cost;
            public bool isInOpenList;
            public int visited;
            public Cell parent;
            public bool isObstacle;
            public List<Cell> neighbors;

            Vector3 _cellCenter;
            float _halfSize;
            Vector2 _botLeft, _botRight, _topLeft, _topRight;
            Rect _cellRect;

            public Cell(Vector3 startPos, float cellSize, int r, int c) {
                row = r;
                col = c;
                _halfSize = cellSize/2;

                if(SteeringManager.Instance.useXYPlane) {
                    _cellCenter = startPos + Vector3.right * row * cellSize + Vector3.up * col * cellSize;
                    _botLeft = new Vector2(_cellCenter.x - _halfSize, _cellCenter.y - _halfSize);
                    _botRight = new Vector2(_cellCenter.x + _halfSize, _cellCenter.y - _halfSize);
                    _topLeft = new Vector2(_cellCenter.x - _halfSize, _cellCenter.y + _halfSize);
                    _topRight = new Vector2(_cellCenter.x + _halfSize, _cellCenter.y + _halfSize);
                }
                else {
                    _cellCenter = startPos + Vector3.right * row * cellSize + Vector3.forward * col * cellSize;
                    _botLeft = new Vector2(_cellCenter.x - _halfSize, _cellCenter.z - _halfSize);
                    _botRight = new Vector2(_cellCenter.x + _halfSize, _cellCenter.z - _halfSize);
                    _topLeft = new Vector2(_cellCenter.x - _halfSize, _cellCenter.z + _halfSize);
                    _topRight = new Vector2(_cellCenter.x + _halfSize, _cellCenter.z + _halfSize);
                }

                _cellRect = new Rect(_botLeft.x, _botLeft.y, _halfSize*2, _halfSize*2);
            }

            public void Reset() {
                cost = int.MaxValue;
                parent = null;
                visited = 0;
                isInOpenList = false;
            }

            public void CacheNeighbors(Cell[,] grid, int numRows, int numCols) {
                neighbors = new List<Cell>();
                EvaluateNeighbor(grid, numRows, numCols, row-1, col-1);
                EvaluateNeighbor(grid, numRows, numCols, row-1, col);
                EvaluateNeighbor(grid, numRows, numCols, row-1, col+1);
                EvaluateNeighbor(grid, numRows, numCols, row, col-1);
                EvaluateNeighbor(grid, numRows, numCols, row, col+1);
                EvaluateNeighbor(grid, numRows, numCols, row+1, col-1);
                EvaluateNeighbor(grid, numRows, numCols, row+1, col);
                EvaluateNeighbor(grid, numRows, numCols, row+1, col+1);
            }

            void EvaluateNeighbor(Cell[,] grid, int numRows, int numCols, int r, int c) {
                // Out of bounds checks
                if(r<0 || r==numRows || c<0 || c==numCols)
                    return;

                neighbors.Add(grid[r,c]);
            }

            public bool HasPosition(Vector3 pos) {
                float cellCenterYValue = _cellCenter.z;
                float posYValue = pos.z;
                if(SteeringManager.Instance.useXYPlane) {
                    cellCenterYValue = _cellCenter.y;
                    posYValue = pos.y;
                }

                if(_cellCenter.x - _halfSize < pos.x &&
                    _cellCenter.x + _halfSize > pos.x &&
                    cellCenterYValue - _halfSize < posYValue &&
                    cellCenterYValue + _halfSize > posYValue) {
                    return true;
                }

                return false;
            }

            public bool HasCharacter(SteeringCharacter aiChar) {
                return IsContained(aiChar.Position, aiChar.radius);
            }

            public Vector3 GetPosition() {
                return _cellCenter;
            }

            public bool EvaluateObstacle(Obstacle obstacle) {
                if(IsContained(obstacle.Position, obstacle.radius)) {
                    SetAsObstacle(true);
                    return true;
                }
                return false;
            }

            public bool IsContained(Vector3 pos, float r) {
                float posYValue = pos.z;
                if(SteeringManager.Instance.useXYPlane) {
                    posYValue = pos.y;
                }

                Rect rect = new Rect(pos.x - r, posYValue - r, r*2, r*2);

                if(rect.Contains(_botLeft) || rect.Contains(_botRight) ||
                    rect.Contains(_topLeft) || rect.Contains(_topRight)) {
                    return true;
                }

                Vector2 botLeft = new Vector2(pos.x-r, posYValue-r);
                Vector2 botRight = new Vector2(pos.x+r, posYValue-r);
                Vector2 topLeft = new Vector2(pos.x-r, posYValue+r);
                Vector2 topRight = new Vector2(pos.x+r, posYValue+r);

                if(_cellRect.Contains(botLeft) || _cellRect.Contains(botRight) ||
                    _cellRect.Contains(topLeft) || _cellRect.Contains(topRight)) {
                    return true;
                }

                return false;
            }

            public void SetAsObstacle(bool val) {
                isObstacle = val;
            }
        }

        Cell[,] _grid;
        public Vector3 StartPosition { get { return _transform.position; } }
        Transform _transform;

        void Awake() {
            _transform = transform;
        }

        void Start() {
            CallbackManager.Instance.playOnNextFrame(()=>{
                if(SteeringManager.Instance.IsInitialized) {
                    Init();
                }
                else {
                    SteeringManager.Instance.OnInitialized += Init;
                }
            });
        }

        void Init() {
            CreateNewGrid();
            SteeringManager.Instance.RegisterNavGrid(this);
        }

        [ContextMenu ("Create New Grid")]
        public void CreateNewGrid() {
            _grid = new Cell[rows,cols];
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    _grid[r,c] = new Cell(_transform.position, cellSize, r, c);
                }
            }

            // Cache neighbors
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    _grid[r,c].CacheNeighbors(_grid, rows, cols);
                }
            }

            Refresh(true);
        }

        public void Refresh(bool evaluateObstacles) {
            List<Obstacle> obstacleList = SteeringManager.Instance.GetObstacles();
            int numObstacles = obstacleList.Count;

            Cell cell = null;
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    cell = _grid[r,c];
                    cell.Reset();
                    if(evaluateObstacles) {
                        cell.SetAsObstacle(false);
                        for(int i=0; i<numObstacles; ++i) {
                            if(cell.EvaluateObstacle(obstacleList[i])) {
                                break;
                            }
                        }
                    }
                }
            }
        }

        #region HELPERS
        public Cell[,] GetCellGrid() {
            return _grid;
        }

        public Cell FindFirstCellThatContainsPoint(Vector3 pos) {
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    if(_grid[r,c].HasPosition(pos)) {
                        return _grid[r,c];
                    }
                }
            }

            return null;
        }
        #endregion

        #region DEBUG
        void OnDrawGizmos() {
            if(showGrid && _grid!=null) {
                for(int r=0; r<rows; ++r) {
                    for(int c=0; c<cols; ++c) {
                        Cell cell = _grid[r,c];
                        if(showDebug) {
                            if(cell.isObstacle)
                                Gizmos.color = Color.black;
                            else if(cell.visited==1)
                                Gizmos.color = Color.cyan;
                            else if(cell.visited>1)
                                Gizmos.color = Color.red;
                            else
                                Gizmos.color = Color.white;
                        }
                        else {
                            Gizmos.color = Color.white;
                        }

                        Gizmos.DrawSphere(cell.GetPosition(), 0.5f);
                    }
                }
            }
        }
        #endregion
    }

}
