using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PriorityQueues;

namespace KineticCH
{
   
    public enum CertType{
         x, yli, yri, yt, slt, srt, sl, sr, any
    }
    public class Program
    {
        //Global priority queue of events to be processed
        public PriorityQueue<Certificate> events;
        //Global binary tree of the different envelopes. Children = 2*i + 1 , 2*i + 2; parents = floor((i-1)/2)
        public Envelope[] envelopeHeirarchy;
        static void Main(string[] args)
        {
        }
        //Call this function every time time is changed.
        public void RemoveVertex(Vertex v) 
        {
            for (int i = 0; i < v.certs.Length; ++i)
            {
                //remove each certificate from the priority queue.
            }
        }
        //Split the vertex OF THE OUTPUT!
        //Come up with method for handling mutliple verticies being created at the same point at the same time.
        public Vertex SplitVertex(Vertex v, Line newl, double time)
        {
            if (v == null)
                return null;
            Vertex newVert = new Vertex(newl, v.b);
            Edge newE = new Edge(v, newVert);
            newVert.rightE = v.rightE;
            v.rightE = newE;
            newVert.leftE = newE;
            newVert.contender = v.contender;
            newVert.prev = v;
            newVert.next = v.next;
            v.RemoveCert(CertType.x);
            v.x_ = CertificateMaker(v, time + .000000001, CertType.x)[0];
            events.Enqueue(v.x_);
            newVert.certs = CertificateMaker(newVert, time + .000000001);
            newVert.parent = SplitVertex(v.parent, newl, time);
            return newVert;//Returns the parent of the new vertex
        }
        public Vertex MergeVerticies(Edge e, double time)
        {
            e.leftV.RemoveCert(CertType.x);
            return e.leftV;
        }
        public Envelope FuseCH(Envelope first, Envelope second, double time)
        {
            return null;
        }
        public Envelope CreateCH(List<Line> input, double time)
        {
            return null;
        }
        private void HandleMassDegeneracy(Vertex v, double time)
        {

        }
        public void HandleEvents(double time)
        {
            Certificate curC;
            int level = events.Peek().binaryTreeIndex;
            List<Vertex> recheck = new List<Vertex>();
            while (time > events.Peek().timeViolated)
            {
                bool vAddRm = true;
                curC = events.Dequeue();
                if (curC.binaryTreeIndex > level)
                {
                    while (recheck.Count != 0)
                    {
                        Vertex curV = recheck[recheck.Count-1];
                        //Get leftmost one in chain to be fused for simplicity.
                        while (curV.leftE.leftV.dying)
                        {
                            curV = curV.leftE.leftV;
                        }
                        Vertex leftOfLeft = curV.leftE.leftV;
                        curV.splitting.Add(curV.leftE.superLine);
                        Envelope envelopeAtPoint = CreateCH(curV.splitting, time);
                        Vertex parent = curV.parent;
                        while (curV.rightE.rightV.dying)
                        {
                            curV = curV.next;
                            Envelope temp = CreateCH(curV.splitting, time);
                            envelopeAtPoint = FuseCH(envelopeAtPoint, temp, time);
                            if (parent == null)
                            {
                                parent = curV.parent;
                            }
                        }
                        Vertex rightOfRight = curV.rightE.rightV;
                        //At this point we have the two points at the ends of the chain of possibly dying verticies with possibly new edges sprouting from any of these points and the upper envelope made by
                        //new potential points. 
                        //Check if the contender edge of any of the verticies at the same point has a vertex that is directly above the vertex. If this vertex is dying, or being split,
                    }
                }
                else if (curC.binaryTreeIndex < level)
                {

                }
                curC.vertex.RemoveCert(curC.type, true);
                //Depending on certificate type, do different things.
                Vertex cd = null, ab = curC.vertex;
                //Might need to include special cases for all of these if something is a dummy vertex/edge (consider putting that logic code in CreateCert, having it return null. Then add code to the PQ that does nothing if you try to add something that's null
                switch (curC.type)
                {
                    case CertType.x:
                        {
                            cd = ab.next;
                            ab.next = cd.next;
                            cd.prev = ab.prev;
                            ab.prev = cd;
                            cd.next = ab;
                            ab.contender = cd.rightE;
                            cd.contender = ab.leftE;
                            if (cd.prev.x_ != null)//ab.prev before change
                            {
                                cd.prev.RemoveCert(CertType.x);
                            }
                            else
                            {
                                cd.prev.x_ = CertificateMaker(cd.prev, time, CertType.x)[0];
                                events.Enqueue(cd.prev.x_);
                            }
                            if (cd.x_ != null)
                                cd.RemoveCert(CertType.x);
                            else
                            {
                                ab.x_ = CertificateMaker(ab, time, CertType.x)[0];
                                events.Enqueue(ab.x_);
                            }
                            if (!(ab.contender.EdgeIsBelow(ab, time)))
                            {
                                if (cd.yt != null)
                                {
                                    cd.RemoveCert(CertType.slt);
                                    cd.RemoveCert(CertType.srt);
                                    cd.RemoveCert(CertType.yt);
                                }
                                if (ab.sl != null)
                                {
                                    ab.RemoveCert(CertType.sl);
                                }
                                if(ab.x_ != null && ab.b.slope(time) < cd.b.slope(time))
                                {
                                    ab.sl = CertificateMaker(ab, time, CertType.sl)[0];
                                    events.Enqueue(ab.sl);
                                }
                                if (cd.prev.x_ != null && !cd.prev.contender.EdgeIsBelow(cd.prev, time) && ab.a.slope(time) < cd.a.slope(time))
                                {
                                    cd.prev.sl = CertificateMaker(cd.prev, time, CertType.sl)[0];
                                    events.Enqueue(cd.prev.sl);
                                }
                                else if (ab.sr != null)
                                {
                                    if (ab.a.slope(time) < cd.b.slope(time))
                                    {
                                        ab.RemoveCert(CertType.sr);
                                        cd.slt = CertificateMaker(cd, time, CertType.slt)[0];
                                        events.Enqueue(cd.slt);
                                        cd.srt = CertificateMaker(cd, time, CertType.srt)[0];
                                        events.Enqueue(cd.srt);
                                        cd.yt = CertificateMaker(cd, time, CertType.yt)[0];
                                        events.Enqueue(cd.yt);

                                    }
                                    else
                                    {
                                        ab.RemoveCert(CertType.sr);
                                        ab.sr = CertificateMaker(ab, time, CertType.sr)[0];
                                        events.Enqueue(ab.sr);
                                    }
                                }
                            }
                            break;
                        }
                    case CertType.yli:
                        {
                            ab.next.RemoveCert(CertType.yri);
                            ab.RemoveCert(CertType.yli);
                            if (ab.yri != null)
                            {
                                ab.prev.RemoveCert(CertType.yli);
                                ab.RemoveCert(CertType.yri);
                                ab.slt = CertificateMaker(ab, time, CertType.slt)[0];
                                events.Enqueue(ab.slt);
                                ab.srt = CertificateMaker(ab, time, CertType.srt)[0];
                                events.Enqueue(ab.srt);
                                ab.yt = CertificateMaker(ab, time, CertType.yt)[0];
                                //Remove ab.contender from output!!
                            }
                            else
                            {
                                ab.yri = CertificateMaker(ab, time, CertType.yri)[0];
                                events.Enqueue(ab.yri);
                                ab.prev.yli = CertificateMaker(ab.prev, time, CertType.yli)[0];
                                events.Enqueue(ab.prev.yli);
                                if (ab.contender.EdgeIsBelow(ab, time - .0000001))//Just before intersection
                                {
                                    //Add a!!
                                    SplitVertex(ab.parent.next, ab.a, time);
                                }
                                else
                                {
                                    //Remove b!!
                                }
                            }
                            break;
                        }
                    case CertType.yri:
                        {
                            //Fill this in later
                            break;
                        }
                    case CertType.yt:
                        {
                            ab.RemoveCert(CertType.slt);
                            ab.RemoveCert(CertType.srt);
                            ab.yli = CertificateMaker(ab, time, CertType.yli)[0];
                            events.Enqueue(ab.yli);
                            ab.next.yri = CertificateMaker(ab.next, time, CertType.yri)[0];
                            events.Enqueue(ab.next.yri);
                            ab.yri = CertificateMaker(ab, time, CertType.yri)[0];
                            events.Enqueue(ab.yri);
                            ab.prev.yli = CertificateMaker(ab.prev, time, CertType.yli)[0];
                            events.Enqueue(ab.prev.yli);
                            //Add ab.contender to output!!
                            SplitVertex(ab.parent, ab.contender.superLine, time);
                            break;
                        }
                    case CertType.slt:
                        {
                            //Remove ab.srt 
                            ab.RemoveCert(CertType.srt);
                            ab.RemoveCert(CertType.yt);
                            cd = ab.prev;
                            if (cd.b == ab.a)
                            {
                                cd.slt = CertificateMaker(cd, time, CertType.slt)[0];
                                events.Enqueue(cd.slt);
                                cd.srt = CertificateMaker(cd, time, CertType.srt)[0];
                                events.Enqueue(cd.srt);
                                cd.yt = CertificateMaker(cd, time, CertType.yt)[0];
                                events.Enqueue(cd.yt);
                            }
                            else
                            {
                                cd.sl = CertificateMaker(cd, time, CertType.sl)[0];
                                events.Enqueue(cd.yt);
                            }
                            break;
                        }
                    case CertType.srt:
                        {
                            //Fill this in later
                            break;
                        }
                    case CertType.sl:
                        {
                            cd = ab.next;
                            //I think this line always works:
                            if (ab.x_ != null)
                            {
                                cd.slt = CertificateMaker(cd, time, CertType.slt)[0];
                                events.Enqueue(cd.slt);
                                cd.srt = CertificateMaker(cd, time, CertType.srt)[0];
                                events.Enqueue(cd.srt);
                                cd.yt = CertificateMaker(cd, time, CertType.yt)[0];
                                events.Enqueue(cd.yt);
                            }
                            else if(!(cd.contender == ab.contender))
                            {
                                cd.sr = CertificateMaker(cd, time, CertType.sr)[0];
                                events.Enqueue(cd.sr);
                            }
                            break;
                        }
                    case CertType.sr:
                        {
                            //Fill this in later
                            break;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                }
                //...If you  remove vertex
                RemoveVertex(curC.vertex);
                //...If you add or remove verticies
                if (vAddRm)
                {
                    //While either of there are non-null parents of the current vertex changed,  update those envelopes too
                }
            }
        }
        //EMPTY METHOD ):
        public Certificate[] CertificateMaker(Vertex v, double time, CertType ct = CertType.any)
        {
            return new Certificate[8];
        }
        public bool WithinXBounds(Vertex v, Edge e)
        {
            return false;
        }//EMPTY METHOD
        //This function creates the fusion of two upper envelopes and creates all the certificates for the two chains.
        public Envelope MergeEnvelopes(Envelope a, Envelope b, double time)
        {
            //User must create verticies unless creating a new envelope, giving envelope a line.
            Envelope top, bottom, ret;
            a.Reset();
            b.Reset();
            //A is the left line, the line that goes to infinity for the first vertex!
            if (a.Current.b.slope(time) < b.Current.b.slope(time))
            {
                top = a;
                bottom = b;
                ret = new Envelope(a.Current.b);
            }
            else
            {
                top = b;
                bottom = a;
                ret = new Envelope(b.Current.b);
            }
            top.Forward();
            bottom.Forward();
            while (!(top.Current.b == Line.infinite))
            {
                //This works because once two edges cross, they don't cross again. 
                if (bottom.Current.rightE.Intersects(top.Current.rightE, time))
                {
                    //Create new vertex, certificates, add vertex
                    Vertex newV = new Vertex(top.Current.rightE.superLine, bottom.Current.rightE.superLine);
                    ret.Append(newV);

                    //Don'tswap if degenerate points
                    if (top.Current.x(time) != bottom.Current.x(time))
                    {
                        Envelope temp = top;
                        top = bottom;
                        bottom = temp;
                    }

                }
                //Stupid code
                int f = 1;
                if (f == 1)
                {
                    //Add degenerate case where they share the same x coordinate
                }
                //Bottom right vertex isn't last vertex
                else if (WithinXBounds(bottom.Current, top.Current.rightE))
                {
                    bottom.Current.certs = CertificateMaker(bottom.Current, time);
                    //Have pq enqueue all certs!!!
                    //POSSIBLY ADD THIS CODE TO CERTIFICATEMAKER???
                    bottom.Current.contender = top.Current.rightE;
                    bottom.Current.prev = bottom.Current.leftE.leftV.x(time) < top.Current.x(time) ? bottom.Current.leftE.leftV : top.Current;
                    bottom.Current.next = bottom.Current.rightE.rightV.x(time) < top.Current.rightE.rightV.x(time) ? bottom.Current.rightE.rightV : top.Current.rightE.rightV;
                    //^^^
                    bottom.Current.certs = CertificateMaker(bottom.Current, time);
                    //Have pq enqueue all certs!!!
                    bottom.Forward();
                }
                //Bottom right vertex is last vertex
                else
                {
                    Vertex newV = new Vertex(top.Current.a, top.Current.b);
                    //Allow for O(1) access to the next identical vertex in the tree
                    top.Current.parent = newV;
                    ret.Append(newV);
                    //POSSIBLY ADD THIS CODE TO CERTIFICATEMAKER???
                    top.Current.contender = bottom.Current.rightE;
                    top.Current.prev = top.Current.leftE.leftV.x(time) < bottom.Current.x(time) ? top.Current.leftE.leftV : bottom.Current;
                    top.Current.next = top.Current.rightE.rightV.x(time) < bottom.Current.rightE.rightV.x(time) ? top.Current.rightE.rightV : bottom.Current.rightE.rightV;
                    //^^^
                    top.Current.certs = CertificateMaker(top.Current, time);
                    //Have pq enqueue all certs!!
                    top.Forward();
                }
            }
            return ret;
        }
    }
    public class Envelope
    {
        //Starts at LHS
        private Vertex head;
        private Vertex tail;
        private Vertex current;
        public Vertex Current
        {
            get { return current; }
        }
        public void Reset() 
        {
            current = head;
        }
        public Vertex Forward()
        {
            if(current != null)
                current = current.rightE.rightV;
            return current;
        }
        public Vertex Back()
        {
            if (current != null)
                current = current.leftE.leftV;
            return current;
        }
        public void Append(Vertex v, bool end = true)
        {
            Line newl;
            if(end)
            {
                if (v.a == tail.a)
                    newl = v.b;
                else
                    newl = v.a;
                Vertex lastReal = tail.leftE.leftV;
                tail.b = newl;
                tail.leftE.leftV = v;
                lastReal.rightE = new Edge(lastReal, v);
                v.leftE = lastReal.rightE;
                v.rightE = tail.leftE;
            }
            else
            {
                if (v.a == head.a)
                    newl = v.b;
                else
                    newl = v.a;
                Vertex lastReal = head.rightE.rightV;
                head.b = newl;
                head.rightE.rightV = v;
                lastReal.leftE = new Edge(v, lastReal);
                v.rightE = lastReal.leftE;
                v.leftE = head.rightE;
            }
        }
        public Envelope(Line start)
        {
            head = new Vertex(Line.infinite, start, null, null, null, null, Vertex.infinite);
            tail = new Vertex(start, Line.infinite, null, null, null, null, Vertex.infinite);
            head.next = tail;
            tail.prev = head;
            Edge e = new Edge(head, tail);
            head.rightE = e;
            tail.leftE = e;
            head.leftE = Edge.infinite;
            tail.rightE = Edge.infinite;
        }
        public void Insert(int numV)
        {

        }
        public void Remove(Vertex v)
        {

        }
    }
    //LINE IS A STRUCT
    public struct Line : IComparable
    {
        public static Line infinite = new Line(double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue);
        public double p_x;
        public double p_y;
        public double v_x;
        public double v_y;
        public double slope(double time)
        {
            return p_x + v_x * time;
        }
        public double intercept(double time)
        {
            return p_y + v_y * time;
        }
        public double y(double x)
        {
            return p_x * x + p_y;
        }
        public static bool operator ==(Line l, Line r)
        {
            return l.p_x == r.p_x && l.p_y == r.p_y && l.v_x == r.v_x && l.v_y == r.v_y;
        }
        public Line(double px, double py, double vx, double vy)
        {
            p_x = px;
            p_y = py;
            v_x = vx;
            v_y = vy;
        }
        public double Intersection(Line other, double time)
        {
            return (other.intercept(time) - intercept(time)) / (slope(time) - other.slope(time));
        }
    }
    //VERTEX IS A CLASS. 
    public class Vertex 
    {
        public bool dying;
        public List<Line> splitting;
        public static Vertex infinite = new Vertex();
        public Certificate[] certs;
        //Line a is ALWAYS a real line.
        //If the vertex is a dummy vertex, b is Line.infinite.
        //However, if the vertex is connected to a real point on the left, rightE is real, and vice versa.
        public Line a;
        public Line b;
        public Edge leftE;
        public Edge rightE;
        public Edge contender;
        public Vertex parent;
        public Vertex prev, next;
        public Vertex(Line aa, Line bb, Vertex par = null, Edge l= null, Edge r = null, Edge c = null, Vertex p = null, Vertex n = null)
        {
            a = aa;
            b = bb;
            leftE = l;
            rightE = r;
            parent = par;
            prev = p;
            next = n;
            certs = new Certificate[8];
            splitting = new List<Line>();
            dying = false;
        }
        //This is meant to be empty
        private Vertex()
        {

        }
        //FILL THESE METHODS IN!!!!
        public double y(double time){
            return -1;
        }
        public double x(double time){
            return -1;
        }
        //FILL IN THESE TOO!!
        public Certificate x_
        {
            get {return new Certificate();}
            set { }
        }
        public Certificate yli
        {
            get {return new Certificate();}
            set { }
        }
        public Certificate yri
        {
            get {return new Certificate();}
            set { }
        }
        public Certificate yt
        {
            get {return new Certificate();}
            set { }
        }
        public Certificate slt
        {
            get {return new Certificate();}
            set { }
        }
        public Certificate srt
        {
            get {return new Certificate();}
            set { }
        }
        public Certificate sl
        {
            get {return new Certificate();}
            set { }
        }
        public Certificate sr
        {
            get {return new Certificate();}
            set { }
        }
        //Filling this in might be a good idea (sarcasm)
        public void RemoveCert(CertType c, bool alreadyRemovedFromPQ = false)
        {

        }
    }
    //EDGE IS A CLASS.
    //EDGE CANNOT CREATE NEW VERTICIES
    public class Edge
    {
        public static Edge infinite = new Edge();
        public Vertex leftV, rightV;
        public Line superLine;
        private Edge()
        {
            leftV = null;
            rightV = null;
            superLine = Line.infinite;
        }
        public Edge(Vertex l, Vertex r)
        {
            leftV = l;
            rightV = r;
            if (leftV.a == rightV.b)
            {
                superLine = leftV.a;
            }
            else if (leftV.a == rightV.a)
            {
                superLine = leftV.a;
            }
            else
                superLine = leftV.b;
        }
        public bool Intersects(Edge other, double time)
        {
            double xleft = Math.Max(leftV.x(time), other.leftV.x(time)),
                xright = Math.Min(rightV.x(time), other.rightV.x(time));
            double xint = superLine.Intersection(other.superLine, time);
            return xint >= xleft && xint <= xright;
        }
        //EMPTY METHOD
        public bool EdgeIsBelow(Vertex v, double time)
        {
            return true;
        }
    }
    public class Certificate : IComparable
    {
        //Change priority queue so each certificate has a handle to itself in the priority queue.
        //The priority queue should also have the ability to change the handle too!
        public CertType type;
        public double timeViolated;
        public Vertex vertex;
        public int binaryTreeIndex;
        public int CompareTo(object obj)
        {
            Certificate other = obj as Certificate;
            if (other != null)
                return timeViolated.CompareTo(other.timeViolated);
            else
                throw new Exception();
        }
    }
}