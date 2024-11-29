using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Apa_Project.Form1;

namespace Apa_Project
{
    internal class BFS
    {
        public class BFSPoint
        {
            public int x;
            public int y;

            public BFSPoint(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        };

        public class queueNode
        {
            public BFSPoint pt;

            public int dist;

            public queueNode(BFSPoint pt, int dist)
            {
                this.pt = pt;
                this.dist = dist;
            }
        };

        public static List<BFSPoint> BFSPath = new List<BFSPoint>();
        public static List<BFSPoint> BFSVisited = new List<BFSPoint>();
        public static BFSPoint[,] parent;
        public static Stopwatch BFSstopwatch = new Stopwatch();
        static bool isValid(int row, int col, int R, int C)
        {
            return (row >= 0) && (row < R) && (col >= 0) && (col < C);
        }

        static int[] rowNum = { -1,  0, 0, 1, -1, -1,  1, 1};
        static int[] colNum = {  0, -1, 1, 0, -1,  1, -1, 1};
        public static int BFSearch(int[,] mat, BFSPoint src, BFSPoint dest)
        {
            BFSstopwatch.Reset();
            BFSstopwatch.Start();
            int ROW = mat.GetLength(0);
            int COL = mat.GetLength(1);
            parent = new BFSPoint[ROW, COL];
            BFSPath.Clear();
            BFSVisited.Clear();

            if (mat[src.x, src.y] != 1 || mat[dest.x, dest.y] != 1)
                return -1;

            bool[,] visited = new bool[ROW, COL];

            visited[src.x, src.y] = true;

            Queue<queueNode> q = new Queue<queueNode>();

            queueNode s = new queueNode(src, 0);
            q.Enqueue(s);

            while (q.Count != 0)
            {
                queueNode curr = q.Peek();
                BFSPoint pt = curr.pt;
                BFSVisited.Add(pt);

                if (pt.x == dest.x && pt.y == dest.y)
                {
                    GetPath(parent, src, dest);
                    BFSstopwatch.Stop();
                    return curr.dist;
                }
                q.Dequeue();

                for (int i = 0; i < 8; i++)
                {
                    int row = pt.x + rowNum[i];
                    int col = pt.y + colNum[i];

                    if (isValid(row, col, ROW, COL) && mat[row, col] == 1 && !visited[row, col])
                    {
                        visited[row, col] = true;
                        queueNode Adjcell = new queueNode(new BFSPoint(row, col), curr.dist + 1);
                        parent[row, col] = new BFSPoint(pt.x, pt.y);
                        q.Enqueue(Adjcell);
                    }
                }
            }
            return -1;
        }
        public static void GetPath(BFSPoint[,] parent, BFSPoint source, BFSPoint destination)
        {
            BFSPoint current = destination;

            while (!(current.x == source.x && current.y == source.y))
            {
                BFSPath.Add(current);
                current = parent[current.x, current.y];
            }
            BFSPath.Add(source);
            BFSPath.Reverse();
        }
    }
}
