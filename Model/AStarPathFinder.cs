
namespace Model
{
    public class AStarPathFinder : IPathFinder
    {
        readonly PathFinderType _algType = PathFinderType.Astar;
        public PathFinderType algType { get => _algType; set {} }

        public void FindPath(Maze maze, int[] pos, Queue<int[]> visitedPositions)
        {
            if (maze?.MazeArray == null || maze.MazeArray.Length == 0)
                return;

            if (pos[0] == maze.End[0] && pos[1] == maze.End[1])
            {
                visitedPositions.Enqueue(pos);
                return;
            }

            var openSet = new SortedSet<Node>(new NodeComparer());
            var gScore = new Dictionary<string, int>();
            var parent = new Dictionary<string, int[]?>();
            var closedSet = new HashSet<string>();
            int insertionCounter = 0;

            string startKey = PositionKey(pos);
            gScore[startKey] = 0;
            parent[startKey] = null;

            var startNode = new Node
            {
                Position = pos,
                G = 0,
                F = ManhattanDistance(pos, maze.End),
                InsertionOrder = insertionCounter++
            };
            openSet.Add(startNode);

            bool foundEnd = false;
            int[]? endPosition = null;

            while (openSet.Count > 0)
            {
                var current = openSet.Min;
                if (current == null) break;
                openSet.Remove(current);

                var currentPos = current.Position;
                var currentKey = PositionKey(currentPos);

                if (currentPos[0] == maze.End[0] && currentPos[1] == maze.End[1])
                {
                    foundEnd = true;
                    endPosition = currentPos;
                    break;
                }

                closedSet.Add(currentKey);

                if (maze.MazeArray[currentPos[0]][currentPos[1]] != 1 &&
                    maze.MazeArray[currentPos[0]][currentPos[1]] != 2)
                {
                    maze.MazeArray[currentPos[0]][currentPos[1]] = 4;
                }

                foreach (var move in maze.moves)
                {
                    int newRow = currentPos[0] + move[0];
                    int newCol = currentPos[1] + move[1];
                    var newPos = new int[] { newRow, newCol };
                    var newKey = PositionKey(newPos);

                    if (!maze.IsValidMove(newRow, newCol) || closedSet.Contains(newKey))
                        continue;

                    int cellValue = maze.MazeArray[newRow][newCol];
                    if (cellValue != 0 && cellValue != 2)
                        continue;

                    int tentativeG = gScore[currentKey] + 1;

                    if (!gScore.TryGetValue(newKey, out int existingG) || tentativeG < existingG)
                    {
                        parent[newKey] = currentPos;
                        gScore[newKey] = tentativeG;

                        int h = ManhattanDistance(newPos, maze.End);
                        int f = tentativeG + h;

                        var existingNode = openSet.FirstOrDefault(n =>
                            n.Position[0] == newPos[0] && n.Position[1] == newPos[1]);
                        if (existingNode != null)
                        {
                            openSet.Remove(existingNode);
                        }

                        var newNode = new Node
                        {
                            Position = newPos,
                            G = tentativeG,
                            F = f,
                            InsertionOrder = insertionCounter++
                        };
                        openSet.Add(newNode);
                    }
                }
            }

            if (foundEnd && endPosition != null)
            {
                ReconstructPath(parent, endPosition, visitedPositions);
            }
            else
            {
                visitedPositions.Enqueue(pos);
            }
        }

        private static int ManhattanDistance(int[] from, int[] to)
        {
            return Math.Abs(from[0] - to[0]) + Math.Abs(from[1] - to[1]);
        }

        private static string PositionKey(int[] pos)
        {
            return $"{pos[0]},{pos[1]}";
        }

        private void ReconstructPath(Dictionary<string, int[]?> parent, int[] endPos, Queue<int[]> visitedPositions)
        {
            var path = new List<int[]>();
            int[]? current = endPos;

            while (current != null)
            {
                path.Add(current);
                var key = PositionKey(current);
                if (parent.TryGetValue(key, out int[]? value))
                {
                    current = value;
                }
                else
                {
                    current = null;
                }
            }

            path.Reverse();

            foreach (var position in path)
            {
                visitedPositions.Enqueue(position);
            }
        }

        private class Node
        {
            public int[] Position { get; set; } = null!;
            public int F { get; set; }
            public int G { get; set; }
            public int InsertionOrder { get; set; }
        }

        private class NodeComparer : IComparer<Node>
        {
            public int Compare(Node? x, Node? y)
            {
                if (x == null || y == null) return 0;
                int fCompare = x.F.CompareTo(y.F);
                if (fCompare != 0)
                    return fCompare;

                int gCompare = x.G.CompareTo(y.G);
                if (gCompare != 0)
                    return gCompare;

                return x.InsertionOrder.CompareTo(y.InsertionOrder);
            }
        }
    }
}

            

