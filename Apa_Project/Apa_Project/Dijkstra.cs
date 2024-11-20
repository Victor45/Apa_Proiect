using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apa_Project
{
    internal class Dijkstra
    {
        public struct DijPair
        {
            public int Row;
            public int Col;

            public DijPair(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }

        //public class Node
        //{
        //    // The coordinates of a cell
        //    public Pair pt;

        //    // cell's distance of from the source
        //    public int dist;

        //    public Node(Pair pt, int dist)
        //    {
        //        this.pt = pt;
        //        this.dist = dist;
        //    }
        //};

        public static List<DijPair> DPath = new List<DijPair>();
        public static List<DijPair> DVisited = new List<DijPair>();
        public static Stopwatch Dstopwatch = new Stopwatch();
        public static int DijPath(int[,] grid, DijPair source, DijPair destination)
        {
            Dstopwatch.Start(); // Start timing
            DPath.Clear();
            DVisited.Clear();
            //Node start = new Node(source, 0);
            // Edge Case: if the source is the same as the destination.
            if (source.Row == destination.Row && source.Col == destination.Col)
                return 0;

            int n = grid.GetLength(0);
            int m = grid.GetLength(1);

            // Create a priority queue (min-heap) for Dijkstra's algorithm.
            SortedSet<(int, DijPair)> openList = new SortedSet<(int, DijPair)>(Comparer<(int, DijPair)>.Create((a, b) =>
            {
                int result = a.Item1.CompareTo(b.Item1);
                if (result == 0)
                    return a.Item2.Row != b.Item2.Row ? a.Item2.Row.CompareTo(b.Item2.Row) : a.Item2.Col.CompareTo(b.Item2.Col);
                return result;
            }));
            //PriorityQueue<Node, int> openList = new PriorityQueue<Node, int>();

            // Distance matrix, initialized to a large value.
            int[,] dist = new int[n, m];
            DijPair[,] parent = new DijPair[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    dist[i, j] = int.MaxValue;
                }
            }

            dist[source.Row, source.Col] = 0;

            // Push the source into the priority queue with a distance of 0.
            openList.Add((0, source));

            // Directions for moving in the grid (up, right, down, left).
            int[] dr = { -1, 0, 1, 0, -1, -1, 1, 1 };
            int[] dc = { 0, 1, 0, -1, -1, 1, -1, 1 };

            while (openList.Count > 0)
            {
                var current = openList.Min;
                openList.Remove(current);
                int dis = current.Item1;
                int r = current.Item2.Row;
                int c = current.Item2.Col;
                //Pair visited;
                //visited.Row = r;
                //visited.Col = c;
                DVisited.Add(current.Item2);

                // If the current distance is greater than the recorded distance, skip.
                if (dis > dist[r, c])
                    continue;

                // Check all 4 adjacent nodes.
                for (int i = 0; i < 8; i++)
                {
                    int newr = r + dr[i];
                    int newc = c + dc[i];
                    //for (int i = -1; i <= 1; i++)
                    //{
                    //    for (int j = -1; j <= 1; j++)
                    //    {
                    //        if (i == 0 && j == 0)// || (i == -1 && j == -1) || (i == -1 && j == 1)||(i == 1 && j == -1)|| (i == 1 && j == 1))
                    //            continue;

                    //int newr = r + i;
                    //    int newc = c + j;
                        DijPair newrc = new DijPair(newr, newc);

                        // Check if the new cell is within bounds and passable.
                        if (newr >= 0 && newr < n && newc >= 0 && newc < m && grid[newr, newc] == 1)
                        {
                            // If a shorter path to the adjacent cell is found.
                            if (dis + 1 < dist[newr, newc])
                            {
                                dist[newr, newc] = dis + 1;
                                //Node nrc = new Node(newrc, dist[newr, newc]);
                                parent[newr, newc] = new DijPair(r, c);
                                openList.Add((dist[newr, newc], newrc));

                                // If the destination is reached, return the distance.
                                if (newr == destination.Row && newc == destination.Col)
                                {
                                    GetPath(parent, source, destination);
                                    Dstopwatch.Stop();
                                    return dist[newr, newc];
                                }
                            }
                        }
                    //}
                }
            }
            // If no path is found, return -1.
            return -1;
        }

        public static void GetPath(DijPair[,] parent, DijPair source, DijPair destination)
        {
            DijPair current = destination;

            while (!(current.Row == source.Row && current.Col == source.Col))
            {
                DPath.Add(current);
                current = parent[current.Row, current.Col];
            }
            DPath.Add(source); // Add the source to the path

            DPath.Reverse(); // Reverse the path to get it from source to destination

        }
    }
}
