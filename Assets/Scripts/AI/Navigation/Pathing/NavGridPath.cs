using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

namespace ZAI {

    public class NavGridPath {
        NavGrid _navGrid;

        FastPriorityQueue<NavGrid.Cell> _unvisitedCellsFPQ;
        List<NavGrid.Cell> _path;
        int _totalPointsInPath;
        NavGrid.Cell _currentPathStartCell, _currentPathEndCell;
        NavGrid.Cell[,] _cellGrid;
        Vector3 _nextPos;
        int _currentPathIndex;
        SteeringCharacter _aiCharacter;

        static bool DEBUG = false;

        public NavGridPath (NavGrid navGrid, SteeringCharacter aiCharacter) {
            _navGrid = navGrid;
            _cellGrid = _navGrid.GetCellGrid();
            _path = new List<NavGrid.Cell>();
            _totalPointsInPath = 0;
            _currentPathIndex = 0;
            _unvisitedCellsFPQ = new FastPriorityQueue<NavGrid.Cell>(_navGrid.rows*_navGrid.cols);
            _aiCharacter = aiCharacter;
        }

        #region ASTAR
        void GenerateAStarPath(Vector3 start, Vector3 destination) {
            NavGrid.Cell startCell = _navGrid.FindFirstCellThatContainsPoint(start);
            if(startCell==null || startCell.isObstacle) {
                if(DEBUG)
                    Debug.Log("Error: Start cell is not on grid or in obstacle");
                return;
            }

            NavGrid.Cell destinationCell = _navGrid.FindFirstCellThatContainsPoint(destination);
            if(destinationCell==null || destinationCell.isObstacle) {
                if(DEBUG)
                    Debug.Log("Error: Destination cell is not on grid or in obstacle");
                return;
            }

            _unvisitedCellsFPQ.Clear();
            _navGrid.Refresh(false);

            _path.Clear();
            _totalPointsInPath = 0;
            _currentPathIndex = 0;
            _currentPathStartCell = startCell;
            _currentPathEndCell = destinationCell;

            startCell.cost = 0;
            _unvisitedCellsFPQ.Enqueue(startCell, 0);

            int numCellsVisited = 0;
            while(_unvisitedCellsFPQ.Count>0) {
                NavGrid.Cell c = _unvisitedCellsFPQ.Dequeue();

                c.isInOpenList = false;
                numCellsVisited++;

                bool hasReachedDestination = false;
                int numNeighbors = c.neighbors.Count;
                for(int i=0; i<numNeighbors; ++i) {
                    if(EvaluateNeighbor(c, c.neighbors[i])) {
                        hasReachedDestination = true;
                        break;
                    }
                }

                if(hasReachedDestination) {
                    break;
                }
            }

            // Calculate Path
            NavGrid.Cell pathFinder = destinationCell;
            _path.Add(pathFinder);
            while(pathFinder!=startCell) {
                pathFinder = pathFinder.parent;
                _path.Add(pathFinder);
            }
            _path.Reverse();

            _totalPointsInPath = _path.Count;

            //DebugGrid();
            //Debug.Break();
        }

        bool EvaluateNeighbor(NavGrid.Cell parentCell, NavGrid.Cell neighborCell) {
            int r = neighborCell.row;
            int c = neighborCell.col;

            if(neighborCell.isObstacle) {
                return false;
            }
            else {
                int costFromParent = (r==parentCell.row || c==parentCell.col)?2:3; // diagonals are more expensive
                int newCost = parentCell.cost + costFromParent;
                if(newCost<neighborCell.cost) {
                    neighborCell.cost = newCost;
                    int heuristic = GetHeuristic(r, c, _currentPathEndCell.row, c);
                    int priority = neighborCell.cost + heuristic;
                    if(neighborCell.isInOpenList) {
                        _unvisitedCellsFPQ.Remove(neighborCell);
                    }
                    _unvisitedCellsFPQ.Enqueue(neighborCell, priority);
                    neighborCell.isInOpenList = true;
                    neighborCell.visited++;
                    neighborCell.parent = parentCell;

                    if(neighborCell == _currentPathEndCell) {
                        return true;
                    }
                }
            }

            return false;
        }

        // Diagonal distance heuristic (improvement on Manhattan distance)
        int GetHeuristic(int r, int c, int destR, int destC) {
            const int D = 2;
            const int D2 = 3;
            const int NUDGE_FACTOR = 1;
            int dr = Mathf.Abs(destR - r);
            int dc = Mathf.Abs(destC - c);
            int heuristic = 0;

            if (dr > dc) 
                heuristic = (D * (dr-dc) + D2 * dc); 
            else 
                heuristic = (D * (dc-dr) + D2 * dr);

            return heuristic;
        }
        #endregion

        #region PATH_FOLLOWING
        public Vector3 GetNextPos(Vector3 destination, ref bool isDestinationCell) {
            // If path has never been created, create it
            if(_path==null) {
                GenerateAStarPath(_aiCharacter.Position, destination);
            }

            // Else run through the path to see where the current and destination positions lie
            bool currentPositionExistsInPath = false;
            bool destinationExistsInPathAndIsLastNode = false;
            int currentPathIndex = 0;
            for(int i=0; i<_totalPointsInPath; ++i) {
                if(_path[i].HasCharacter(_aiCharacter)) {
                    currentPositionExistsInPath = true;
                    currentPathIndex = i;
                }
                if(i==_totalPointsInPath-1 && _path[i].HasPosition(destination)) {
                    destinationExistsInPathAndIsLastNode = true;
                }
            }

            // If current position or the destination position is off the path
            if(!currentPositionExistsInPath || !destinationExistsInPathAndIsLastNode) {
                GenerateAStarPath(_aiCharacter.Position, destination);
            }
            // Else positions are on the path
            else {
                // Choosing the next node
                if(currentPathIndex >= _currentPathIndex) {
                    _currentPathIndex = currentPathIndex+1;
                    if(_currentPathIndex>=_totalPointsInPath) {
                        _currentPathIndex = _totalPointsInPath-1;
                    }
                }
            }

            isDestinationCell = (_currentPathIndex == _totalPointsInPath-1);

            // Else return the current path node position
            return _path[_currentPathIndex].GetPosition();
        }
        #endregion

        #region DEBUG
        void DebugGrid() {
            for(int r=0; r<_navGrid.rows; ++r) {
                for(int c=0; c<_navGrid.cols; ++c) {
                    NavGrid.Cell cell = _cellGrid[r,c];
                    if(cell.cost<int.MaxValue) {
                        Debug.Log("(" + cell.row.ToString() + "," + cell.col.ToString() + ") = " + cell.cost.ToString());
                    }
                }
            }
        }

        public void DebugDrawOnRun() {
            for(int i=0; i<_path.Count; ++i) {
                NavGrid.Cell cell = _path[i];
                if(cell == _currentPathStartCell)
                    Gizmos.color = Color.yellow;
                else if(cell == _currentPathEndCell)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawSphere(cell.GetPosition(), 0.55f);
            }
        }
        #endregion
    }

}
