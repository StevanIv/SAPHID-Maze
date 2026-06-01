
using Model;
using View;
using Controller;



class Program
{
    private static void StartMenu (Maze maze, MazeView view)
    {
      Console.ForegroundColor = ConsoleColor.DarkRed;
      Console.BackgroundColor = ConsoleColor.White;
      MenuView.DisplayMenu();
      view.DisplayMaze(maze); 
    }

    static void Main()
    {
        //-----------constants:------------
        const int rows = 25, cols = 2*rows;
        const int timeInterval = 400;
        //---------------------------------
        
        //Predefined maze:
        //Maze maze = new Maze(mazeText); //to use the string above;
        //OR
        //Maze maze = new Maze(MazeGrids.mazeText);
        //OR
        //Maze maze = new Maze(-1, -1);
        //OR
        //Maze maze = new Maze(false);

        Maze maze = new Maze(rows, cols);
        MazeView view = new MazeView();

        MenuController menuController = new MenuController(maze, view, timeInterval);
        ConsoleKey key;
        bool resp = true;

        //----Refresh for visualization reason----
        int i = 0;
        while (i <= 4)
        {
          StartMenu(maze, view);           
          i++;
        }
        //----------------------------------------

        while (resp)
        {
          StartMenu(maze, view); 
          key = Console.ReadKey(true).Key;
          resp = menuController.Run(key);
        }
    }
}

