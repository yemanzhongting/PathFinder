using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Where
{
    /// <summary>
    /// 边库
    /// 
    /// </summary>
    public class EdgeSet
    {
        public List<Edge> Data { get; set; }

        public EdgeSet()
        {
            Data = new List<Edge>();
        }
        //
        public Edge this[int start,int end]
        {
            get
            {
                var res = new Edge();
                foreach (var d in Data)
                {
                    if((d.StartCode==start && d.EndCode==end)
                        ||(d.StartCode==end && d.StartCode==start))
                    {
                        res = d;
                        break;
                    }
                }
                return res;

            }
        }

        public int Count => Data.Count;
        public int IndexOf(Edge edge) => Data.IndexOf(edge);

        public void Add(Edge edge)
        {
            if(!Data.Contains(edge))
            {
                Data.Add(edge);
            }
        }

        public void Add(EdgeSet edgeSet)
        {
            foreach (var d in edgeSet.Data)
            {
                Add(d);
            }
        }
    }
}
