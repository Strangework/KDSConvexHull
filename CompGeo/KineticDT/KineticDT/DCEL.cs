using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KineticDT
{
    //InfInf-Edge is on border and half edge is facing outside.
    //Infint- " " facing inside
    //Int-Edge is internal
    //Inf-Totally infinite edge

    public enum EdgeType
    {
        InfInf, 
        InfInt, 
        Int, 
        Inf // vertex of half edge on cH connected to infinity
    }
    
    public class DCEL
    {
    }
    public class HalfEdge : IComparable<HalfEdge>
    {
        public Vertex origin; //Origin vertex
        
        public HalfEdge twin;
        
        public Face incidentFace;
        
        public HalfEdge next;
        public HalfEdge prev;
        

        private Cert certificate;
        
        public HalfEdge(Vertex origin, HalfEdge twin = null, Face incidentFace = null, HalfEdge next = null, HalfEdge prev = null, Cert certificate = null)
        {
            this.origin = origin;
            this.twin = twin;
            this.incidentFace = incidentFace;
            this.prev = prev;
            this.next = next;
            this.certificate = certificate;
        }
        
        public int CompareTo(HalfEdge halfEdge)
        {
            return 0;
        }
        
        public void UpdatePriority(double time)
        {

        }
        
        public EdgeType EdgeIs
        {
            get { return EdgeType.InfInf; }
        }

        //Return -1 for empty Cert
        public double CertTime
        {
            get { return 0.0; }
        }
        
        public void UpdateCert(Cert c)
        {

        }

    }
    public class Face : IComparable
    {
        public double timeCreated;
        public HalfEdge edge;
        public int CompareTo(object obj)
        {
            return 0;
        }
        public Face(HalfEdge e = null, double t = 0)
        {
            edge = e;
            timeCreated = t;
        }
        
    }
    public class Vertex : IComparable
    {
        public HalfEdge edge;
        public Point point;
        public int CompareTo(object obj)
        {
            return 0;
        }
    }
    public struct Point
    {
        private double x_;
        private double y_;
        double v_x;
        double v_y;
        public double x(double time)
        {
            return x_ + v_x * time;
        }
        public double y(double time)
        {
            return y_ + v_y * time;
        }
        //Function of time methods go below
    }
    public class PriorityQueue
    {
        public PQObject Enqeue(Cert c)
        {
            return new PQObject();
        }
        public List<PQObject> EnqeueList(List<Cert> certs)
        {
            return new List<PQObject>();
        }
        public Cert PopMin()
        {
            return new Cert();
        }
        public Cert Peek();
        public void UpdatePriority(PQObject pqo, double value)
        {

        }
    }
    public class PQObject
    {

    }
    public class Cert : IComparable
    {
        private double tc;
        public double timeCreated
        {
            get { return tc; }
        }
        public bool internalCert;
        private PQObject pqo;
        public HalfEdge edge;
        public int CompareTo(object obj)
        {
            Cert c = obj as Cert;
            return timeCreated.CompareTo(c);
        }
        //DON'T CALL THIS!!!
        public void UpdatePriority(double time)
        {

        }
    }
}
