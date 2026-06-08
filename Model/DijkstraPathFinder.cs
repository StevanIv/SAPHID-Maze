namespace Model
{
    public class DijkstraPathFinder : IPathFinder
    {
        PathFinderType _algType = PathFinderType.Dijkstra;
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

            var dist = new Dictionary<string, int>();
            var parent = new Dictionary<string, int[]?>();
            var visited = new HashSet<string>();
            var pq = new PriorityQueue<int[], int>();

            string startKey = Key(pos);
            dist[startKey] = 0;
            parent[startKey] = null;
            pq.Enqueue(pos, 0);

            bool foundEnd = false;
            int[]? endPosition = null;

            while (pq.Count > 0)
            {
                pq.TryDequeue(out int[]? currentPos, out int currentDist);
                if (currentPos == null) break;

                string currentKey = Key(currentPos);
                if (visited.Contains(currentKey)) continue;
                visited.Add(currentKey);

                if (currentPos[0] == maze.End[0] && currentPos[1] == maze.End[1])
                {
                    foundEnd = true;
                    endPosition = currentPos;
                    break;
                }

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
                    var newKey = Key(newPos);

                    if (!maze.IsValidMove(newRow, newCol) || visited.Contains(newKey))
                        continue;

                    int cellValue = maze.MazeArray[newRow][newCol];
                    if (cellValue != 0 && cellValue != 2)
                        continue;

                    int newDist = currentDist + 1;
                    if (!dist.TryGetValue(newKey, out int existingDist) || newDist < existingDist)
                    {
                        dist[newKey] = newDist;
                        parent[newKey] = currentPos;
                        pq.Enqueue(newPos, newDist);
                    }
                }
            }

            if (foundEnd && endPosition != null)
                ReconstructPath(parent, endPosition, visitedPositions);
            else
                visitedPositions.Enqueue(pos);
        }

        private static string Key(int[] pos) => $"{pos[0]},{pos[1]}";

        private static void ReconstructPath(Dictionary<string, int[]?> parent, int[] endPos, Queue<int[]> visitedPositions)
        {
            var path = new List<int[]>();
            int[]? current = endPos;

            while (current != null)
            {
                path.Add(current);
                var key = Key(current);
                if (parent.TryGetValue(key, out int[]? value))
                    current = value;
                else
                    current = null;
            }

            path.Reverse();

            foreach (var position in path)
                visitedPositions.Enqueue(position);
        }
    }
}
