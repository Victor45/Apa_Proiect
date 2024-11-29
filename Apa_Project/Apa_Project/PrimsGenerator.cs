using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Apa_Project.AStar;

namespace Apa_Project
{
    public struct Cell
    {
        public int i;
        public int j;
        bool state;
        public Cell(int i, int j)
        {
            this.i = i;
            this.j = j;
            this.state = false;
        }
    }

    public struct RemovableWall
    {
        public int i, j;
        public RemovableWall(int x, int y)
        {
            i = x;
            j = y;
        }
    }

    public class PrimsGenerator
    {
        Random random = new Random();
        int rows = 30;
        int cols = 51;
        //public bool[,] maze;
        public int[,] maze;
        List<Cell> frontiers;
        public PrimsGenerator()
        {
            //maze = new bool[rows, cols];
            maze = new int[rows, cols];
            frontiers = new List<Cell>();
        }

        public PrimsGenerator(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            //maze = new bool[rows, cols];
            maze = new int[rows, cols];
            frontiers = new List<Cell>();
        }

        public void GenerateMaze()
        {
            int i = random.Next(1, rows - 1);
            int j = random.Next(1, cols - 1);
            if (rows % 2 == 1)
            {
                do
                {
                    i = random.Next(1, rows - 1);
                } while (i % 2 == 0);
            }
            if (cols % 2 == 1)
            {
                do
                {
                    j = random.Next(1, cols - 1);
                } while (j % 2 == 0);
            }

            Cell randomCell = new Cell(i, j);
            SetCellAsPassage(randomCell);
            FindAndSaveCellFrontiers(randomCell);
            while (frontiers.Count > 0)
            {
                int randomFrontier = random.Next(0, frontiers.Count);
                Cell cell = frontiers[randomFrontier];
                SetCellAsPassage(cell);
                FindAndSaveCellFrontiers(cell);
                MakePassge(cell);

            }
            RemoveWall();
        }
        public void SetCellAsPassage(Cell cell)
        {
            if (cell.i == 0 || cell.i == rows - 1) return;
            if (cell.j == 0 || cell.j == cols - 1) return;
            maze[cell.i, cell.j] = 1;
        }
        public void FindAndSaveCellFrontiers(Cell cell)
        {
            if (cell.i == 0 || cell.i == rows - 1) return;
            if (cell.j == 0 || cell.j == cols - 1) return;
            // top frontier
            if (cell.i - 2 >= 0 && maze[cell.i - 2, cell.j] == 0) 
            {
                Cell frontier = new Cell(cell.i - 2, cell.j);
                if (!frontiers.Contains(frontier))
                {
                    frontiers.Add(frontier);
                }
            }
            // right frontier
            if (cell.j + 2 < cols && maze[cell.i, cell.j + 2] == 0) 
            {
                Cell frontier = new Cell(cell.i, cell.j + 2);
                if (!frontiers.Contains(frontier))
                {
                    frontiers.Add(frontier);
                }
            }
            // bottom frontier
            if (cell.i + 2 < rows && maze[cell.i + 2, cell.j] == 0) 
            {
                Cell frontier = new Cell(cell.i + 2, cell.j);
                if (!frontiers.Contains(frontier))
                {
                    frontiers.Add(frontier);
                }
            }
            // left frontier
            if (cell.j - 2 >= 0 && maze[cell.i, cell.j - 2] == 0) 
            {
                Cell frontier = new Cell(cell.i, cell.j - 2);
                if (!frontiers.Contains(frontier))
                {
                    frontiers.Add(frontier);
                }
            }
        }
        public void MakePassge(Cell cell)
        {
            List<int> visited = new List<int>();
            int frontier = random.Next(1, 5);
            visited.Add(frontier);
            while (true)
            {
                // top frontier
                if (frontier == 1 && 0 <= cell.i - 2 && maze[cell.i - 2, cell.j] == 1) 
                {
                    maze[cell.i - 1, cell.j] = 1; 
                    
                    frontiers.Remove(cell);

                    return;
                }
                // right frontier
                if (frontier == 2 && cell.j + 2 < cols && maze[cell.i, cell.j + 2] == 1) 
                {
                    maze[cell.i, cell.j + 1] = 1; 
                    
                    frontiers.Remove(cell);

                    return;
                }
                // bottom frontier
                if (frontier == 3 && cell.i + 2 < rows && maze[cell.i + 2, cell.j] == 1) 
                {
                    maze[cell.i + 1, cell.j] = 1; 
                    
                    frontiers.Remove(cell);

                    return;
                }
                // left frontier
                if (frontier == 4 && 0 <= cell.j - 2 && maze[cell.i, cell.j - 2] == 1) 
                {
                    maze[cell.i, cell.j - 1] = 1; 
                    
                    frontiers.Remove(cell);

                    return;
                }

                while (visited.Contains(frontier))
                {
                    frontier = random.Next(1, 5);
                }
                visited.Add(frontier);
            }
        }
        public void PrintMaze()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (maze[i, j] == 1)
                    {
                        Console.Write(" ");
                    }
                    else Console.Write("#");
                }
                Console.WriteLine();
            }
        }

        public void RemoveWall()
        {
            List<RemovableWall> removableWalls = new List<RemovableWall>();
            int rwcount = 0;
            for (int i = 1; i < rows - 1; i++)
            {
                for (int j = 1; j < cols - 1; j++)
                {
                    if (maze[i, j] == 0) 
                    {
                        if (maze[i, j - 1] == 0 && maze[i, j + 1] == 0 && maze[i - 1, j] == 1 && maze[i + 1, j] == 1)
                        {
                            removableWalls.Add(new RemovableWall(i, j));
                            rwcount++;
                        }
                        if (maze[i, j - 1] == 1 && maze[i, j + 1] == 1 && maze[i - 1, j] == 0 && maze[i + 1, j] == 0)
                        {
                            removableWalls.Add(new RemovableWall(i, j));
                            rwcount++;
                        }
                    }
                }
            }
            int r, c;
            int wallstoremove = rwcount * 15 / 100;
            int wallcount = 0, wallindex;
            while (wallcount < wallstoremove)
            {
                wallindex = random.Next(rwcount);
                r = removableWalls[wallindex].i;
                c = removableWalls[wallindex].j;
                maze[r, c] = 1;
                wallcount++;
                rwcount--;
                removableWalls.RemoveAt(wallindex);
            }
            if (maze[1, 1] == 1)
            {
                maze[1, 0] = 1;
            }
            if (maze[rows - 2, cols - 2] == 1)
            {
                maze[rows - 2, cols - 1] = 1;
            }
        }
        
    }
}