using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace KineticDT
{
    class Program
    {
        public Vertex Infinity;
        public PriorityQueue events;
        static void Main(string[] args)
        {
        }
        //This function creates the initial Delaunay triangulation at time t = 0, including the vertex at infinity.
        public Vertex CreateDTInitial(List<Point> initialPoints)
        {
            return new Vertex();
        }
        //Given a half edge, create a certificate for an internal DT
        public Cert CreateInteralCert(HalfEdge e, double time)
        {
            return new Cert();
        }
        //Given a half edge, create a certificate for a CH DT
        public Cert CreateCHCert(HalfEdge e, double time)
        {
            return new Cert();
        }
        //Remove an edge and merge the faces. Return the resulting face. Also, remove all appropriate certificates of edges that belong to the same quadrilateral.
        public Face RemoveInternalEdgeRecursive(HalfEdge e, double time, Face starter = null)
        {
            //First check anything connected by the current face, then try the other direction
            return new Face();
        }
        //Remove an edge with a face going to infinity and merge the faces. Return the resulting face. Also, remove all appropriate certificates of edges that belong to the same quadrilateral.
        //Recursively removes all edges of neighboring triangles if necessary.
        public Face RemoveInfEdgeRecursive(HalfEdge e, double time)
        {
            return new Face();
        }
        //Given a new face (in this code it will always have a convex boundary), triangulate it.
        public HalfEdge TriangulateRegion(Face f, double time)
        {
            return new HalfEdge();
        }
        //Given an outer face that goes to infinity, create the convex hull of the outer points on the face.
        public HalfEdge CreateCH(Face f, double time)
        {
            return new HalfEdge();
        }
        
        //The methods below are DONE
        
        //Pop all events with a time < (strictly less than) the given time. In this time, update the DT.
        //Returns the time of the last event executed.
        public double ExecuteEvents(double time)
        {
            double curT = 0.0;
            while (events.Peek().timeCreated < time)
            {
                curT = events.Peek().timeCreated;
                while (events.Peek().timeCreated == curT)
                {
                    Cert curC = events.PopMin();
                    Face curF;
                    HalfEdge curE;
                    if (curC.internalCert)
                    {
                        curF = (RemoveInternalEdgeRecursive(curC.edge, curT));
                        RemoveBorderCert(curF);
                        curE = TriangulateRegion(curF, time);
                    }
                    else
                    {
                        curF = (RemoveInfEdgeRecursive(curC.edge, curT));
                        RemoveBorderCert(curF);
                        curE = CreateCH(curF, time);
                    }
                    events.EnqeueList(DFSCreateCert(curE, curT));
                }
            }
            return curT;
        }
        //Loop around a face and remove all certificates associated with it.
        public void RemoveBorderCert(Face f)
        {
            HalfEdge start = f.edge;
            HalfEdge curE = start;
            do
            {
                curE.UpdatePriority(double.MinValue);
                events.PopMin();
                curE = curE.next;
            } while (curE.CompareTo(start) != 1);
        }
        //Given an edge in the DCEL known to need to have its certificate updated, run DFS on its associated quadrilateral, creating certificates as it goes.
        public List<Cert> BFSCreateCert(HalfEdge initial, double time)
        {
            List<Cert> ret = new List<Cert>();
            if (initial.CertTime == time || initial.CertTime != -1)
                return ret;
            ret.Add(CreateCHCert(initial, time));
            initial.UpdateCert(ret[ret.Count - 1]);
            EdgeType initialET = initial.EdgeIs;
            List<HalfEdge> BFSNextLevel = new List<HalfEdge>();
            //This code is unnecessarily ugly but should work!
            if (initialET != EdgeType.Int)
            {
                if (initialET == EdgeType.InfInf)
                    initial = initial.twin;
                if (initial.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next, time));
                    initial.next.UpdateCert(ret[ret.Count - 1]);
                    BFSNextLevel.Add(initial.next);
                }
                if (initial.next.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next.next, time));
                    initial.next.next.UpdateCert(ret[ret.Count - 1]);
                    BFSNextLevel.Add(initial.next.next);
                }
            }
            else
            {
                if (initial.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next, time));
                    initial.next.UpdateCert(ret[ret.Count - 1]);
                    BFSNextLevel.Add(initial.next);
                }
                if (initial.next.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next.next, time));
                    initial.next.next.UpdateCert(ret[ret.Count - 1]);
                    BFSNextLevel.Add(initial.next.next);
                }

                initial = initial.twin;
                if (initial.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next, time));
                    initial.next.UpdateCert(ret[ret.Count - 1]);
                    BFSNextLevel.Add(initial.next);
                }
                if (initial.next.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next.next, time));
                    initial.next.next.UpdateCert(ret[ret.Count - 1]);
                    BFSNextLevel.Add(initial.next.next);
                }
            }
            for (int i = 0; i < BFSNextLevel.Count; ++i)
                ret.AddRange(BFSCreateCert(BFSNextLevel[i], time));
            return ret;
        }
    }
}