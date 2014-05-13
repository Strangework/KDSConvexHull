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
        public Vertex CreateDTInitial(List<Vertex> initialPoints)
        {
            List<HalfEdge> outerFaces = new List<HalfEdge>();//ask David
            List<Face> Triangles = new List<Face>();
            
            //Bootstrapping
            Vertex a, b, c;
            
            HalfEdge ab = new HalfEdge(initialPoints[initialPoints.Count - 1]);
            initialPoints[initialPoints.Count - 1].edge = ab;
            
            a = initialPoints[initialPoints.Count - 1];
            initialPoints.RemoveAt(initialPoints.Count - 1);
            
            HalfEdge bc = new HalfEdge(initialPoints[initialPoints.Count - 1]);
            initialPoints[initialPoints.Count - 1].edge = bc;
            
            b = initialPoints[initialPoints.Count - 1];
            initialPoints.RemoveAt(initialPoints.Count - 1);
            
            HalfEdge ca = new HalfEdge(initialPoints[initialPoints.Count - 1]);
            initialPoints[initialPoints.Count - 1].edge = ca;
            
            c = initialPoints[initialPoints.Count - 1];
            initialPoints.RemoveAt(initialPoints.Count - 1);

            ab.prevous = ca;
            ab.next = bc;
            
            bc.prevous = ab;
            bc.next = ca;
            
            ca.next = ab;
            ca.prevous = bc;

            Face curF = new Face(ab);
            ab.face = curF;
            bc.face = curF;
            ca.face = curF;

            ab.twin = new HalfEdge(b);
            bc.twin = new HalfEdge(c);
            ca.twin = new HalfEdge(a);

            ab.twin.next = ca.twin;
            ab.twin.prevous = bc.twin;
            
            ca.twin.next = bc.twin;
            ca.twin.prevous = ab.twin;
            
            bc.twin.next = ab.twin;
            bc.twin.prevous = ca.twin;

            outerFaces.Add(ab.twin);
            outerFaces.Add(bc.twin);
            outerFaces.Add(ca.twin);

            return new Vertex();
        }
        //returns true if the vertex is within the face
        private bool InFace(Face f, Vertex v, double time)
        {
            return true;
        }
        //returns true if the vertex is on the same side of the plane as the line made by the half edge
        private bool SameSideOfPlane(HalfEdge e, Vertex v, double time)
        {
            return true;
        }
        //if a vertex is in a face, add the vertex to the decel structure, adding new faces, edges etc.
        //returns the new faces made.
        private List<Face> AddVertex(Face f, Vertex v, double time)
        {
            return new List<Face>();
        }
        //Given a triangulated region and a pointer and a half edge of the convex hull, finish the DCEL. From each point on the CH, there is an edge that goes out to infinity.
        private void AddInfEdgesToCH(HalfEdge onCH, double time)
        {

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
                    events.EnqeueList(BFSCreateCert(curE, curT));
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
        //Remove an edge and merge the faces. Return the resulting face. Also, remove all appropriate certificates of edges that belong to the same quadrilateral.
        public Face RemoveInternalEdges(HalfEdge e, double time)
        {
            if (e.CertTime != time)
                return null;
            if (e.twin.face.timeCreated != time)
                e.twin.face.timeCreated = time;
            HalfEdge firstNonBroken = e;
            HalfEdge current = e;
            HalfEdge inE;
            HalfEdge outE;
            bool justVisited = false;
            while (current.CompareTo(firstNonBroken) != 1 || justVisited)
            {
                outE = current.twin.next;
                inE = current.twin.prevous;
                if (current.CertTime == time && current.EdgeIs == EdgeType.Int)
                {
                    current.UpdatePriority(double.MinValue);
                    events.PopMin();
                    if (current.CompareTo(firstNonBroken) == 1)
                    {
                        firstNonBroken = current.prevous;//????? Might not work
                        justVisited = true;
                    }
                    //If looped around. Here we have an edge going to nowhere!
                    if (current.next.CompareTo(current.twin) == 1)
                    {
                        current.prevous = current.twin.next;
                        current.twin.next = current.prevous;
                        current = current.twin.next;
                    }
                    else
                    {
                        inE.next = current.next;
                        current.next.prevous = inE;
                        outE.prevous = current.prevous;
                        current.prevous.next = outE;

                        inE.next.face = current.twin.face;
                        outE.prevous.face = current.twin.face;

                        if (current.vertex.edge.CompareTo(current) == 1)
                            current.vertex.edge = outE;
                        if (current.twin.vertex.edge.CompareTo(current.twin) == 1)
                            current.twin.vertex.edge = inE.next;
                        current = outE;
                    }
                    current.UpdatePriority(double.MinValue);
                    events.PopMin();
                }
                else
                {
                    justVisited = false;
                    current = current.next;
                }
            }
            return current.face;
        }
        //Remove an edge with a face going to infinity and merge the faces. Return the resulting face. Also, remove all appropriate certificates of edges that belong to the same quadrilateral.
        //Recursively removes all edges of neighboring triangles if necessary.
        public Face RemoveInfEdges(HalfEdge e, double time)
        {
            //Note, this code breaks if everything goes on a single line. (This is unlikely and we can prevent the input from ever doing that)
            if (e.CertTime != time)
                return null;
            HalfEdge current = e;
            HalfEdge leftInf;
            //Follow if's are just to make the code more simple.
            if (current.EdgeIs == EdgeType.Inf && current.vertex.CompareTo(Infinity) != 1)
                current = current.twin;
            else if (current.EdgeIs == EdgeType.InfInt)
                current = current.twin;
            bool finished = false;
            if (current.EdgeIs == EdgeType.Inf)
                leftInf = current.twin.prevous.prevous;
            else
                leftInf = current.prevous;
            //Go left for simplicity of code
            while (!finished)
            {
                if (current.EdgeIs == EdgeType.Inf && current.twin.prevous.CertTime == time)
                    current = current.twin.prevous;
                else if (current.EdgeIs == EdgeType.InfInf && current.prevous.CertTime == time)
                    current = current.twin.prevous;
                else
                    finished = true;
            }
            current.face.timeCreated = time;
            while (current.CertTime != time)
            {
                if (current.EdgeIs == EdgeType.Inf)
                {
                    current.twin.prevous.next = current.next;
                    current.next.prevous = current.twin.prevous.next;
                    current.twin.prevous.prevous.face = current.face;
                    current.twin.prevous.face = current.face;
                    current.UpdatePriority(double.MinValue);
                    events.PopMin();
                    if (current.vertex.edge.CompareTo(current) == 1)
                    {

                        current.vertex.edge = current.prevous.twin;
                    }
                    current = current.next;
                }
                else
                {
                    current.prevous.next = current.twin.next;
                    current.twin.next.prevous = current.prevous;
                    current.next.prevous = current.twin.prevous;
                    current.twin.prevous.next = current.next;
                    current.UpdatePriority(double.MinValue);
                    events.PopMin();
                    if (current.vertex.edge.CompareTo(current) == 1)
                    {
                        current.vertex.edge = current.twin.next;
                    }
                    current = current.next.twin;
                }
            }
            if (current.EdgeIs == EdgeType.Inf)
            {
                current.twin.next = leftInf;
                leftInf.prevous = current.twin.next;
            }
            else
            {
                current.next.next = leftInf;
                leftInf.prevous = current.next;
            }
            return current.face;
        }
    }
}
