// Only 2d implementation for now

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZAI {

    public class FlowField : MonoBehaviour {
        public int rows, cols;
        public float cellSize;
        public Transform target;
        public bool autoRefresh;
        public bool refreshObstacles;
        public bool showDebug;

        public class Cell {
            public int row;
            public int col;
            public int cost;
            public bool isInOpenList;
            public bool visited;
            public Cell parent;
            public bool isObstacle;
            public List<Cell> neighbors;

            FlowField _flowField;
            Vector3 _cellCenter;
            float _halfSize;
            Vector2 _botLeft, _botRight, _topLeft, _topRight;
            Rect _cellRect;

            public Cell(FlowField flowField, int r, int c) {
                _flowField = flowField;
                row = r;
                col = c;
                _halfSize = _flowField.cellSize/2;

                if(SteeringManager.Instance.useXYPlane) {
                    _cellCenter = _flowField.StartPosition + Vector3.right * row * _flowField.cellSize + Vector3.up * col * _flowField.cellSize;
                    _botLeft = new Vector2(_cellCenter.x - _halfSize, _cellCenter.y - _halfSize);
                    _botRight = new Vector2(_cellCenter.x + _halfSize, _cellCenter.y - _halfSize);
                    _topLeft = new Vector2(_cellCenter.x - _halfSize, _cellCenter.y + _halfSize);
                    _topRight = new Vector2(_cellCenter.x + _halfSize, _cellCenter.y + _halfSize);
                }
                else {
                    _cellCenter = _flowField.StartPosition + Vector3.right * row * _flowField.cellSize + Vector3.forward * col * _flowField.cellSize;
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
                visited = false;
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
        List<Cell> _unvisitedCells;
        int _maxVal;
        public Vector3 StartPosition { get { return _transform.position; } }
        Transform _transform;
        Cell _targetCell;
        bool _isInitialized;

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
            SteeringManager.Instance.RegisterFlowField(this);
        }

        [ContextMenu ("Create New Grid")]
        public void CreateNewGrid() {
            _grid = new Cell[rows,cols];
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    _grid[r,c] = new Cell(this, r, c);
                }
            }

            // Cache neighbors
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    _grid[r,c].CacheNeighbors(_grid, rows, cols);
                }
            }

            _isInitialized = true;

            _unvisitedCells = new List<Cell>();
            ForceRefreshGrid();
        }

        [ContextMenu ("RefreshGrid")]
        public void ForceRefreshGrid() {
            RefreshGrid(true);
        }

        void RefreshGrid(bool evaluateObstacles) {
            if(!_isInitialized) {
                return;
            }

            if(target==null) {
                Debug.Log("No target specified so flow field isn't generated");
                return;
            }

            Vector3 targetPos = target.transform.position;
            if(!evaluateObstacles && _targetCell!=null) {
                // Re-evaluate whether target has left this cell, if not, don't refresh grid
                if(_targetCell.HasPosition(targetPos)) {
                    return;
                }
            }

            List<Obstacle> obstacleList = SteeringManager.Instance.GetObstacles();
            int numObstacles = obstacleList.Count;

            _targetCell = null;
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
                    // Check for target position
                    if(cell.HasPosition(targetPos)) {
                        _targetCell = cell;
                    }
                }
            }

            if(_targetCell!=null) {
                _unvisitedCells.Clear();
                GenerateGrid();
            }
        }

        void GenerateGrid() {
            _targetCell.cost = 0;
            _unvisitedCells.Add(_targetCell);

            int numCellsVisited = 0;

            while(_unvisitedCells.Count>0) {
                Cell c = _unvisitedCells[0];

                c.visited = true;
                numCellsVisited++;

                _unvisitedCells.RemoveAt(0);

                int numNeighbors = c.neighbors.Count;
                for(int i=0; i<numNeighbors; ++i) {
                    EvaluateNeighbor(c, c.neighbors[i]);
                }
            }
        }

        void EvaluateNeighbor(Cell parentCell, Cell neighborCell) {
            int r = neighborCell.row;
            int c = neighborCell.col;

            if(parentCell.isObstacle || neighborCell.isObstacle) {
                return;
            }
            else {
                int costFromParent = (r==parentCell.row || c==parentCell.col)?2:3; // diagonals are more expensive
                int newCost = parentCell.cost + costFromParent;
                if(newCost<neighborCell.cost) {
                    neighborCell.cost = newCost;
                    if(!neighborCell.visited) {
                        _unvisitedCells.Add(neighborCell);
                    }
                    neighborCell.parent = parentCell;
                }
            }
        }

        public Vector3 GetFuturePosition(Vector3 aiPosition, Vector3 currentTargetPosition, ref bool isFinalCell) {
            Cell cell = null;
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    cell = _grid[r,c];

                    if(cell.HasPosition(aiPosition)) {
                        if(cell.isObstacle) {
                            return currentTargetPosition;
                        }
                        else {
                            isFinalCell = (cell.parent==null);
                            // If Destination cell
                            if(isFinalCell) {
                                return cell.GetPosition();
                            }
                            // If Not Destination cell
                            else {
                                return cell.parent.GetPosition();
                            }
                        }
                    }
                }
            }

            return _targetCell.GetPosition();
        }

        public bool IsValid() {
            return _targetCell!=null;
        }

        void Update() {
            if(!_isInitialized) {
                return;
            }

            if(autoRefresh && target!=null) {
                RefreshGrid(refreshObstacles);
                refreshObstacles = false;
            }
        }

        #region DEBUG
        void OnDrawGizmos() {
            if(_isInitialized && showDebug) {
                for(int r=0; r<rows; ++r) {
                    for(int c=0; c<cols; ++c) {
                        Cell cell = _grid[r,c];

                        if(cell.parent!=null) {
                            Vector3 dirn = cell.parent.GetPosition() - cell.GetPosition();
                            ZUtils.DrawGizmoArrow(cell.GetPosition(), dirn/3, SteeringManager.Instance.useXYPlane);
                        }
                    }
                }
            }
        }
        #endregion
    }

}
