using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apa_Project
{
    internal class AStar
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

        public static List<Pair> Final = new List<Pair>();
        public static List<Pair> AStVisited = new List<Pair>();
        public static Stopwatch Aststopwatch = new Stopwatch();
        public static double AStdistance; 


        //static int[] rowNum = { -1, 0, 0, 1 };
        //static int[] colNum = { 0, -1, 1, 0 };
        public struct Cell
        {
            public int parent_i, parent_j;

            public double f, g, h;
        }
        public static void A_Star(int[,] grid, Pair src, Pair dest)
        {
            Aststopwatch.Reset();
            Aststopwatch.Start();
            Final.Clear();
            AStVisited.Clear(); 
            int ROW = grid.GetLength(0);
            int COL = grid.GetLength(1);

            if (!IsValid(src.first, src.second, ROW, COL) || !IsValid(dest.first, dest.second, ROW, COL))
            {
                Console.WriteLine("Source or destination is invalid");
                return;
            }

            if (!IsUnBlocked(grid, src.first, src.second) || !IsUnBlocked(grid, dest.first, dest.second))
            {
                Console.WriteLine("Source or the destination is blocked");
                return;
            }

            if (src.first == dest.first && src.second == dest.second)
            {
                Console.WriteLine("We are already at the destination");
                return;
            }

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

            int x = src.first, y = src.second;
            cellDetails[x, y].f = 0.0;
            cellDetails[x, y].g = 0.0;
            cellDetails[x, y].h = 0.0;
            cellDetails[x, y].parent_i = x;
            cellDetails[x, y].parent_j = y;

            //SortedSet<(double, Pair)> openList = new SortedSet<(double, Pair)>(
            //    Comparer<(double, Pair)>.Create((a, b) =>
            //    {
            //        int result = a.Item1.CompareTo(b.Item1);
            //        if (result == 0)
            //        {
            //            result = a.Item2.first.CompareTo(b.Item2.first);
            //            if (result == 0)
            //                result = a.Item2.second.CompareTo(b.Item2.second);
            //        }
            //        return result;
            //    }));

            SortedSet<(double, Pair)> openList = new SortedSet<(double, Pair)>(Comparer<(double, Pair)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));

            openList.Add((0.0, new Pair(x, y)));

            bool foundDest = false;

            while (openList.Count > 0)
            {
                (double f, Pair pair) p = openList.Min;
                openList.Remove(p);

                x = p.pair.first;
                y = p.pair.second;
                Pair visited;
                visited.first = x;
                visited.second = y;
                closedList[x, y] = true;
                AStVisited.Add(visited);

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        //if (Math.Abs(i) == Math.Abs(j))
                        //    continue;

                        //int newX = x + rowNum[i];
                        //int newY = y + colNum[i];
                        int newX = x + i;
                        int newY = y + j;

                        if (IsValid(newX, newY, ROW, COL))
                        {

                            if (IsDestination(newX, newY, dest))
                            {
                                cellDetails[newX, newY].parent_i = x;
                                cellDetails[newX, newY].parent_j = y;
                                AStdistance = cellDetails[x, y].g + 1.0;
                                TracePath(cellDetails, dest);
                                foundDest = true;
                                Aststopwatch.Stop();
                            return;
                            }

                            if (!closedList[newX, newY] && IsUnBlocked(grid, newX, newY))
                            {
                                double gNew = cellDetails[x, y].g + 1.0;
                                double hNew = CalculateHValue(newX, newY, dest);
                                double fNew = gNew + hNew;

                                if (cellDetails[newX, newY].f == double.MaxValue || cellDetails[newX, newY].f >= fNew)
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
            return grid[row, col] == 1;
        }
        public static bool IsDestination(int row, int col, Pair dest)
        {
            return (row == dest.first && col == dest.second);
        }
        public static double CalculateHValue(int row, int col, Pair dest)
        {
            return Math.Sqrt(Math.Pow(row - dest.first, 2) + Math.Pow(col - dest.second, 2));
            //return Math.Abs(row - dest.first) + Math.Abs(col - dest.second);
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
                Final.Add(new Pair(p.first, p.second));
                Path.Pop();
                Console.Write(" -> ({0},{1}) ", p.first, p.second);
            }
        }
    }
}
