using System;
using System.Collections.Generic;
using System.Numerics;

public class AStarSearch
{
    public struct Pair
    {
        public int first, second;

        public Pair(int x, int y)
        {
            first = x;
            second = y;
        }
    }

    public struct FinPair
    {
        public int i, j;
        public FinPair(int x, int y)
        {
            i = x;
            j = y;
        }
    }

    public static List<FinPair> Final = new List<FinPair>();
    public struct Cell
    {
        public int parent_i, parent_j;
        // f = g + h
        public double f, g, h;
    }
    public static void AStar(int[,] grid, Pair src, Pair dest)
    {
        int ROW = grid.GetLength(0);
        int COL = grid.GetLength(1);
        // If the source or destination is out of range
        if (!IsValid(src.first, src.second, ROW, COL) || !IsValid(dest.first, dest.second, ROW, COL))
        {
            Console.WriteLine("Source or destination is invalid");
            return;
        }
        // Either the source or the destination is blocked
        if (!IsUnBlocked(grid, src.first, src.second) || !IsUnBlocked(grid, dest.first, dest.second))
        {
            Console.WriteLine("Source or the destination is blocked");
            return;
        }
        // If the destination cell is the same as the source cell
        if (src.first == dest.first && src.second == dest.second)
        {
            Console.WriteLine("We are already at the destination");
            return;
        }
        // For the cells already visited and already with calculated cost
        bool[,] closedList = new bool[ROW, COL];

        Cell[,] cellDetails = new Cell[ROW, COL];

        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                cellDetails[i, j].f = double.MaxValue;
                cellDetails[i, j].g = double.MaxValue;
                cellDetails[i, j].h = double.MaxValue;
                cellDetails[i, j].parent_i = -1;
                cellDetails[i, j].parent_j = -1;
            }
        }
        // Starting node
        int x = src.first, y = src.second;
        cellDetails[x, y].f = 0.0;
        cellDetails[x, y].g = 0.0;
        cellDetails[x, y].h = 0.0;
        cellDetails[x, y].parent_i = x;
        cellDetails[x, y].parent_j = y;

        SortedSet<(double, Pair)> openList = new SortedSet<(double, Pair)>(
            Comparer<(double, Pair)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));

        // Adding the starting cell in the list with f=0
        openList.Add((0.0, new Pair(x, y)));

        //Destination verifier
        bool foundDest = false;

        while (openList.Count > 0)
        {
            (double f, Pair pair) p = openList.Min;
            openList.Remove(p);

            // Adding the cell to the closed list
            x = p.pair.first;
            y = p.pair.second;
            closedList[x, y] = true;

            //Generating all the 8 successors/neighbors of the cell
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int newX = x + i;
                    int newY = y + j;

                    // If this successor is a valid cell
                    if (IsValid(newX, newY, ROW, COL))
                    {

                        // If the destination cell is the same as the current successor
                        if (IsDestination(newX, newY, dest))
                        {
                            cellDetails[newX, newY].parent_i = x;
                            cellDetails[newX, newY].parent_j = y;
                            Console.WriteLine("The destination cell is found");
                            TracePath(cellDetails, dest);
                            foundDest = true;
                            return;
                        }

                        // If the successor is already on the closed list or if it is blocked ignore it.
                        if (!closedList[newX, newY] && IsUnBlocked(grid, newX, newY))
                        {
                            double gNew = cellDetails[x, y].g + 1.0;
                            double hNew = CalculateHValue(newX, newY, dest);
                            double fNew = gNew + hNew;
                            // If it isn’t on the open list, add it to
                            // the open list. Make the current square
                            // the parent of this square. Record the
                            // f, g, and h costs of the square cell
                            if (cellDetails[newX, newY].f == double.MaxValue || cellDetails[newX, newY].f > fNew)
                            {
                                openList.Add((fNew, new Pair(newX, newY)));

                                cellDetails[newX, newY].f = fNew;
                                cellDetails[newX, newY].g = gNew;
                                cellDetails[newX, newY].h = hNew;
                                cellDetails[newX, newY].parent_i = x;
                                cellDetails[newX, newY].parent_j = y;
                            }
                        }
                    }
                }
            }
        }

        if (!foundDest)
            Console.WriteLine("Failed to find the Destination Cell");
    }

    public static bool IsValid(int row, int col, int ROW, int COL)
    {
        return (row >= 0) && (row < ROW) && (col >= 0) && (col < COL);
    }
    public static bool IsUnBlocked(int[,] grid, int row, int col)
    {
        return grid[row, col] == 0;
    }
    public static bool IsDestination(int row, int col, Pair dest)
    {
        return (row == dest.first && col == dest.second);
    }
    public static double CalculateHValue(int row, int col, Pair dest)
    {
        return Math.Sqrt(Math.Pow(row - dest.first, 2) + Math.Pow(col - dest.second, 2));
    }
    public static void TracePath(Cell[,] cellDetails, Pair dest)
    {
        Console.WriteLine("\nThe Path is ");
        int ROW = cellDetails.GetLength(0);
        int COL = cellDetails.GetLength(1);

        int row = dest.first;
        int col = dest.second;

        Stack<Pair> Path = new Stack<Pair>();

        while (!(cellDetails[row, col].parent_i == row && cellDetails[row, col].parent_j == col))
        {
            Path.Push(new Pair(row, col));
            int temp_row = cellDetails[row, col].parent_i;
            int temp_col = cellDetails[row, col].parent_j;
            row = temp_row;
            col = temp_col;
        }

        Path.Push(new Pair(row, col));
        int i = 0;
        while (Path.Count > 0)
        {
            Pair p = Path.Peek();
            Final.Add(new FinPair(p.first, p.second));
            Path.Pop();
            Console.Write(" -> ({0},{1}) ", p.first, p.second);
        }
    }

    public static void Main(string[] args)
    {

        int[,] grid =
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {0, 0, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 0, 1, 1, 0, 0, 1, 0, 1, 0},
            {1, 0, 0, 1, 1, 0, 0, 0, 1, 0},
            {1, 0, 1, 0, 1, 1, 1, 0, 0, 1},
            {1, 0, 0, 1, 1, 1, 0, 1, 1, 1},
            {1, 0, 1, 1, 0, 1, 0, 0, 1, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        Pair src = new Pair(1, 0);

        Pair dest = new Pair(7, 9);

        AStar(grid, src, dest);

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);
        int[,] finalpath = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                finalpath[i, j] = 1;
            }
        }

        foreach (var pair in Final)
        {
            finalpath[pair.i, pair.j] = 0;
        }

        for (int i = 0; i < rows; i++)
        {
            Console.WriteLine();
            for (int j = 0; j < cols; j++)
            {
                Console.Write(finalpath[i, j]);
            }
        }

    }
}
