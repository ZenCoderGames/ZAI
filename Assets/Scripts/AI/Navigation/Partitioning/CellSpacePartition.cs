// Note: This is only a 2D representation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZAI {

    public class CellSpacePartition : MonoBehaviour {
        public int rows, cols;
        public int cellSize;
        public bool showDebug;

        public class Cell {
            public int row;
            public int col;
            public bool debugHighlight;
            public List<Cell> neighbors;

            List<SteeringCharacter> _listOfAICharacters;
            Vector3 _cellCenter;
            float _halfSize;
            Vector3 _botLeft, _botRight, _topLeft, _topRight;
            Vector2 _botLeft2D, _botRight2D, _topLeft2D, _topRight2D;
            Rect _cellRect;

            public Cell(Vector3 startPos, int r, int c, int cellSize) {
                row = r;
                col = c;
                _halfSize = cellSize/2;

                if(SteeringManager.Instance.useXYPlane) {
                    _cellCenter = startPos + Vector3.right * row * cellSize + Vector3.up * col * cellSize;
                    _botLeft = new Vector3(_cellCenter.x - _halfSize, _cellCenter.y - _halfSize, 0);
                    _botLeft2D = new Vector2(_botLeft.x, _botLeft.y);
                    _botRight = new Vector3(_cellCenter.x + _halfSize, _cellCenter.y - _halfSize, 0);
                    _botRight2D = new Vector2(_botRight.x, _botRight.y);
                    _topLeft = new Vector3(_cellCenter.x - _halfSize, _cellCenter.y + _halfSize, 0);
                    _topLeft2D = new Vector2(_topLeft.x, _topLeft.y);
                    _topRight = new Vector3(_cellCenter.x + _halfSize, _cellCenter.y + _halfSize, 0);
                    _topRight2D = new Vector2(_topRight.x, _topRight.y);
                }
                else {
                    _cellCenter = startPos + Vector3.right * row * cellSize + Vector3.forward * col * cellSize;
                    _botLeft = new Vector3(_cellCenter.x - _halfSize, 0, _cellCenter.z - _halfSize);
                    _botLeft2D = new Vector2(_botLeft.x, _botLeft.z);
                    _botRight = new Vector3(_cellCenter.x + _halfSize, 0, _cellCenter.z - _halfSize);
                    _botRight2D = new Vector2(_botRight.x, _botRight.z);
                    _topLeft = new Vector3(_cellCenter.x - _halfSize, 0, _cellCenter.z + _halfSize);
                    _topLeft2D = new Vector2(_topLeft.x, _topLeft.z);
                    _topRight = new Vector3(_cellCenter.x + _halfSize, 0, _cellCenter.z + _halfSize);
                    _topRight2D = new Vector2(_topRight.x, _topRight.z);
                }

                _listOfAICharacters = new List<SteeringCharacter>();
                _cellRect = new Rect(_botLeft2D.x, _botLeft2D.y, _halfSize*2, _halfSize*2);
            }

            public void Reset() {
                _listOfAICharacters.Clear();
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

            public Vector3 GetBotLeftPos() {
                return _botLeft;
            }

            public Vector3 GetBotRightPos() {
                return _botRight;
            }

            public Vector3 GetTopLeftPos() {
                return _topLeft;
            }

            public Vector3 GetTopRightPos() {
                return _topRight;
            }

            public void StoreRefIfWithin(SteeringCharacter aiChar) {
                if(IsContained(aiChar.Position, aiChar.radius)) {
                    _listOfAICharacters.Add(aiChar);
                }
            }

            public bool HasCharacters() {
                return _listOfAICharacters.Count>0;
            }

            public bool HasCharacter(SteeringCharacter aiChar) {
                return _listOfAICharacters.Contains(aiChar);
            }

            public List<SteeringCharacter> GetCharacters() {
                return _listOfAICharacters;
            }

            public bool IsContained(Vector3 pos, float r) {
                float posYValue = pos.z;
                if(SteeringManager.Instance.useXYPlane) {
                    posYValue = pos.y;
                }

                Rect rect = new Rect(pos.x - r, posYValue - r, r*2, r*2);

                if(rect.Contains(_botLeft2D) || rect.Contains(_botRight2D) ||
                    rect.Contains(_topLeft2D) || rect.Contains(_topRight2D)) {
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
        }

        Cell[,] _grid;
        Transform _transform;
        List<SteeringCharacter> _listOfChars;
        List<SteeringCharacter> _listOfQueriedChars = new List<SteeringCharacter>();
        Dictionary<SteeringCharacter, bool> _repeatDict = new Dictionary<SteeringCharacter, bool>();

    	void Awake() {
            _transform = transform;
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
            CreateNewGrid();
            SteeringManager.Instance.RegisterCellSpacePartition(this);
        }

        [ContextMenu ("CreateNewGrid")]
        public void CreateNewGrid() {
            _grid = new Cell[rows,cols];
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    _grid[r,c] = new Cell(_transform.position, r, c, cellSize);
                }
            }

            // Cache neighbors
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    _grid[r,c].CacheNeighbors(_grid, rows, cols);
                }
            }
        }

        public void UpdateGrid() {
            // Track positions of all characters
            _listOfChars = SteeringManager.Instance.GetCharacters();
            int totalChars = _listOfChars.Count;
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    _grid[r,c].Reset();
                    for(int i=0; i<totalChars; ++i) {
                        _grid[r,c].StoreRefIfWithin(_listOfChars[i]);
                    }
                    _grid[r,c].debugHighlight = _grid[r,c].GetCharacters().Count>0;
                }
            }
        }

        public List<SteeringCharacter> GetCharacters(Vector3 pos, float radius) {
            List<Cell> cellsInRange = new List<Cell>();
            Cell currentCell = null;
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    currentCell = _grid[r,c];
                    if(currentCell.IsContained(pos, radius)) {
                        cellsInRange.Add(currentCell);
                    }
                }
            }

            return GetListOfUniqueCharacters(cellsInRange);
        }

        // Find characters in all cells the current character is and all neighboring cells
        public List<SteeringCharacter> GetCharactersAroundMe(SteeringCharacter aiChar) {
            List<Cell> cellsInRange = new List<Cell>();
            Cell currentCell = null;
            for(int r=0; r<rows; ++r) {
                for(int c=0; c<cols; ++c) {
                    currentCell = _grid[r,c];
                    if(currentCell.HasCharacter(aiChar)) {
                        cellsInRange.Add(currentCell);
                    }
                }
            }

            // Expand around these cells
            int numCellsInRange = cellsInRange.Count;
            if(numCellsInRange>0) {
                for(int i=0; i<numCellsInRange; ++i) {
                    int neighborsCount = cellsInRange[i].neighbors.Count;
                    Cell neighborCell = null;
                    for(int j=0; j<neighborsCount; ++j) {
                        neighborCell = cellsInRange[i].neighbors[j];
                        if(neighborCell.HasCharacters() && !cellsInRange.Contains(neighborCell)) {
                            cellsInRange.Add(neighborCell);
                        }
                    }
                }
            }

            return GetListOfUniqueCharacters(cellsInRange);
        }

        List<SteeringCharacter> GetListOfUniqueCharacters(List<Cell> cellsInRange) {
            _listOfQueriedChars.Clear();
            Cell currentCell = null;
            int numCellsInRange = cellsInRange.Count;
            if(numCellsInRange>0) {
                List<SteeringCharacter> charsInCell = null;
                int numCharsInCell = 0;
                _repeatDict.Clear();
                for(int i=0; i<numCellsInRange; ++i) {
                    currentCell = cellsInRange[i];
                    charsInCell = currentCell.GetCharacters();
                    numCharsInCell = charsInCell.Count;
                    for(int j=0; j<numCharsInCell; ++j) {
                        if(!_repeatDict.ContainsKey(charsInCell[j])) {
                            _listOfQueriedChars.Add(charsInCell[j]);
                            _repeatDict.Add(charsInCell[j], true);
                        }
                    }
                }
            }
            return _listOfQueriedChars;
        }

        #region DEBUG
        void OnDrawGizmos() {
            if(showDebug && _grid!=null) {
                for(int r=0; r<rows; ++r) {
                    for(int c=0; c<cols; ++c) {
                        Vector3 cellCenter = Vector3.zero;
                        if(SteeringManager.Instance.useXYPlane) {
                            cellCenter = _transform.position + Vector3.right * r * cellSize + Vector3.up * c * cellSize;
                        }
                        else {
                            cellCenter = _transform.position + Vector3.right * r * cellSize + Vector3.forward * c * cellSize;
                        }
                        Cell cell = _grid[r,c];

                        if(cell.debugHighlight)
                            Gizmos.color = Color.green;
                        else
                            Gizmos.color = Color.white; 
                        
                        Gizmos.DrawLine(cell.GetBotLeftPos(), cell.GetBotRightPos());
                        Gizmos.DrawLine(cell.GetBotRightPos(), cell.GetTopRightPos());
                        Gizmos.DrawLine(cell.GetTopRightPos(), cell.GetTopLeftPos());
                        Gizmos.DrawLine(cell.GetTopLeftPos(), cell.GetBotLeftPos());
                    }
                }
            }
        }
        #endregion
    }

}
