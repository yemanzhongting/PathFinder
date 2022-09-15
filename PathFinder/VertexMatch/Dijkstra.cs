using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Where;
namespace VertexMatch
{
    //顶点
    class Vertex
    {
        public int label;
        public bool isInTree;
        public Vertex(int code)
        {
            label = code;
            isInTree = false;
        }

        public override string ToString()
        {
            return $"{label},{isInTree}";
        }
    }
    /// <summary>
    /// 跟踪顶点与用于计算最短路径的原始顶点之间的关系。
    /// </summary>
    class DistOriginal
    {
        public double distance;
        public int parentVert;
        public DistOriginal(int pv, double dist)
        {
            distance = dist;
            parentVert = pv;
        }

        public override string ToString()
        {
            return $"{distance},{parentVert}";
        }
    }

    /// <summary>
    /// 利用Dijkstra进行最短路径计算
    /// </summary>
    public class Dijkstra
    {
        int MaxVerts = 20;
        double Infinity = 1000000;
        Vertex[] Verts;
        double[,] AdjMat;
        DistOriginal[] Path;

        int nVerts;
        int nTree;
        int currentVert;
        double startToCurrent;
        public Dijkstra(int maxVerts)
        {
            MaxVerts = maxVerts;

            Verts = new Vertex[MaxVerts];
            AdjMat = new double[MaxVerts, MaxVerts];
            nVerts = 0;
            nTree = 0;
            for (int j = 0; j <= MaxVerts - 1; j++)
                for (int k = 0; k <= MaxVerts - 1; k++)
                    AdjMat[j, k] = Infinity;
            Path = new DistOriginal[MaxVerts];
        }
        public void AddVertex(int lab)
        {
            Verts[nVerts] = new Vertex(lab);
            nVerts++;
        }

        int IndexOfVert(int lab)
        {
            int index = -1;
            for (int i = 0; i < MaxVerts; i++)
            {
                if (Verts[i].label == lab)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public void AddEdge(int startCode, int endCode, double weight)
        {
            int theStart = IndexOfVert(startCode);
            int theEnd = IndexOfVert(endCode);
            AdjMat[theStart, theEnd] = weight;

            AdjMat[theEnd, theStart] = weight; //Add
        }

        public List<Node> FindPath(int startCode, int endCode)
        {
            FindPath();
            return ShowPath(startCode, endCode);
        }

        public void FindPath()
        {
            int startTree = 0;
            Verts[startTree].isInTree = true;
            nTree = 1;
            for (int j = 0; j < nVerts; j++)
            {
                double tempDist = AdjMat[startTree, j];
                Path[j] = new DistOriginal(startTree, tempDist);
            }
            while (nTree < nVerts)
            {
                int indexMin = GetMin();
                double minDist = Path[indexMin].distance;
                currentVert = indexMin;
                startToCurrent = Path[indexMin].distance;
                Verts[currentVert].isInTree = true;
                nTree++;
                AdjustShortPath();
            }
        }

        int GetMin()
        {
            double minDist = Infinity;
            int indexMin = 0;
            for (int j = 1; j < nVerts; j++)
            {
                if (!(Verts[j].isInTree)
                    && Path[j].distance < minDist)
                {
                    minDist = Path[j].distance;
                    indexMin = j;
                }
            }
            return indexMin;
        }

        void AdjustShortPath()
        {
            int column = 1;
            while (column < nVerts)
            {
                if (Verts[column].isInTree)
                {
                    column++;
                }
                else
                {
                    double currentToFring = AdjMat[currentVert, column];
                    double startToFringe = startToCurrent + currentToFring;

                    double pathDist = Path[column].distance;
                    if (startToFringe < pathDist)
                    {
                        Path[column].parentVert = currentVert;
                        Path[column].distance = startToFringe;
                    }
                    column++;
                }
            }
        }

        public string DisplayPaths()
        {
            string res = "";
            for (int j = 0; j <= nVerts - 1; j++)
            {
                res+=Verts[j].label + "=";
                if (Path[j].distance > Infinity - 1)
                   res+="inf";
                else
                   res+=Path[j].distance;
                int parent = Verts[Path[j].parentVert].label;
                res+="(" + parent + ")\n ";
            }
            return res;
        }

        //List<Node> ShowPath()
        //{
        //    var res = new List<Node>();
        //    for (int i = 0; i < nVerts; i++)
        //    {
        //        var node = new Node();
        //        node.Code = Verts[i].label;
        //        node.Distance = Path[i].distance;
        //        int parent = Verts[Path[i].parentVert].label;
        //        node.Name = parent.ToString();
        //        res.Add(node);
        //    }
        //    return res;
        //}

        List<Node> ShowPath(int startCode, int endCode)
        {
            var res = new List<Node>();

            Node node=new Node();

            try
            {
                int startIndex = IndexOfVert(startCode);
                int endIndex = IndexOfVert(endCode);

                while (startIndex != endIndex)
                {
                    node = new Node();
                    node.Code = Verts[endIndex].label;
                    node.Distance = Path[endIndex].distance;
                    endIndex = Path[endIndex].parentVert;
                    res.Add(node);
                }

                node = new Node();
                node.Code = Verts[startIndex].label;
                node.Distance = Path[startIndex].distance;
                endIndex = Path[startIndex].parentVert;
                res.Add(node);

                res.Reverse();
            }
            catch (Exception ex)
            {
                string text = $"Dijkstra.cs: {ex.Message},{node.Code}";
                Logger.WriteError(text);
            }
           
            return res;
        }
    }
}

