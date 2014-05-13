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
        InfInf, // InFInf are half edges on the CH
        InfInt, // the twins of InfInf
        Int, // Internal half edges
        Inf // Inf are edges connected to the inifinity
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
        
        public EdgeType edgeType;

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
        
        public int CompareTo(HalfEdge rhs)
        {
            return (this == rhs) 1 : 0;
            
        }
        
        public void UpdatePriority(double time)
        {

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
    
    public class Face : IComparable<Face>
    {
        public HalfEdge halfEdge;
        public double timeCreated;
        
        public int CompareTo(Face rhs)
        {
            return (this == rhs) 1 : 0; 
        }
        
        public Face(HalfEdge halfEdge = null, double timeCreated = 0)
        {
            this.halfEdge = halfEdge;
            this.timeCreated = timeCreated;
        }
    }
    
    public class Vertex : IComparable<Vertex>
    {
        public HalfEdge halfEdge;
        public Point point;
        
        public int CompareTo(Vertex rhs)
        {
            return (this == rhs) 1 : 0;
        }
        
        public Vertex(HalfEdge halfEdge = null, Point point = null)
        {
            
            this.halfEdge = halfEdge;
            this.point = point;
        }
    }
    
    public struct Point
    {
        public double x_;
        public double y_;
        
        public double v_x;
        public double v_y;
        
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
    
    public class Cert : IComparable<Cert>
    {
        public double timeCreated;
        
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
