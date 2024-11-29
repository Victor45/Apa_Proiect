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

        public static List<DijPair> DPath = new List<DijPair>();
        public static List<DijPair> DVisited = new List<DijPair>();
        public static Stopwatch Dstopwatch = new Stopwatch();
        public static int DijPath(int[,] grid, DijPair source, DijPair destination)
        {
            Dstopwatch.Reset();
            Dstopwatch.Start();
            DPath.Clear();
            DVisited.Clear();

            if (source.Row == destination.Row && source.Col == destination.Col)
                return 0;

            int n = grid.GetLength(0);
            int m = grid.GetLength(1);

            SortedSet<(int, DijPair)> openList = new SortedSet<(int, DijPair)>(Comparer<(int, DijPair)>.Create((a, b) =>
            {
                int result = a.Item1.CompareTo(b.Item1);
                if (result == 0)
                    return a.Item2.Row != b.Item2.Row ? a.Item2.Row.CompareTo(b.Item2.Row) : a.Item2.Col.CompareTo(b.Item2.Col);
                return result;
            }));

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

            openList.Add((0, source));

            int[] dr = { -1, 0, 1, 0, -1, -1, 1, 1 };
            int[] dc = { 0, 1, 0, -1, -1, 1, -1, 1 };

            while (openList.Count > 0)
            {
                var current = openList.Min;
                openList.Remove(current);
                int dis = current.Item1;
                int r = current.Item2.Row;
                int c = current.Item2.Col;
                DVisited.Add(current.Item2);

                if (dis > dist[r, c])
                    continue;

                for (int i = 0; i < 8; i++)
                {
                    int newr = r + dr[i];
                    int newc = c + dc[i];

                    DijPair newrc = new DijPair(newr, newc);

                    if (newr >= 0 && newr < n && newc >= 0 && newc < m && grid[newr, newc] == 1)
                    {
                        if (dis + 1 < dist[newr, newc])
                        {
                            dist[newr, newc] = dis + 1;
                            parent[newr, newc] = new DijPair(r, c);
                            openList.Add((dist[newr, newc], newrc));

                            if (newr == destination.Row && newc == destination.Col)
                            {
                                GetPath(parent, source, destination);
                                Dstopwatch.Stop();
                                return dist[newr, newc];
                            }
                        }
                    }
                }
            }
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
            DPath.Add(source);
            DPath.Reverse();
        }
    }
}
