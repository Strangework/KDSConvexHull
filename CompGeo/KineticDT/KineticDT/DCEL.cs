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
        public Vertex vertex; //Origin vertex
        public HalfEdge twin;
        
        public Face incidentFace;
        
        public HalfEdge next;
        public HalfEdge prev;

        public EdgeType EdgeIs
        {
            get { return EdgeType.Inf; }
        }

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

        //Return -1 for empty Cert or delaunay edge cert!!
        public double CertTime
        {
            get { return 0.0; }
        }
        
        public void UpdateCert(Cert c)
        {
            
        }
        //returns true of the saved cert is the static, DelaunayEdge cert
        public bool DelaunayEdgeCert()
        {
            return false;
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
    
    public class PQKey
    {
        public int loc;
    }

    public class PQWrapper<T> where T : IComparable<T>
    {
        public PQKey key;
        
        public T data;
        public double priority;

        public int CompareTo(PQWrapper<T> rhs)
        {
            if (this.priority < rhs.priority) return -1;
            else if (this.priority > other.priority) return 1;
            else return 0;
        }
    }

    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<PQWrapper<T>> list;

        public PriorityQueue()
        {
            this.list = new List<PQWrapper<T>>();
        }

        public PQKey Enqueue(T item, double priority)
        {
            PQWrapper<T> newItem = new PQWrapper<T>();
            
            newItem.key = new PQKey();
            newItem.data = item;
            newItem.priority = priority;
            
            list.Add(newItem);
            
            int ci = list.Count - 1; // child index; start at end
            newItem.key.loc = ci;
            
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                
                if (list[ci].CompareTo(list[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                
                PQWrapper<T> tmp = list[ci]; 
                list[ci] = list[pi]; 
                list[pi] = tmp;
                
                list[ci].key.loc = ci;
                list[pi].key.loc = pi;
                ci = pi;
            }
            
            return(newItem.key);
        }

        public T Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = list.Count - 1; // last index (before removal)
            T frontItem = list[0].data;   // fetch the front
            
            list[0] = list[li];
            list.RemoveAt(li);

            --li; // last index (after removal)
            //Adjust key vals
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].key.loc = i;
            }
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li) break;  // no children so done
                int rc = ci + 1;     // right child
                if (rc <= li && list[rc].CompareTo(list[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    ci = rc;
                if (list[pi].CompareTo(list[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                PQWrapper<T> tmp = list[pi]; list[pi] = list[ci]; list[ci] = tmp; // swap parent and child
                //Adjust key vals
                list[ci].key.loc = ci;
                list[pi].key.loc = pi;
                pi = ci;
            }
            return frontItem;
        }

        public void ChangePriority(PQKey key, double newP)
        {
            double oldP = list[key.loc].priority;
            list[key.loc].priority = newP;
            if (oldP > list[key.loc].priority)
            {
                int ci = key.loc;
                while (ci > 0)
                {
                    int pi = (ci - 1) / 2; // parent index
                    if (list[ci].CompareTo(list[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                    PQWrapper<T> tmp = list[ci]; list[ci] = list[pi]; list[pi] = tmp;
                    list[ci].key.loc = ci;
                    list[pi].key.loc = pi;
                    ci = pi;
                }
            }
            else
            {
                int pi = key.loc; // parent index. start at front of pq
                int li = list.Count - 1;
                while (true)
                {
                    int ci = pi * 2 + 1; // left child index of parent
                    if (ci > li) break;  // no children so done
                    int rc = ci + 1;     // right child
                    if (rc <= li && list[rc].CompareTo(list[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                        ci = rc;
                    if (list[pi].CompareTo(list[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                    PQWrapper<T> tmp = list[pi]; list[pi] = list[ci]; list[ci] = tmp; // swap parent and child
                    //Adjust key vals
                    list[ci].key.loc = ci;
                    list[pi].key.loc = pi;
                    pi = ci;
                }
            }
        }

        public T Peek()
        {
            T frontItem = list[0].data;
            return frontItem;
        }

        public int Count()
        {
            return list.Count;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < list.Count; ++i)
                s += ("(" +list[i].data.ToString()+ ", " +list[i].priority+") ");
            s += "count = " + list.Count;
            return s;
        }

        public bool IsConsistent()
        {
            // is the heap property true for all data?
            if (list.Count == 0) return true;
            int li = list.Count - 1; // last index
            for (int pi = 0; pi < list.Count; ++pi) // each parent index
            {
                int lci = 2 * pi + 1; // left child index
                int rci = 2 * pi + 2; // right child index

                if (lci <= li && list[pi].CompareTo(list[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
                if (rci <= li && list[pi].CompareTo(list[rci]) > 0) return false; // check the right child too.
            }
            return true; // passed all checks
        } // IsConsistent
        
        
    } // PriorityQueue
    
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
        public Cert()
        {
            timeCreated = -1;
        }
        public static Cert DelaunayEdge = new Cert();
    }
}
