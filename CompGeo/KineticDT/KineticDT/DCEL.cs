using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KineticDT
{
    public class DCEL
    {
    }
    public class HalfEdge
    {
        HalfEdge next;
        Vertex nextV;
        HalfEdge prevous;
        HalfEdge prevV;
        HalfEdge twin;
        Face face;
        double timeCertCreated;
    }
    public class Face
    {
        double timeCreated;
    }
    public class Vertex
    {

    }
    public struct Point
    {
        double x_;
        double y_;
        double v_x;
        double v_y;
        //Function of time methods go below
    }
    public class PriorityQueue
    {
        public PQObject Enqeue(Cert c)
        {
            return new PQObject();
        }
        public Cert PopMin()
        {
            return new Cert();
        }
        public void UpdatePriority(PQObject pqo, double value)
        {

        }
    }
    public class PQObject
    {

    }
    public class Cert
    {

    }
}
