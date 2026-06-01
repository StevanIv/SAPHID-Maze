
namespace Model
{
    public class StackPathFinder : IPathFinder
    {
        readonly PathFinderType _algType = PathFinderType.Stack;
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

            var stack = new Stack<int[]>();
            var parent = new Dictionary<string, int[]?>();
            var visited = new HashSet<string>();

            stack.Push(pos);
            parent[PositionKey(pos)] = null;
            visited.Add(PositionKey(pos));

            bool foundEnd = false;
            int[]? endPosition = null;

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current[0] == maze.End[0] && current[1] == maze.End[1])
                {
                    foundEnd = true;
                    endPosition = current;
                    break;
                }

                if (maze.MazeArray[current[0]][current[1]] != 1 &&
                    maze.MazeArray[current[0]][current[1]] != 2)
                {
                    maze.MazeArray[current[0]][current[1]] = 4;
                }

                foreach (var move in maze.moves)
                {
                    int newRow = current[0] + move[0];
                    int newCol = current[1] + move[1];
                    var newPos = new int[] { newRow, newCol };
                    var posKey = PositionKey(newPos);

                    if (maze.IsValidMove(newRow, newCol) && !visited.Contains(posKey))
                    {
                        int cellValue = maze.MazeArray[newRow][newCol];
                        if (cellValue == 0 || cellValue == 2)
                        {
                            stack.Push(newPos);
                            parent[posKey] = current;
                            visited.Add(posKey);
                        }
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
    }
}

            

