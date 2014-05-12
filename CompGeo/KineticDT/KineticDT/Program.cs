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
        //Given an edge in the DCEL known to need to have its certificate updated, run DFS on its associated quadrilateral, creating certificates as it goes.
        public List<Cert> DFSCreateCert(HalfEdge initial, double time)
        {
            return new List<Cert>();
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
        public Face RemoveInternalEdge(HalfEdge e, double time)
        {
            return new Face();
        }
        //Remove an edge with a face going to infinity and merge the faces. Return the resulting face. Also, remove all appropriate certificates of edges that belong to the same quadrilateral.
        public Face RemoveInternalEdge(HalfEdge e, double time)
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
        //Pop all events with a time < (strictly less than) the given time. In this time, update the DT.
        public double ExecuteEvents(double time)
        {
            return 0.0;
        }
    }

}
