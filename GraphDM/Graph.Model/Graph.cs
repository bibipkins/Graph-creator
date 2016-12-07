using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraphModel
{
    public class Vertex
    {
        public Vertex(int id, double x, double y)
        {
            this.ID = id;
            this.X = x;
            this.Y = y;
        }
        public Vertex(Vertex v)
        {
            this.ID = v.ID;
            this.X = v.X;
            this.Y = v.Y;
            this.Marked = v.Marked;
        }

        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool Marked { get; set; }

        public override string ToString()
        {
            return "Вершина: v" + ID + " (" + X + ", " + Y + ")";
        }
    }

    public class Edge
    {
        public Edge(Edge e)
        {
            this.V1 = e.V1;
            this.V2 = e.V2;
            this.Weight = e.Weight;
            this.Marked = e.Marked;
        }
        public Edge(Vertex v1, Vertex v2, int weight = 0)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.Weight = weight;
        }

        public Vertex V1 { get; set; }
        public Vertex V2 { get; set; }
        public int Weight { get; set; }
        public bool Marked { get; set; }

        public override string ToString()
        {
            return "Ребро: (" + V1.ID + ", " + V2.ID + "), вага: " + 
                Weight;
        }
    }

    public class Graph
    {
        public Graph(bool oriented = false)
        {
            this.Oriented = oriented;
        }

        public void AddVertex(double x, double y)
        {
            int id = GenerateID();
            AddVertex(new Vertex(id, x, y));
        }
        public void AddVertex(Vertex v)
        {
            _vertices.Add(v);
        }
        public void CopyVertex(Vertex v)
        {
            _vertices.Add(new Vertex(v.ID, v.X, v.Y));
        }
        public Vertex VertexAt(int i)
        {
            return _vertices[i];
        }
        public Vertex GetVertex(int id)
        {
            return _vertices.Find(v => (v.ID == id));
        }
        public bool RemoveVertex(Vertex v)
        {
            return RemoveVertex(v.ID);
        }
        public bool RemoveVertex(int id)
        {
            Vertex removeVertex = GetVertex(id);

            if (removeVertex != null)
            {
                _edges.RemoveAll(e => (e.V1.ID == id || e.V2.ID == id));
                _vertices.Remove(removeVertex);
                return true;
            }

            return false;
        } 
        public bool MarkVertex(Vertex v, bool mark)
        {
            return MarkVertex(v.ID, mark);
        }
        public bool MarkVertex(int id, bool mark)
        {
            Vertex v = GetVertex(id);

            if(v != null)
            {
                v.Marked = mark;
                return true;
            }

            return false;
        }
        public void MarkAllVertices(bool mark)
        {
            foreach (Vertex v in _vertices)
                v.Marked = mark;
        }
        public int VertexCount
        {
            get
            {
                return _vertices.Count;
            }
        }

        public bool AddEdge(Vertex v1, Vertex v2, int weight = 0)
        {
            if (v1 != v2 && _vertices.Contains(v1) && _vertices.Contains(v2))
            {
                return AddEdge(new Edge(v1, v2, weight)); ;
            }

            return false;
        }
        public bool AddEdge(int id1, int id2, int weight = 0)
        {
            Vertex v1 = GetVertex(id1);
            Vertex v2 = GetVertex(id2);
            Edge checkEdge = GetEdge(id1, id2);

            if (v1 != v2 && v1 != null && v2 != null && checkEdge == null)
            {
                return AddEdge(new Edge(v1, v2, weight));
            }

            return false;
        }
        public bool AddEdge(Edge e)
        {
            if (e != null && GetEdge(e.V1, e.V2) == null && GetEdge(e.V2, e.V1) == null && 
                _vertices.Contains(e.V1) && _vertices.Contains(e.V2) && e.V1 != e.V2)
            {
                _edges.Add(e);
                return true;
            }

            return false;
        }
        public bool CopyEdge(Edge e)
        {
            if (e != null && !_edges.Contains(e) && _vertices.Contains(e.V1) && _vertices.Contains(e.V2) && e.V1 != e.V2)
            {
                _edges.Add(new Edge(e));
                return true;
            }

            return false;
        }
        public Edge EdgeAt(int i)
        {
            return _edges[i];
        }
        public Edge GetEdge(Vertex v1, Vertex v2)
        {
            return GetEdge(v1.ID, v2.ID);
        }
        public Edge GetEdge(int id1, int id2)
        {
            return _edges.Find(e => (e.V1.ID == id1 && e.V2.ID == id2));
        }
        public bool RemoveEdge(Edge e)
        {
            return _edges.Remove(e);
        }
        public bool RemoveEdge(Vertex v1, Vertex v2)
        {
            return RemoveEdge(v1.ID, v2.ID);
        }
        public bool RemoveEdge(int id1, int id2)
        {
            Edge removeEdge = GetEdge(id1, id2);

            if (removeEdge != null)
            {
                return _edges.Remove(removeEdge);
            }

            return false;
        }
        public bool MarkEdge(Vertex v1, Vertex v2, bool mark)
        {
            return MarkEdge(v1.ID, v2.ID, mark);
        }
        public bool MarkEdge(int id1, int id2, bool mark)
        {
            Edge e = GetEdge(id1, id2);

            if (e != null)
            {
                e.Marked = mark;
                return true;
            }

            return false;
        }
        public void MarkAllEdges(bool mark)
        {
            foreach (Edge e in _edges)
                e.Marked = mark;
        }
        public int EdgeCount
        {
            get
            {
                return _edges.Count;
            }
        }
        
        public void Clear()
        {
            _vertices.Clear();
            _edges.Clear();
        }

        public int GenerateID()
        {
            int maxID = -1;
            if (_vertices.Count > 0)
                maxID = _vertices[0].ID;

            foreach (Vertex v in _vertices)
                if (v.ID > maxID)
                    maxID = v.ID;

            return maxID + 1;
        }
        public double VerticesDistance(Vertex V1, Vertex V2)
        {
            return Math.Sqrt((V1.X - V2.X) * (V1.X - V2.X) + (V1.Y - V2.Y) * (V1.Y - V2.Y));
        }
        public bool PathExists(Vertex startVertex, Vertex endVertex)
        {
            List<Vertex> vertices = new List<Vertex>(_vertices);
            List<Edge> edges = new List<Edge>(_edges);

            Queue<Vertex> vertexQueue = new Queue<Vertex>();
            List<Vertex> visitedVertices = new List<Vertex>();

            vertexQueue.Enqueue(startVertex);

            while (vertexQueue.Count > 0)
            {
                Vertex currentVertex = vertexQueue.Dequeue();
                if (currentVertex.ID == endVertex.ID)
                    return true;

                visitedVertices.Add(currentVertex);

                foreach (Edge e in edges)
                {
                    if (e.V1.ID == currentVertex.ID && !visitedVertices.Exists(v => v.ID == e.V2.ID))
                        vertexQueue.Enqueue(e.V2);
                    else if (e.V2.ID == currentVertex.ID && !visitedVertices.Exists(v => v.ID == e.V1.ID))
                        vertexQueue.Enqueue(e.V1);
                }
            }

            return false;
        }

        public bool Oriented 
        { 
            get; 
            set; 
        }
        public Vertex Center
        {
            get
            {
                Vertex center = new Vertex(0, 0, 0);

                foreach (Vertex v in _vertices)
                {
                    center.X += v.X;
                    center.Y += v.Y;
                }

                center.X /= _vertices.Count;
                center.Y /= _vertices.Count;

                return center;
            }
        }
        public double MeanDistanceToCenter
        {
            get
            {
                Vertex center = Center;
                double meanDistance = 0;

                foreach (Vertex v in _vertices)
                    meanDistance += VerticesDistance(v, center);

                meanDistance /= _vertices.Count;

                return meanDistance;
            }
        }
        public List<Vertex> Vertices
        {
            get
            {
                return _vertices;
            }
        }
        public List<Vertex> OddVertices
        {
            get
            {
                List<Vertex> oddVertices = new List<Vertex>();

                foreach (Vertex v in _vertices)
                {
                    int degree = 0;
                    foreach (Edge e in _edges)
                        if (e.V1.ID == v.ID || e.V2.ID == v.ID)
                            degree++;
                    if (degree % 2 != 0)
                        oddVertices.Add(v);
                }

                return oddVertices;
            }
        }
        public List<Edge> Edges
        {
            get
            {
                return _edges;
            }
        }
        
        private List<Vertex> _vertices = new List<Vertex>();
        private List<Edge> _edges = new List<Edge>();
    }

    public class GraphGenerator
    {
        public GraphGenerator(int gridSizeX, int gridSizeY, 
            double gridStrideX = 100, double gridStrideY = 100)
        {
            this.GridSizeX = gridSizeX;
            this.GridSizeY = gridSizeY;
            this.GridStrideX = gridStrideX;
            this.GridStrideY = gridStrideY;
            this.GridLeft = 0;
            this.GridTop = 0;
        }

        public Graph GenerateGraph(int vertexNumber, double meanDistanceToCenter, bool oriented = false)
        {
            _graph = new Graph(oriented);
            
            GenerateVertices(vertexNumber, meanDistanceToCenter);
            GenerateEdges();

            return _graph;
        }
        public Graph GenerateOddGraph(int vertexNumber, int oddNumber, double meanDistanceToCenter, bool oriented = false)
        {
            bool incorrectGraph = false;

            do
            {
                incorrectGraph = false;

                _graph = new Graph(oriented);

                GenerateVertices(vertexNumber, meanDistanceToCenter);
                GenerateEdges();

                while (_graph.OddVertices.Count < oddNumber)
                {
                    List<Vertex> allowedVertices = new List<Vertex>(_graph.Vertices);
                    foreach (Vertex v in _graph.OddVertices)
                        allowedVertices.Remove(v);

                    List<Edge> allowedEdges = new List<Edge>();
                    for (int i = 0; i < allowedVertices.Count - 1; i++)
                        for (int j = i + 1; j < allowedVertices.Count; j++)
                        {
                            Edge edgeToAdd = new Edge(allowedVertices[i], allowedVertices[j], _random.Next(100));

                            if (EdgeAvailable(edgeToAdd) && !_graph.Edges.Exists(e => (e.V1.ID == edgeToAdd.V1.ID && e.V2.ID == edgeToAdd.V2.ID) ||
                                                                                      (e.V2.ID == edgeToAdd.V1.ID && e.V1.ID == edgeToAdd.V2.ID)))
                                allowedEdges.Add(edgeToAdd);
                        }

                    if (allowedEdges.Count > 0)
                    {
                        Edge edgeToAdd = allowedEdges[_random.Next(allowedEdges.Count)];
                        _graph.AddEdge(edgeToAdd);
                    }
                    else
                    {
                        List<Edge> canRemoveEdges = new List<Edge>();
                        for (int i = 0; i < allowedVertices.Count - 1; i++)
                            for (int j = i + 1; j < allowedVertices.Count; j++)
                            {
                                Edge edge = _graph.Edges.Find(e =>
                                                    (e.V1.ID == allowedVertices[i].ID && e.V2.ID == allowedVertices[j].ID) ||
                                                    (e.V2.ID == allowedVertices[i].ID && e.V1.ID == allowedVertices[j].ID));

                                if (edge != null)
                                {
                                    _graph.RemoveEdge(edge);

                                    if (_graph.PathExists(edge.V1, edge.V2))
                                        canRemoveEdges.Add(edge);

                                    _graph.AddEdge(edge);
                                }
                            }

                        if (canRemoveEdges.Count > 0)
                        {
                            Edge edgeToRemove = canRemoveEdges[_random.Next(canRemoveEdges.Count)];
                            _graph.RemoveEdge(edgeToRemove);
                        }
                        else
                        {
                            incorrectGraph = true;
                            break;
                        }
                    }
                }

                while (_graph.OddVertices.Count > oddNumber)
                {
                    List<Vertex> allowedVertices = new List<Vertex>(_graph.OddVertices);

                    List<Edge> canRemoveEdges = new List<Edge>();
                    for (int i = 0; i < allowedVertices.Count - 1; i++)
                        for (int j = i + 1; j < allowedVertices.Count; j++)
                        {
                            Edge edge = _graph.Edges.Find(e =>
                                                (e.V1.ID == allowedVertices[i].ID && e.V2.ID == allowedVertices[j].ID) ||
                                                (e.V2.ID == allowedVertices[i].ID && e.V1.ID == allowedVertices[j].ID));

                            if (edge != null)
                            {
                                _graph.RemoveEdge(edge);

                                if (_graph.PathExists(edge.V1, edge.V2))
                                    canRemoveEdges.Add(edge);

                                _graph.AddEdge(edge);
                            }
                        }

                    if (canRemoveEdges.Count > 0)
                    {
                        Edge edgeToRemove = canRemoveEdges[_random.Next(canRemoveEdges.Count)];
                        _graph.RemoveEdge(edgeToRemove);
                    }
                    else
                    {
                        incorrectGraph = true;
                        break;
                    }
                }

                if (_graph.OddVertices.Count != oddNumber)
                    incorrectGraph = true;

            } while (incorrectGraph);

            return _graph;
        }
        public Graph GenerateFlow(int vertexNumber, double meanDistanceToCenter)
        {
            bool correctGraph = true;

            do
            {
                _graph = new Graph(true);
                correctGraph = true;

                GenerateVertices(vertexNumber, meanDistanceToCenter);
                GenerateFlowEdges();

                List<Vertex> vertices = _graph.Vertices;
                List<Edge> edges = _graph.Edges;

                Vertex source = null;
                Vertex sink = null;
                Vertex center = _graph.Center;

                double distanceToCenterMax = 0;
                foreach (Vertex v in vertices)
                {
                    double distanceToCenter = _graph.VerticesDistance(v, center);
                    if (distanceToCenter > distanceToCenterMax)
                    {
                        distanceToCenterMax = distanceToCenter;
                        source = v;
                    }
                }

                double distanceToSinkMax = 0;
                foreach (Vertex v in vertices)
                {
                    double distanceToSink = _graph.VerticesDistance(v, source);
                    if (distanceToSink > distanceToSinkMax)
                    {
                        distanceToSinkMax = distanceToSink;
                        sink = v;
                    }
                }

                source.ID = 0;
                sink.ID = vertices.Count - 1;

                int idCounter = 1;
                foreach(Vertex v in vertices)
                    if (v != sink && v != source)
                    {
                        v.ID = idCounter;
                        idCounter++;
                    }

                _graph.Vertices.Sort(delegate(Vertex v1, Vertex v2) { return v1.ID.CompareTo(v2.ID); });

                foreach(Edge e in edges)
                {
                    if (e.V2 == source)
                    {
                        Vertex temp = e.V2;
                        e.V2 = e.V1;
                        e.V1 = temp;
                    }
                    if(e.V1 == sink)
                    {
                        Vertex temp = e.V2;
                        e.V2 = e.V1;
                        e.V1 = temp;
                    }
                }

                foreach(Vertex v in vertices)
                {
                    if(v!= source && v!= sink)
                    {
                        List<Edge> edgesIn = new List<Edge>();
                        List<Edge> edgesOut = new List<Edge>();

                        foreach(Edge e in edges)
                        {
                            if (e.V1 == v)
                                edgesOut.Add(e);
                            else if (e.V2 == v)
                                edgesIn.Add(e);
                        }

                        if(edgesIn.Count == 0)
                        {
                            List<Edge> allowedEdges = new List<Edge>(edgesOut);
                            Edge edgeToRevers = null;
                            while (edgeToRevers == null && allowedEdges.Count > 0)
                            {
                                edgeToRevers = allowedEdges[_random.Next(allowedEdges.Count)];
                                if(edgeToRevers.V2 == sink)
                                {
                                    allowedEdges.Remove(edgeToRevers);
                                    edgeToRevers = null;
                                }
                            }

                            if (edgeToRevers == null)
                            {
                                correctGraph = false;
                                break;
                            }
                            else
                            {
                                Vertex temp = edgeToRevers.V2;
                                edgeToRevers.V2 = edgeToRevers.V1;
                                edgeToRevers.V1 = temp;
                            }
                        }

                        if (edgesOut.Count == 0)
                        {
                            List<Edge> allowedEdges = new List<Edge>(edgesIn);
                            Edge edgeToRevers = null;
                            while (edgeToRevers == null && allowedEdges.Count > 0)
                            {
                                edgeToRevers = allowedEdges[_random.Next(allowedEdges.Count)];
                                if (edgeToRevers.V1 == source)
                                {
                                    allowedEdges.Remove(edgeToRevers);
                                    edgeToRevers = null;
                                }
                            }

                            if (edgeToRevers == null)
                            {
                                correctGraph = false;
                                break;
                            }
                            else
                            {
                                Vertex temp = edgeToRevers.V2;
                                edgeToRevers.V2 = edgeToRevers.V1;
                                edgeToRevers.V1 = temp;
                            }
                        }
                    }
                }

                foreach (Vertex v in vertices)
                {
                    if (v != source && v != sink)
                    {
                        int edgesIn = 0;
                        int edgesOut = 0;

                        foreach (Edge e in edges)
                        {
                            if (e.V1 == v)
                                edgesOut++;
                            else if (e.V2 == v)
                                edgesIn++;
                        }

                        if (edgesIn == 0 || edgesOut == 0)
                        {
                            correctGraph = false;
                            break;
                        }
                    } 
                }

            } while (!correctGraph);

            return _graph;
        }

        private void GenerateVertices(int vertexNumber, double meanDistanceToCenter)
        {
            int[,] vertexGrid = new int[GridSizeX, GridSizeY];

            do
            {
                _graph.Clear();

                for (int i = 0; i < GridSizeX; i++)
                    for (int j = 0; j < GridSizeY; j++)
                        vertexGrid[i, j] = -1;

                for (int i = 0; i < vertexNumber; i++)
                {
                    bool available = false;

                    do
                    {
                        int posI = _random.Next(GridSizeX);
                        int posJ = _random.Next(GridSizeY);

                        if (vertexGrid[posI, posJ] == -1)
                        {
                            double x = GridLeft + posI * GridStrideX;
                            double y = GridTop + posJ * GridStrideY;
                            _graph.AddVertex(x, y);
                            vertexGrid[posI, posJ] = _graph.VertexAt(_graph.VertexCount - 1).ID;

                            available = true;
                        }
                    } while (!available);
                }
                
            } while (_graph.MeanDistanceToCenter < meanDistanceToCenter);
        }
        private void GenerateEdges()
        {
            List<Vertex> sortedVertices = new List<Vertex>(_graph.Vertices);
            Vertex center = _graph.Center;

            sortedVertices.Sort(delegate(Vertex v1, Vertex v2)
            {
                double dx1 = v1.X - center.X;
                double dy1 = v1.Y - center.Y;
                double a1 = Math.Atan2(dy1, dx1);

                double dx2 = v2.X - center.X;
                double dy2 = v2.Y - center.Y;
                double a2 = Math.Atan2(dy2, dx2);

                if (a1 == a2)
                {
                    double d1 = dx1 * dx1 + dy1 * dy1;
                    double d2 = dx2 * dx2 + dy2 * dy2;
                    return d1.CompareTo(d2);
                }

                return a1.CompareTo(a2);
            });

            for (int i = 0; i < sortedVertices.Count; i++)
            {
                Edge edgeToAdd = new Edge(sortedVertices[i], sortedVertices[(i + 1) % sortedVertices.Count], _random.Next(100));

                if (i == sortedVertices.Count - 1)
                {
                    if (EdgeAvailable(edgeToAdd))
                        _graph.AddEdge(edgeToAdd);
                }
                else
                {
                    _graph.AddEdge(edgeToAdd);
                }
            }

            for (int i = 0; i < sortedVertices.Count; i++)
            {
                int addNumber = _random.Next(_random.Next(2), 5);
                List<Vertex> allowedVertices = new List<Vertex>(sortedVertices);
                allowedVertices.Remove(sortedVertices[i]);

                foreach (Edge e in _graph.Edges)
                {
                    if (e.V1.ID == sortedVertices[i].ID)
                        allowedVertices.Remove(e.V2);
                    else if (e.V2.ID == sortedVertices[i].ID)
                        allowedVertices.Remove(e.V1);
                }

                if (_random.Next(5) == 0)
                {
                    List<Edge> connectedEdges = new List<Edge>();
                    foreach (Edge e in _graph.Edges)
                        if (e.V1.ID == sortedVertices[i].ID || e.V2.ID == sortedVertices[i].ID)
                            connectedEdges.Add(e);

                    if(connectedEdges.Count != 0)
                    {
                        Edge edgeToRemove = connectedEdges[_random.Next(connectedEdges.Count)];
                        if (edgeToRemove.V1.ID == sortedVertices[i].ID)
                            allowedVertices.Remove(edgeToRemove.V2);
                        else if (edgeToRemove.V2.ID == sortedVertices[i].ID)
                            allowedVertices.Remove(edgeToRemove.V1);

                        _graph.RemoveEdge(edgeToRemove);

                        foreach (Vertex v in sortedVertices)
                        {
                            if (v.ID != sortedVertices[i].ID && !_graph.PathExists(v, sortedVertices[i]))
                            {
                                _graph.AddEdge(edgeToRemove);
                                break;
                            }
                        }
                    }
                }

                for (int count = 0; count < addNumber; count++)
                {
                    while (allowedVertices.Count > 0)
                    {
                        Vertex vertexToConnect = allowedVertices[_random.Next(allowedVertices.Count)];
                        Edge edgeToAdd = new Edge(sortedVertices[i], vertexToConnect, _random.Next(100));

                        if (EdgeAvailable(edgeToAdd))
                        {
                            _graph.AddEdge(edgeToAdd);
                            allowedVertices.Remove(vertexToConnect);
                            break;
                        }
                        else
                        {
                            allowedVertices.Remove(vertexToConnect);
                        }
                    }
                }
            }
        }
        private void GenerateFlowEdges()
        {
            List<Vertex> sortedVertices = new List<Vertex>(_graph.Vertices);
            Vertex center = _graph.Center;

            sortedVertices.Sort(delegate(Vertex v1, Vertex v2)
            {
                double dx1 = v1.X - center.X;
                double dy1 = v1.Y - center.Y;
                double a1 = Math.Atan2(dy1, dx1);

                double dx2 = v2.X - center.X;
                double dy2 = v2.Y - center.Y;
                double a2 = Math.Atan2(dy2, dx2);

                if (a1 == a2)
                {
                    double d1 = dx1 * dx1 + dy1 * dy1;
                    double d2 = dx2 * dx2 + dy2 * dy2;
                    return d1.CompareTo(d2);
                }

                return a1.CompareTo(a2);
            });

            for (int i = 0; i < sortedVertices.Count; i++)
            {
                Edge edgeToAdd = new Edge(sortedVertices[i], sortedVertices[(i + 1) % sortedVertices.Count], _random.Next(100));

                if (EdgeAvailable(edgeToAdd))
                    _graph.AddEdge(edgeToAdd);
            }

            for (int i = 0; i < sortedVertices.Count; i++)
            {
                int addNumber = _random.Next(_random.Next(2), 5);
                List<Vertex> allowedVertices = new List<Vertex>(sortedVertices);
                allowedVertices.Remove(sortedVertices[i]);

                foreach (Edge e in _graph.Edges)
                {
                    if (e.V1.ID == sortedVertices[i].ID)
                        allowedVertices.Remove(e.V2);
                    else if (e.V2.ID == sortedVertices[i].ID)
                        allowedVertices.Remove(e.V1);
                }

                for (int count = 0; count < addNumber; count++)
                {
                    while (allowedVertices.Count > 0)
                    {
                        Vertex vertexToConnect = allowedVertices[_random.Next(allowedVertices.Count)];
                        Edge edgeToAdd = new Edge(sortedVertices[i], vertexToConnect, _random.Next(100));

                        if (EdgeAvailable(edgeToAdd))
                        {
                            _graph.AddEdge(edgeToAdd);
                            allowedVertices.Remove(vertexToConnect);
                            break;
                        }
                        else
                        {
                            allowedVertices.Remove(vertexToConnect);
                        }
                    }
                }
            }
        }

        private int Orientation(Vertex p, Vertex q, Vertex r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0;  // colinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }
        private bool OnSegment(Vertex p, Vertex q, Vertex r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }
        public bool SegmentsIntersect(Vertex p1, Vertex q1, Vertex p2, Vertex q2)
        {
            bool touch = (p1.X == p2.X && p1.Y == p2.Y) ||
                         (p1.X == q2.X && p1.Y == q2.Y) ||
                         (q1.X == p2.X && q1.Y == p2.Y) ||
                         (q1.X == q2.X && q1.Y == q2.Y);

            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            if(touch)
            {
                if(o1==0 && o2==0 && o3==0 && o4==0)
                {
                    if ((p1.X == p2.X && p1.Y == p2.Y && !OnSegment(p1, q2, q1) && !OnSegment(p2, q1, q2)) ||
                        (p1.X == q2.X && p1.Y == q2.Y && !OnSegment(p1, p2, q1) && !OnSegment(p2, q1, q2)) ||
                        (q1.X == p2.X && q1.Y == p2.Y && !OnSegment(p1, q2, q1) && !OnSegment(p2, p1, q2)) ||
                        (q1.X == q2.X && q1.Y == q2.Y && !OnSegment(p1, p2, q1) && !OnSegment(p2, p1, q2))
                       )
                        return false;
                    else
                        return true;
                }
                else
                {
                    return false;
                }
            }

            if (o1 != o2 && o3 != o4)
                return true;

            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false;
        }
        private bool EdgeAvailable(Edge edgeForCheck)
        {
            foreach (Edge e in _graph.Edges)
                if (SegmentsIntersect(edgeForCheck.V1, edgeForCheck.V2, e.V1, e.V2))
                    return false;

            return true;
        }

        public int GridSizeX
        {
            get
            {
                return _gridSizeX;
            }
            set
            {
                if (value > 0)
                    _gridSizeX = value;
                else
                {
                    throw new Exception("Wrong grid size!");
                }
            }
        }
        public int GridSizeY
        {
            get
            {
                return _gridSizeY;
            }
            set
            {
                if (value > 0)
                    _gridSizeY = value;
                else
                {
                    throw new Exception("Wrong grid size!");
                }
            }
        }
        public double GridStrideX
        {
            get
            {
                return _gridStrideX;
            }
            set
            {
                if (value > 0)
                    _gridStrideX = value;
                else
                {
                    throw new Exception("Wrong grid stride!");
                }
            }
        }
        public double GridStrideY
        {
            get
            {
                return _gridStrideY;
            }
            set
            {
                if (value > 0)
                    _gridStrideY = value;
                else
                {
                    throw new Exception("Wrong grid stride!");
                }
            }
        }
        public double GridLeft { get; set; }
        public double GridTop { get; set; }

        private int _gridSizeX;
        private int _gridSizeY;
        private double _gridStrideX;
        private double _gridStrideY;

        private Graph _graph = null;
        private Random _random = new Random();
    }
}