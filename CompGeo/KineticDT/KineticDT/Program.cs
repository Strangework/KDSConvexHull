using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KineticDT
{
    class Program
    {
        public Vertex Infinity;
        public PriorityQueue<Cert> events;
        
        static void Main(string[] args)
        {
        }

        #region Helper Methods
        public double InCircle(Point pa, Point pb, Point pc, Point pd)
        {
            double adx, bdx, cdx, ady, bdy, cdy;
            double bdxcdy, cdxbdy, cdxady, adxcdy, adxbdy, bdxady;
            double alift, blift, clift;
            double det;

            double permanent;

            // I have 3 points that are on the circumference of the circle
            adx = pa.x_ - pd.x_;
            bdx = pb.x_ - pd.x_;
            cdx = pc.x_ - pd.x_;
            ady = pa.y_ - pd.y_;
            bdy = pb.y_ - pd.y_;
            cdy = pc.y_ - pd.y_;

            bdxcdy = bdx * cdy;
            cdxbdy = cdx * bdy;
            alift = adx * adx + ady * ady;

            cdxady = cdx * ady;
            adxcdy = adx * cdy;
            blift = bdx * bdx + bdy * bdy;

            adxbdy = adx * bdy;
            bdxady = bdx * ady;
            clift = cdx * cdx + cdy * cdy;

            det = alift * (bdxcdy - cdxbdy)
                + blift * (cdxady - adxcdy)
                + clift * (adxbdy - bdxady);

            permanent = (Math.Abs(bdxcdy) + Math.Abs(cdxbdy)) * alift
                      + (Math.Abs(cdxady) + Math.Abs(adxcdy)) * blift
                      + (Math.Abs(adxbdy) + Math.Abs(bdxady)) * clift;

            return det;
        }

        //Returns true if an edge is locally delaunay.
        private bool IsLocalDelaunay(HalfEdge halfEdge, double time)
        {
            if (halfEdge.EdgeIs == EdgeType.InfInf)
                return true;
            else if (halfEdge.EdgeIs == EdgeType.Int)
            {
                Vertex u = halfEdge.vertex;
                Vertex v = halfEdge.next.vertex;
                Vertex p = halfEdge.next.next.vertex;
                Vertex q = halfEdge.twin.next.next.vertex;
                if (InCircle(u.point, v.point, p.point, q.point) <= 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        //returns true if the vertex is within the face
        private bool InFace(Face f, Vertex v, double time)
        {
            return (SameSideOfPlane(f.halfEdge, v, time) && SameSideOfPlane(f.halfEdge.next, v, time) && SameSideOfPlane(f.halfEdge.next.next, v, time));
        }
        //returns true if the vertex is on the same side of the plane as the line made by the half edge
        private bool SameSideOfPlane(HalfEdge e, Vertex v, double time)
        {
            double slope = (e.vertex.point.y(time) - e.twin.vertex.point.y(time)) / ((e.vertex.point.x(time) - e.twin.vertex.point.x(time)));
            double intercept = e.vertex.point.y(time) - slope * e.vertex.point.x(time);
            bool above = e.twin.vertex.point.x(time) > e.vertex.point.x(time);
            return above && v.point.y(time) > v.point.x(time) * slope + intercept;
        }
        #endregion

        #region Functions that create DTs
        //This function creates the initial Delaunay triangulation at time t = 0, including the vertex at infinity.
        public HalfEdge CreateDTInitial(List<Vertex> initialPoints)
        {
            List<HalfEdge> outerFaces = new List<HalfEdge>();//ask David
            List<Face> triangles = new List<Face>();
            List<HalfEdge> edgesToCheck = new List<HalfEdge>();

            //Bootstrapping
            Vertex a, b, c;

            HalfEdge ab = new HalfEdge(initialPoints[initialPoints.Count - 1]);
            initialPoints[initialPoints.Count - 1].halfEdge = ab;

            a = initialPoints[initialPoints.Count - 1];
            initialPoints.RemoveAt(initialPoints.Count - 1);

            HalfEdge bc = new HalfEdge(initialPoints[initialPoints.Count - 1]);
            initialPoints[initialPoints.Count - 1].halfEdge = bc;

            b = initialPoints[initialPoints.Count - 1];
            initialPoints.RemoveAt(initialPoints.Count - 1);

            HalfEdge ca = new HalfEdge(initialPoints[initialPoints.Count - 1]);
            initialPoints[initialPoints.Count - 1].halfEdge = ca;

            c = initialPoints[initialPoints.Count - 1];
            initialPoints.RemoveAt(initialPoints.Count - 1);

            ab.prev = ca;
            ab.next = bc;

            bc.prev = ab;
            bc.next = ca;

            ca.next = ab;
            ca.prev = bc;

            Face curF = new Face(ab);
            ab.incidentFace = curF;
            bc.incidentFace = curF;
            ca.incidentFace = curF;

            ab.twin = new HalfEdge(b);
            bc.twin = new HalfEdge(c);
            ca.twin = new HalfEdge(a);

            ab.twin.next = ca.twin;
            ab.twin.prev = bc.twin;

            ca.twin.next = bc.twin;
            ca.twin.prev = ab.twin;

            bc.twin.next = ab.twin;
            bc.twin.prev = ca.twin;

            outerFaces.Add(ab.twin);
            outerFaces.Add(bc.twin);
            outerFaces.Add(ca.twin);
            //Done bootstrapping

            HalfEdge onCH = ab.twin;

            while (initialPoints.Count != 0)
            {
                Vertex curV = initialPoints[initialPoints.Count - 1];
                initialPoints.RemoveAt(initialPoints.Count - 1);
                bool foundinface = false;
                Tuple<List<Face>, List<HalfEdge>> temp = null;
                for (int i = 0; i < triangles.Count; ++i)
                {
                    if (InFace(triangles[i], curV, 0))
                    {
                        foundinface = true;
                        temp = AddVertex(triangles[i], curV, 0);
                        onCH = temp.Item2[0];
                        triangles[i] = temp.Item1[temp.Item1.Count - 1];
                        temp.Item1.RemoveAt(temp.Item1.Count - 1);
                        triangles.AddRange(temp.Item1);
                        i = triangles.Count + 10;
                    }
                }
                if (!foundinface)
                {
                    HalfEdge leftMost = null;
                    for (int i = 0; i < outerFaces.Count; ++i)
                    {
                        if (SameSideOfPlane(outerFaces[i], curV, 0))
                        {
                            leftMost = outerFaces[i];
                            i = int.MaxValue;
                        }
                    }
                    while (SameSideOfPlane(leftMost.prev, curV, 0))
                        leftMost = leftMost.prev;
                    temp = AddExternalVertex(leftMost, curV, 0);
                    triangles.AddRange(temp.Item1);
                }
                edgesToCheck.AddRange(temp.Item2);
            }
            HalfEdge curE = null;
            while (edgesToCheck.Count != 0)
            {
                curE = edgesToCheck[edgesToCheck.Count - 1];
                if (IsLocalDelaunay(curE, 0))
                    curE.UpdateCert(Cert.DelaunayEdge, events);
                else
                {
                    List<HalfEdge> tempEs = Flip(curE);
                    for (int i = 0; i < tempEs.Count - 1; ++i)
                    {
                        if (tempEs[i].DelaunayEdgeCert())
                        {
                            tempEs[i].UpdateCert(null, events);
                            edgesToCheck.Add(tempEs[i]);
                        }
                    }
                }
            }
            return curE;
        }

        //Given a new face (in this code it will always have a convex boundary), triangulate it.
        public HalfEdge TriangulateRegion(Face f, double time)
        {
            HalfEdge curE = f.halfEdge;
            List<HalfEdge> edgesToCheck = new List<HalfEdge>();
            while (curE.next.next.next.CompareTo(curE) != 1)
            {
                Face newF = new Face(curE, time);
                HalfEdge newE = new HalfEdge(curE.next.next.vertex, new HalfEdge(curE.vertex));
                newE.next = curE;
                newE.prev = curE.next;
                newE.twin.twin = curE;
                newE.twin.next = curE.next.next;
                newE.twin.prev = curE.prev;

                curE.prev.next = newE.twin;
                curE.next.next.prev = newE.twin;
                curE.next.next = newE;
                curE.prev = newE;
                curE.incidentFace = newF;
                curE.next.incidentFace = newF;
                curE.next.next.incidentFace = newF;
                newE.twin.incidentFace = newE.prev.incidentFace;
                edgesToCheck.Add(newE);
            }
            while (edgesToCheck.Count != 0)
            {
                curE = edgesToCheck[edgesToCheck.Count - 1];
                if (IsLocalDelaunay(curE, 0))
                    curE.UpdateCert(Cert.DelaunayEdge, events);
                else
                {
                    List<HalfEdge> tempEs = Flip(curE);
                    for (int i = 0; i < tempEs.Count - 1; ++i)
                    {
                        if (tempEs[i].DelaunayEdgeCert())
                        {
                            tempEs[i].UpdateCert(null, events);
                            edgesToCheck.Add(tempEs[i]);
                        }
                    }
                }
            }
            return curE;
        }

        //Triangluate a small by the CH of a DT
        public HalfEdge CreateCH(HalfEdge left, Vertex right, double time)
        {
            List<HalfEdge> newEdges = new List<HalfEdge>();
            bool finished = false;
            HalfEdge curE = left;
            //The next edge will always be on the CH
            left = left.prev;
            while (!finished)
            {
                finished = true;
                while (curE.twin.vertex.CompareTo(right) != 1)
                {
                    if (CCW(left.vertex, left.next.vertex, left.next.next.vertex, time))
                    {
                        Face newF = new Face(left, time);
                        HalfEdge newE = new HalfEdge(curE.next.twin.prev.vertex);
                        newE.twin = new HalfEdge(curE.vertex);
                        newE.twin.twin = newE;
                        newE.twin.next = curE.next.next;
                        newE.twin.prev = curE.prev;
                        curE.prev.next = newE.twin;
                        curE.next.next.prev = newE.twin;
                        curE.prev = newE;
                        curE.next.next = newE;
                        newE.prev = curE.next;
                        newE.next = curE;

                        newE.twin.incidentFace = curE.incidentFace;
                        newE.incidentFace = newF;
                        newE.next.incidentFace = newF;
                        newE.next.next.incidentFace = newF;

                        finished = true;
                        newEdges.Add(newE.twin);
                        curE = newE;
                    }
                    else
                        curE = curE.next;
                }
                if (!finished)
                    curE = left.next;
            }
            while (newEdges.Count != 0)
            {
                curE = newEdges[newEdges.Count - 1];
                if (IsLocalDelaunay(curE, 0))
                    curE.UpdateCert(Cert.DelaunayEdge, events);
                else
                {
                    List<HalfEdge> tempEs = Flip(curE);
                    for (int i = 0; i < tempEs.Count - 1; ++i)
                    {
                        if (tempEs[i].DelaunayEdgeCert())
                        {
                            tempEs[i].UpdateCert(null, events);
                            newEdges.Add(tempEs[i]);
                        }
                    }
                }
            }
            AddInfEdgesToCH(left.next, time, curE.next);
            return left.next;
        }

        //Given a triangulated region and a pointer and a half edge of the convex hull, finish the DCEL. From each point on the CH, there is an edge that goes out to infinity.
        private void AddInfEdgesToCH(HalfEdge onCH, double time, HalfEdge end = null)
        {
            HalfEdge curE = onCH;
            curE.prev = new HalfEdge(Infinity);

            do
            {
                //Before changes are made.
                HalfEdge temp = curE.next;

                curE.incidentFace = new Face(curE, time);
                curE.prev.incidentFace = curE.incidentFace;

                curE.next = new HalfEdge(curE.twin.vertex);
                curE.next.incidentFace = curE.incidentFace;
                curE.next.prev = curE;

                curE.next.twin = new HalfEdge(Infinity);
                curE.next.twin.twin = curE.next;

                curE.next.next = curE.prev;
                curE.prev.prev = curE.next;

                curE = temp;
            } while (curE != onCH && curE != end);
        }
        #endregion

        #region DCEL Maniupuation

        //Flip a half edge along the quadrilateral it is the diagonal of.
        //Returns the four edges of the quadrilateral and the new edge made (only one of the half edges)
        public List<HalfEdge> Flip(HalfEdge e)
        {
            e.next.prev = e.twin.prev;
            e.twin.prev.next = e.next;
            e.prev.next = e.twin.next;
            e.twin.next.prev = e.prev;

            HalfEdge newE = new HalfEdge(e.twin.prev.vertex);
            newE.twin = new HalfEdge(e.prev.vertex);

            newE.twin.twin = newE;
            newE.prev = e.twin.next; ;
            newE.next = e.prev;
            newE.twin.next = e.twin.prev;
            newE.twin.prev = e.next;

            newE.incidentFace = e.incidentFace;
            newE.next.incidentFace = e.incidentFace;
            newE.next.next.incidentFace = e.incidentFace;

            newE.twin.incidentFace = e.twin.incidentFace;
            newE.twin.next.incidentFace = e.twin.incidentFace;
            newE.twin.next.next.incidentFace = e.twin.incidentFace;

            List<HalfEdge> ret = new List<HalfEdge>();
            ret.Add(newE);
            ret.Add(newE.next);
            ret.Add(newE.next);
            ret.Add(newE.next.next);
            ret.Add(newE.twin.next);
            ret.Add(newE.twin.next.next);
            return ret; ;
        }

        //Given the left most edge of the convex hull that has v on the same side of the half plane created by the edge, attach this vertex to the DCEL.
        //The first of the half edges returned will be the one on the convex hull.
        private Tuple<List<Face>, List<HalfEdge>> AddExternalVertex(HalfEdge leftMost, Vertex v, double time)
        {
            Tuple<List<Face>, List<HalfEdge>> ret = new Tuple<List<Face>, List<HalfEdge>>(new List<Face>(), new List<HalfEdge>());
            HalfEdge outE = new HalfEdge(leftMost.vertex), inE = new HalfEdge(v), curE = leftMost.next, leftSideOfOuterTriangle = null;
            Face tempF = new Face(leftMost, time);
            inE.next = leftMost;
            inE.prev = outE;
            outE.next = inE;
            inE.twin = new HalfEdge(leftMost.vertex, inE, null, null, leftMost.prev);
            leftMost.prev.next = inE.twin;
            leftMost.prev = inE;
            leftSideOfOuterTriangle = inE.twin;

            ret.Item1.Add(tempF);
            ret.Item2.Add(leftSideOfOuterTriangle);

            while (SameSideOfPlane(curE, v, time))
            {
                HalfEdge temp = curE.next;
                outE.twin = new HalfEdge(Infinity);
                outE.twin.twin = outE;

                inE = outE;
                outE = new HalfEdge(curE.twin.vertex);

                tempF = new Face(curE, time);
                inE.next = curE;
                curE.prev = inE;
                curE.next = outE;
                inE.prev = outE;
                outE.next = inE;
                curE = temp;

                ret.Item1.Add(tempF);
                ret.Item2.Add(outE);
            }
            outE.twin = new HalfEdge(Infinity, outE, null, curE, leftSideOfOuterTriangle);
            leftSideOfOuterTriangle.next = outE.twin;
            return ret;
        }

        //if a vertex is in a face, add the vertex to the decel structure, adding new faces, edges etc.
        //returns the new faces made.
        //if a vertex is in a face, add the vertex to the decel structure, adding new faces, edges etc.
        //returns the new faces made.
        private Tuple<List<Face>, List<HalfEdge>> AddVertex(Face f, Vertex v, double time)
        {
            if (InFace(f, v, time))
            {
                Vertex a = f.halfEdge.vertex;
                Vertex b = f.halfEdge.next.vertex;
                Vertex c = f.halfEdge.next.next.vertex;

                HalfEdge halfEdgeVToA = new HalfEdge(v);
                HalfEdge halfEdgeVToB = new HalfEdge(v);
                HalfEdge halfEdgeVToC = new HalfEdge(v);

                HalfEdge halfEdgeAToV = new HalfEdge(a);
                HalfEdge halfEdgeBToV = new HalfEdge(b);
                HalfEdge halfEdgeCToV = new HalfEdge(c);

                halfEdgeVToA.twin = halfEdgeAToV;
                halfEdgeVToA.next = a.halfEdge;
                halfEdgeVToA.prev = halfEdgeBToV;

                halfEdgeVToB.twin = halfEdgeBToV;
                halfEdgeVToB.next = b.halfEdge;
                halfEdgeVToB.prev = halfEdgeCToV;

                halfEdgeVToC.twin = halfEdgeCToV;
                halfEdgeVToC.next = c.halfEdge;
                halfEdgeVToC.prev = halfEdgeAToV;

                halfEdgeAToV.twin = halfEdgeVToA;
                halfEdgeAToV.next = halfEdgeVToC;
                halfEdgeAToV.prev = c.halfEdge;

                halfEdgeBToV.twin = halfEdgeVToB;
                halfEdgeBToV.next = halfEdgeVToA;
                halfEdgeBToV.prev = a.halfEdge;

                halfEdgeCToV.twin = halfEdgeVToC;
                halfEdgeCToV.next = halfEdgeVToB;
                halfEdgeCToV.prev = b.halfEdge;

                Face fa = new Face(halfEdgeVToA);
                Face fb = new Face(halfEdgeVToB);
                Face fc = new Face(halfEdgeVToC);

                halfEdgeVToA.incidentFace = fa;
                halfEdgeVToA.twin.incidentFace = fc;

                halfEdgeVToB.incidentFace = fb;
                halfEdgeVToB.twin.incidentFace = fa;

                halfEdgeVToC.incidentFace = fc;
                halfEdgeVToC.twin.incidentFace = fb;

                List<Face> faces = new List<Face>();
                faces.Add(fa);
                faces.Add(fb);
                faces.Add(fc);

                List<HalfEdge> halfEdges = new List<HalfEdge>();
                halfEdges.Add(halfEdgeAToV);
                halfEdges.Add(halfEdgeBToV);
                halfEdges.Add(halfEdgeCToV);
                halfEdges.Add(halfEdgeVToA);
                halfEdges.Add(halfEdgeVToB);
                halfEdges.Add(halfEdgeVToC);
                Tuple<List<Face>, List<HalfEdge>> tuple = new Tuple<List<Face>, List<HalfEdge>>(faces, halfEdges);
                return tuple;
            }
            else
            {
                return null;
            }
        }

        //Loop around a face and remove all certificates associated with it.
        public void RemoveBorderCert(Face f)
        {
            HalfEdge start = f.halfEdge;
            HalfEdge curE = start;
            do
            {
                curE.UpdatePriority(double.MinValue, events);
                events.Dequeue();
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
            initial.UpdateCert(ret[ret.Count - 1], events);
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
                    initial.next.UpdateCert(ret[ret.Count - 1], events);
                    BFSNextLevel.Add(initial.next);
                }
                if (initial.next.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next.next, time));
                    initial.next.next.UpdateCert(ret[ret.Count - 1], events);
                    BFSNextLevel.Add(initial.next.next);
                }
            }
            else
            {
                if (initial.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next, time));
                    initial.next.UpdateCert(ret[ret.Count - 1], events);
                    BFSNextLevel.Add(initial.next);
                }
                if (initial.next.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next.next, time));
                    initial.next.next.UpdateCert(ret[ret.Count - 1], events);
                    BFSNextLevel.Add(initial.next.next);
                }

                initial = initial.twin;
                if (initial.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next, time));
                    initial.next.UpdateCert(ret[ret.Count - 1], events);
                    BFSNextLevel.Add(initial.next);
                }
                if (initial.next.next.CertTime == -1)
                {
                    ret.Add(CreateCHCert(initial.next.next, time));
                    initial.next.next.UpdateCert(ret[ret.Count - 1], events);
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
            if (e.twin.incidentFace.timeCreated != time)
                e.twin.incidentFace.timeCreated = time;
            HalfEdge firstNonBroken = e;
            HalfEdge current = e;
            HalfEdge inE;
            HalfEdge outE;
            bool justVisited = false;
            while (current.CompareTo(firstNonBroken) != 1 || justVisited)
            {
                outE = current.twin.next;
                inE = current.twin.prev;
                if (current.CertTime == time && current.EdgeIs == EdgeType.Int)
                {
                    current.UpdatePriority(double.MinValue, events);
                    events.Dequeue();
                    if (current.CompareTo(firstNonBroken) == 1)
                    {
                        firstNonBroken = current.prev;//????? Might not work
                        justVisited = true;
                    }
                    //If looped around. Here we have an edge going to nowhere!
                    if (current.next.CompareTo(current.twin) == 1)
                    {
                        current.prev = current.twin.next;
                        current.twin.next = current.prev;
                        current = current.twin.next;
                    }
                    else
                    {
                        inE.next = current.next;
                        current.next.prev = inE;
                        outE.prev = current.prev;
                        current.prev.next = outE;

                        inE.next.incidentFace = current.twin.incidentFace;
                        outE.prev.incidentFace = current.twin.incidentFace;

                        if (current.vertex.halfEdge.CompareTo(current) == 1)
                            current.vertex.halfEdge = outE;
                        if (current.twin.vertex.halfEdge.CompareTo(current.twin) == 1)
                            current.twin.vertex.halfEdge = inE.next;
                        current = outE;
                    }
                    current.UpdatePriority(double.MinValue, events);
                    events.Dequeue();
                }
                else
                {
                    justVisited = false;
                    current = current.next;
                }
            }
            return current.incidentFace;
        }
        
        //Remove an edge with a face going to infinity and merge the faces. Return the resulting face. Also, remove all appropriate certificates of edges that belong to the same quadrilateral.
        //Recursively removes all edges of neighboring triangles if necessary.
        public Tuple<HalfEdge, Vertex, Face> RemoveInfEdges(HalfEdge e, double time)
        {
            //Note, this code breaks if everything goes on a single line. (This is unlikely and we can prevent the input from ever doing that)
            Tuple<HalfEdge, Vertex, Face> ret;
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

            //Go left for simplicity of code
            while (!finished)
            {
                if (current.EdgeIs == EdgeType.Inf && current.twin.prev.CertTime == time)
                    current = current.twin.prev;
                else if (current.EdgeIs == EdgeType.InfInf && current.prev.CertTime == time)
                    current = current.twin.prev;
                else
                    finished = true;
            }
            if (current.EdgeIs == EdgeType.Inf)
                leftInf = current;
            else
                leftInf = current.prev;
            current.incidentFace.timeCreated = time;
            while (current.CertTime != time)
            {
                if (current.EdgeIs == EdgeType.Inf)
                {
                    current.twin.prev.next = current.next;
                    current.next.prev = current.twin.prev.next;
                    current.twin.prev.prev.incidentFace = current.incidentFace;
                    current.twin.prev.incidentFace = current.incidentFace;
                    current.UpdatePriority(double.MinValue, events);
                    events.Dequeue();
                    if (current.vertex.halfEdge.CompareTo(current) == 1)
                    {

                        current.vertex.halfEdge = current.prev.twin;
                    }
                    current = current.next;
                }
                else
                {
                    current.prev.next = current.twin.next;
                    current.twin.next.prev = current.prev;
                    current.next.prev = current.twin.prev;
                    current.twin.prev.next = current.next;
                    current.UpdatePriority(double.MinValue, events);
                    events.Dequeue();
                    if (current.vertex.halfEdge.CompareTo(current) == 1)
                    {
                        current.vertex.halfEdge = current.twin.next;
                    }
                    current = current.next.twin;
                }
            }
            if (current.EdgeIs == EdgeType.Inf)
            {
                current.twin.next = leftInf;
                leftInf.prev = current.twin.next;
                ret = new Tuple<HalfEdge, Vertex, Face>(leftInf.next, current.twin.vertex, current.incidentFace);
            }
            else
            {
                current.next.next = leftInf;
                leftInf.prev = current.next;
                ret = new Tuple<HalfEdge, Vertex, Face>(leftInf.next, current.next.vertex, current.incidentFace);
            }
            return ret;
        }

        #endregion
       
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
                    Cert curC = events.Dequeue();
                    Face curF;
                    HalfEdge curE;
                    if (curC.internalCert)
                    {
                        curF = (RemoveInternalEdges(curC.edge, curT));
                        RemoveBorderCert(curF);
                        curE = TriangulateRegion(curF, time);
                    }
                    else
                    {
                        Tuple<HalfEdge, Vertex, Face> temp = (RemoveInfEdges(curC.edge, curT));
                        curF = temp.Item3;
                        RemoveBorderCert(curF);
                        curE = CreateCH(temp.Item1, temp.Item2, time);
                    }
                    List<Cert> tempCs = BFSCreateCert(curE, curT);
                    List<double> priors = new List<double>();
                    for (int i = 0; i < tempCs.Count; ++i)
                    {
                        priors.Add(tempCs[i].timeCreated);
                    }
                    List<PQKey> keys = events.EnqeueList(tempCs, priors);
                    for (int i = 0; i < tempCs.Count; ++i)
                    {
                        tempCs[i].key = keys[i];
                    }
                }
            }
            return curT;
        }
       
        #region Unfinished Methods
        public bool CCW(Vertex a, Vertex b, Vertex c, double time)
        {
            return true;
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
        #endregion
    }
}
