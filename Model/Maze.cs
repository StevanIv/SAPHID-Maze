
using System.Data;

namespace Model
{
    public enum MazeGenerationMode
    {
        Default,
        Recursive
    }

    public class Maze
    {
        public int[][] MazeArray { get; private set; }
        public int[,] MazeMDArray { get; private set; }
        public int[] Begin { get; private set; }
        public int[] End { get; private set; }

        public readonly int[][] moves = {           
            new int[] {  1,  0 },  //down
            new int[] { -1,  0 },  //up
            new int[] {  0, -1 },  //left
            new int[] {  0,  1 },  //right
            };
        
        public Maze() => GenerateFromText(MazeGrids.mazeText);
        public Maze(bool automatic = true) => GenerateFromText(MazeGrids.mazeText);
        public Maze(int rows, int cols) => GenerateMaze(rows, cols);
        public Maze(int rows, int cols, MazeGenerationMode mode)
        {
            if (mode == MazeGenerationMode.Recursive)
                GenerateMaze(rows, cols);
            else
                GenerateFromText(MazeGrids.mazeText);
        }
        public Maze(string lines) => GenerateFromText(lines);

        void GenerateFromText(string lines){
            MazeArray = ToMazeArray(lines);
            MazeMDArray = ToMazeMDArray(lines);
        }

        void GenerateMaze(int rows = 20, int cols = 40)
        {
            if(rows < 4 || cols < 4) {rows = 20; cols = 40;}
            if(rows % 2 != 0) {rows++;}
            if(cols % 2 != 0) {cols++;}

            var mazeGrid = CreateFilledGrid(rows, cols, -1);
            var visited = new bool[rows, cols];

            int startRow = 1;
            int startCol = 1;

            CarvePassagesRecursive(mazeGrid, visited, startRow, startCol);

            int[] begin = new int[] { startRow, startCol };
            int[] end = FindFarthestPassage(mazeGrid, begin);

            mazeGrid[begin[0]][begin[1]] = 1;
            mazeGrid[end[0]][end[1]] = 2;

            ApplyGeneratedGrid(mazeGrid, begin, end);
        }

        private static int[][] CreateFilledGrid(int rows, int cols, int fillValue)
        {
            var grid = new int[rows][];

            for (int rowIdx = 0; rowIdx < rows; rowIdx++)
            {
                grid[rowIdx] = new int[cols];

                for (int colIdx = 0; colIdx < cols; colIdx++)
                {
                    grid[rowIdx][colIdx] = fillValue;
                }
            }

            return grid;
        }

        private void CarvePassagesRecursive(int[][] grid, bool[,] visited, int row, int col)
        {
            visited[row, col] = true;
            grid[row][col] = 0;

            var directions = new (int dRow, int dCol)[]
            {
                (-2, 0),
                (2, 0),
                (0, -2),
                (0, 2)
            };

            foreach (var (dRow, dCol) in directions.OrderBy(_ => Random.Shared.Next()))
            {
                int nextRow = row + dRow;
                int nextCol = col + dCol;

                if (nextRow <= 0 || nextRow >= grid.Length - 1 ||
                    nextCol <= 0 || nextCol >= grid[0].Length - 1 ||
                    visited[nextRow, nextCol])
                {
                    continue;
                }

                grid[row + dRow / 2][col + dCol / 2] = 0;
                CarvePassagesRecursive(grid, visited, nextRow, nextCol);
            }
        }

        private static int[] FindFarthestPassage(int[][] grid, int[] begin)
        {
            var queue = new Queue<int[]>();
            var distance = new int[grid.Length][];

            for (int rowIdx = 0; rowIdx < grid.Length; rowIdx++)
            {
                distance[rowIdx] = new int[grid[rowIdx].Length];

                for (int colIdx = 0; colIdx < grid[rowIdx].Length; colIdx++)
                {
                    distance[rowIdx][colIdx] = -1;
                }
            }

            queue.Enqueue(begin);
            distance[begin[0]][begin[1]] = 0;

            int[] farthest = begin;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                int currentDistance = distance[current[0]][current[1]];

                if (currentDistance >= distance[farthest[0]][farthest[1]])
                {
                    farthest = current;
                }

                foreach (var move in new[]
                {
                    new int[] { 1, 0 },
                    new int[] { -1, 0 },
                    new int[] { 0, 1 },
                    new int[] { 0, -1 }
                })
                {
                    int nextRow = current[0] + move[0];
                    int nextCol = current[1] + move[1];

                    if (nextRow < 0 || nextCol < 0 ||
                        nextRow >= grid.Length || nextCol >= grid[0].Length ||
                        grid[nextRow][nextCol] == -1 ||
                        distance[nextRow][nextCol] != -1)
                    {
                        continue;
                    }

                    distance[nextRow][nextCol] = currentDistance + 1;
                    queue.Enqueue(new int[] { nextRow, nextCol });
                }
            }

            return farthest;
        }

        private void ApplyGeneratedGrid(int[][] grid, int[] begin, int[] end)
        {
            MazeArray = grid;
            Begin = begin;
            End = end;

            MazeMDArray = new int[grid.Length, grid[0].Length];

            for (int rowIdx = 0; rowIdx < grid.Length; rowIdx++)
            {
                for (int colIdx = 0; colIdx < grid[rowIdx].Length; colIdx++)
                {
                    MazeMDArray[rowIdx, colIdx] = grid[rowIdx][colIdx];
                }
            }
        }

        int[][] ToMazeArray(string maze)
        {
            // substrings from the maze string
            var arrayLines = maze.Split(new char[] { '.', '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            int[][] outArray = new int[arrayLines.Length][];

            for (var rowIdx = 0; rowIdx < arrayLines.Length; rowIdx++)
            {
                var line = arrayLines[rowIdx];
                // row array:
                var row = new int[line.Length];
                for (int colIdx = 0; colIdx < line.Length; colIdx++)
                {
                    //from chars to integers
                    switch (line[colIdx])
                    {
                        case 'x':
                            row[colIdx] = -1;  //walls
                            break;
                        case '1':
                            row[colIdx] = 1;   //begin
                            Begin = new int[] { rowIdx, colIdx };
                            break;
                        case '2':
                            row[colIdx] = 2;   //end 
                            End = new int[] { rowIdx, colIdx };
                            break;
                        default:
                            row[colIdx] = 0;   //not visited
                            break;
                    }
                }
                // row in the output jagged array.
                outArray[rowIdx] = row;
            }

            return outArray;
            
        }

        int[,] ToMazeMDArray(string maze)
        {
            // substrings from the maze string
            var arrayLines = maze.Split(new char[] { '.', '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            var lineLength = 0;
            if (arrayLines != null && arrayLines.Length > 0)
                lineLength = arrayLines[0].Length;
            else
            throw new Exception($"Maze incorrect");
            
            for (var rowIdx = 0; arrayLines != null && rowIdx < arrayLines.Length; rowIdx++)
            {
                var line = arrayLines[rowIdx];
                if (arrayLines[rowIdx] == null || line.Length != lineLength)
                    throw new Exception($"Not same line length for rows in maze:\n at row 0: {lineLength}, at row {rowIdx}: {line.Length}");
            }
            
            int[,] outArray = new int[arrayLines.Length, lineLength];

            for (var rowIdx = 0; rowIdx < arrayLines.Length; rowIdx++)
            {
                var line = arrayLines[rowIdx];

                for (int colIdx = 0; colIdx < line.Length; colIdx++)
                {
                    //from chars to integers
                    switch (line[colIdx])
                    {
                        case 'x':
                            outArray[rowIdx, colIdx] = -1;  //walls
                            break;
                        case '1':
                            outArray[rowIdx, colIdx] = 1;   //begin
                            Begin = new int[] { rowIdx, colIdx };
                            break;
                        case '2':
                            outArray[rowIdx, colIdx] = 2;   //end 
                            End = new int[] { rowIdx, colIdx };
                            break;
                        default:
                            outArray[rowIdx, colIdx] = 0;   //not visited
                            break;
                    }
                }
            }
            return outArray;
        }

        static int CountNotVisited(int[][] maze)
        {
            int cnt = 0;
            if (maze != null && maze.Length > 0)
            {
                for (int rowIdx = 0; rowIdx < maze.Length; rowIdx++)
                {
                    for (int colIdx = 0; maze[rowIdx] != null && colIdx < maze[rowIdx].Length; colIdx++)
                    {
                        cnt = maze[rowIdx][colIdx] == 0 ? cnt + 1 : cnt;
                    }
                }
            }
            return cnt;
        }

        public int CountNotVisited() => CountNotVisited(MazeArray);

        static bool IsValidPos(int[][] array, int newRow, int newColumn)
        {
            // ... Ensure position is within the array bounds.
            /*
            if (newRow < 0) return false;
            if (newColumn < 0) return false;
            if (newRow >= array.Length) return false;
            if (newColumn >= array[newRow].Length) return false;
            return true;
            */
            return !(newRow < 0)
                    && !(newColumn < 0)
                    && !(newRow >= array.Length)
                    && !(newColumn >= array[newRow].Length);
        }
        
        // Make sure the position is within the maze array bounds.
        // no walls
        public bool IsValidMove(int newRow, int newColumn) => 
            IsValidPos(MazeArray, newRow, newColumn) &&
            !(MazeArray[newRow][newColumn] == -1); //no walls 

        //Marking strategy
        public bool IsValidMove(int newRow, int newColumn, bool notVisited = true)
        {
            // Make sure the position is within the maze array bounds.
            // no walls, not yet visited ? (flag notVisited: false)
            return notVisited ?
                    IsValidPos(MazeArray, newRow, newColumn) &&
                    !(MazeArray[newRow][newColumn] == -1)  //no walls, but already visited -> ok
                    :
                    IsValidPos(MazeArray, newRow, newColumn) &&
                    !(MazeArray[newRow][newColumn] == -1 || MazeArray[newRow][newColumn] == 4); //no walls, not yet visited 
        }
        
    }

    public static class MazeGrids
    {
      public static string mazeText = @"
xxxxxx1xxxxxxxxxxxxxxxxxxxxxxx.
 x   x   x                    .
xx2x xxx   x xxxxxxxx    x xx .
x  x xxxxxxx xxxxxxxxxxxxx xxx.
 x x xx      x                .
x  x xx xxxxx  x xxxx xxxxx  x.
xx    x xxx   xx xxx  xxx   xx.
xxx   xxx   x xxxx   xx   x xx.
xx     xx   x xxxx   xx   x xx.
xxxx    xxxxx xx xxxx xxxxx xx.
xx            xx            xx.";
    }
}
