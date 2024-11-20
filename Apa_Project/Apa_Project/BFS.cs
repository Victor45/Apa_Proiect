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
        // To store matrix cell coordinates
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


        // A Data Structure for queue used in BFS
        public class queueNode
        {
            // The coordinates of a cell
            public BFSPoint pt;

            // cell's distance of from the source
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
        // check whether given cell (row, col) 
        // is a valid cell or not.
        static bool isValid(int row, int col, int R, int C)
        {
            // return true if row number and 
            // column number is in range
            return (row >= 0) && (row < R) && (col >= 0) && (col < C);
        }

        // These arrays are used to get row and column
        // numbers of 4 neighbours of a given cell
        static int[] rowNum = { -1,  0, 0, 1, -1, -1,  1, 1};
        static int[] colNum = {  0, -1, 1, 0, -1,  1, -1, 1};

        // function to find the shortest path between
        // a given source cell to a destination cell.
        public static int BFSearch(int[,] mat, BFSPoint src, BFSPoint dest)
        {
            BFSstopwatch.Reset();
            BFSstopwatch.Start();
            int ROW = mat.GetLength(0);
            int COL = mat.GetLength(1);
            parent = new BFSPoint[ROW, COL];
            BFSPath.Clear();
            BFSVisited.Clear();
            // check source and destination cell
            // of the matrix have value 1
            if (mat[src.x, src.y] != 1 || mat[dest.x, dest.y] != 1)
                return -1;

            bool[,] visited = new bool[ROW, COL];

            // Mark the source cell as visited
            visited[src.x, src.y] = true;

            // Create a queue for BFS
            Queue<queueNode> q = new Queue<queueNode>();

            // Distance of source cell is 0
            queueNode s = new queueNode(src, 0);
            q.Enqueue(s); // Enqueue source cell

            // Do a BFS starting from source cell
            while (q.Count != 0)
            {
                queueNode curr = q.Peek();
                BFSPoint pt = curr.pt;
                BFSVisited.Add(pt);

                // If we have reached the destination cell,
                // we are done
                if (pt.x == dest.x && pt.y == dest.y)
                {
                    GetPath(parent, src, dest);
                    BFSstopwatch.Stop();
                    return curr.dist;
                }
                // Otherwise dequeue the front cell 
                // in the queue and enqueue
                // its adjacent cells
                q.Dequeue();

                for (int i = 0; i < 8; i++)
                {
                    //for (int i = -1; i <= 1; i++)
                    //{
                    //    for (int j = -1; j <= 1; j++)
                    //    {
                    //        if (i == 0 && j == 0)// || (i == -1 && j == -1) || (i == -1 && j == 1)||(i == 1 && j == -1)|| (i == 1 && j == 1))
                    //            continue;

                    int row = pt.x + rowNum[i];
                        int col = pt.y + colNum[i];
                        //int row = pt.x + i;
                        //int col = pt.y + j;

                        // if adjacent cell is valid, has path 
                        // and not visited yet, enqueue it.
                        if (isValid(row, col, ROW, COL) && mat[row, col] == 1 && !visited[row, col])
                        {
                            // mark cell as visited and enqueue it
                            visited[row, col] = true;
                            queueNode Adjcell = new queueNode(new BFSPoint(row, col), curr.dist + 1);
                            parent[row, col] = new BFSPoint(pt.x, pt.y);
                            q.Enqueue(Adjcell);
                        }
                    //}
                }
            }

            // Return -1 if destination cannot be reached
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
            BFSPath.Add(source); // Add the source to the path
            BFSPath.Reverse(); // Reverse the path to get it from source to destination

        }
    }
}
