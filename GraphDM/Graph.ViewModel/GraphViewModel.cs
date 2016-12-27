using GraphModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace GraphViewModel
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        public GraphViewModel()
        {
            InitCommands();

            _generator = new GraphGenerator(_gridSizeX, _gridSizeY, 100, 100) { GridLeft = 100, GridTop = 30 };
        }

        public void GenerateGraph()
        {
            ClearGraph();
            TextOutput = "";

            int vertexNumber = int.Parse(VertexNumber);
            int oddNumber = int.Parse(OddNumber);

            if (Lab1Radio)
                _graph = _generator.GenerateGraph(vertexNumber, 100);
            else if (Lab2Radio)
                _graph = _generator.GenerateOddGraph(vertexNumber, oddNumber, 100);
            else if (Lab3Radio)
                _graph = _generator.GenerateFlow(vertexNumber, 100);
            else if (Lab4Radio)
                _graph = _generator.GenerateGraph(vertexNumber, 100, Lab4Check);

            TextOutput += "\n\n\t\tГраф згенеровано\n\n\tСписок вершин:";
            foreach (Vertex v in _graph.Vertices)
                TextOutput += "\t" + v.ToString() + "\n";
            TextOutput += "\n\n\tСписок ребер:\n";
            foreach (Edge e in _graph.Edges)
                TextOutput += "\t" + e.ToString() + "\n";

            OnPropertyChanged("TextOutput");
            OnPropertyChanged("VertexItems");
            OnPropertyChanged("EdgeItems");
        }
        public void AddVertex(double x, double y)
        {
            if (CheckVertexHit(x, y, 40) == null)
            {
                _graph.AddVertex(x, y);
                Vertex v = CheckVertexHit(x, y, 40);
                TextOutput += "\tВершина v" + v.ID + " (" + x + ", " + y + ") додана\n";
                OnPropertyChanged("VertexItems");
            }
        }
        public void RemoveVertex(double x, double y)
        {
            Vertex removeVertex = CheckVertexHit(x, y, 15);
            if(removeVertex != null)
            {
                if(_graph.RemoveVertex(removeVertex))
                    TextOutput += "\tВершина v" + removeVertex.ID + " видалена\n";
            }

            Deselect();

            OnPropertyChanged("VertexItems");
            OnPropertyChanged("EdgeItems");
        }
        public void AddEdge(Vertex v1, Vertex v2)
        {
            if (char.IsDigit(EdgeWeight, EdgeWeight.Length - 1))
            {
                int w = int.Parse(EdgeWeight);
                if(_graph.AddEdge(v1, v2, w))
                    TextOutput += "\tРебро e(" + v1.ID + ", " + v2.ID + "), вага: " + w + " додане\n";

                Deselect();

                OnPropertyChanged("VertexItems");
                OnPropertyChanged("EdgeItems");
            }
            else return;
        }
        public void RemoveEdge(Vertex v1, Vertex v2)
        {
            if (v1 != null && v2 != null && v1 != v2)
            {
                if(_graph.RemoveEdge(v1, v2))
                    TextOutput += "\tРебро e(" + v1.ID + ", " + v2.ID + ") видалене\n";
                if(_graph.RemoveEdge(v2, v1))
                    TextOutput += "\tРебро e(" + v2.ID + ", " + v1.ID + ") видалене\n"; 
            }

            Deselect();

            OnPropertyChanged("EdgeItems");
            OnPropertyChanged("VertexItems");
        }
        public void ClearGraph()
        {
            _graph.Clear();

            TextOutput = _startLine;

            OnPropertyChanged("VertexItems");
            OnPropertyChanged("EdgeItems");
        }
        public Graph FindMinTree()
        {
            TextOutput += "\t\tПошук мінімального дерева:\n\n";

            _graph.MarkAllEdges(false);

            Graph minTreeGraph = new Graph();
            foreach (Vertex v in _graph.Vertices)
                minTreeGraph.AddVertex(v);

            List<Edge> edgeList = new List<Edge>(_graph.Edges);
            edgeList.Sort((e1, e2) => e1.Weight.CompareTo(e2.Weight));

            TextOutput += "\tВідсортований список ребер:\n\n";
            for (int i = 0; i < edgeList.Count; i++)
                TextOutput += "\t" + i + ". " + edgeList[i] + "\n";
            TextOutput += "\n";

            int addedEdges = 0;

            foreach (Edge e in edgeList)
            {
                minTreeGraph.AddEdge(e);

                if (CheckForCycle(minTreeGraph, e))
                {
                    minTreeGraph.RemoveEdge(e);

                    TextOutput += "\t" + e + " відкинуто\n";
                }
                else
                {
                    _graph.GetEdge(e.V1, e.V2).Marked = true;

                    TextOutput += "\t" + e + " додано\n";

                    if (++addedEdges == _graph.VertexCount - 1)
                    {
                        break;
                    }
                }
            }

            TextOutput += "\n\tМінімальне дерево знайдено!\n\n";

            OnPropertyChanged("VertexItems");
            OnPropertyChanged("EdgeItems");

            return minTreeGraph;
        }

        public void LeftMouseDragStart(double x, double y)
        {
            _leftMouseStartVertex = CheckVertexHit(x, y, 15);
        }
        public void LeftMouseDragEnd(double x, double y)
        {
            if (_leftMouseStartVertex != null)
            {
                Vertex endVertex = CheckVertexHit(x, y, 15);
                if (endVertex != null && endVertex != _leftMouseStartVertex)
                    AddEdge(_leftMouseStartVertex, endVertex);
            }

            _leftMouseStartVertex = null;
        }
        public void SelectVertices(double x, double y)
        {
            Vertex v = CheckVertexHit(x, y, 15);

            if (v != null)
            {
                if (_selectStartVertex == null)
                {
                    _selectStartVertex = v;
                    _selectStartVertex.Marked = true;
                }
                else if(_selectEndVertex == null)
                {
                    _selectEndVertex = v;
                    _selectEndVertex.Marked = true;
                }
                else
                {
                    _selectEndVertex.Marked = false;
                    _selectEndVertex = null;

                    _selectStartVertex.Marked = false;
                    _selectStartVertex = v;
                    _selectStartVertex.Marked = true;
                }
            }
            else
            {
                Deselect();
            }

            OnPropertyChanged("VertexItems");
        }
        public void AddEdgeFromSelected()
        {
            if (_selectStartVertex != null && _selectEndVertex != null &&
               _selectStartVertex != _selectEndVertex)
                AddEdge(_selectStartVertex, _selectEndVertex);
        }
        public void RemoveEdgeFromSelected()
        {
            if (_selectStartVertex != null && _selectEndVertex != null &&
               _selectStartVertex != _selectEndVertex)
            {
                RemoveEdge(_selectStartVertex, _selectEndVertex);
            }
        }
        public void TakePicture()
        {
            Rectangle rect = new Rectangle((int)ScreenLeft, (int)ScreenTop, 100, 100);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            try
            {
                bmp.Save("E:\\GraphDM\\image.jpeg", ImageFormat.Jpeg);
            }
            catch (ExternalException e)
            {
                System.Windows.MessageBox.Show("Invalid file path", "Error");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void InitCommands()
        {
            this.ClearCommand = new ClearCommand(this);
            this.AddEdgeCommand = new AddEdgeCommand(this);
            this.RemoveEdgeCommand = new RemoveEdgeCommand(this);
            this.LeftMouseDownCommand = new LeftMouseDownCommand(this);
            this.LeftMouseUpCommand = new LeftMouseUpCommand(this);
            this.RightMouseDownCommand = new RightMouseDownCommand(this);
            this.FindMinTreeCommand = new FindMinTreeCommand(this);
            this.GenerateGraphCommand = new GenerateGraphCommand(this);
            this.TakePictureCommand = new TakePictureCommand(this);
        }

        private bool CheckForCycle(Graph g, Edge e)
        {
            Queue<Vertex> queue = new Queue<Vertex>();
            List<Vertex> visitedVertices = new List<Vertex>();
            Vertex vertexToFind = e.V1;

            queue.Enqueue(e.V2);

            while (queue.Count > 0)
            {
                Vertex currentVertex = queue.Dequeue();
                if (vertexToFind.ID == currentVertex.ID)
                    return true;

                visitedVertices.Add(currentVertex);

                for (int i = 0; i < g.Edges.Count; i++)
                {
                    Edge currentEdge = g.Edges[i];

                    if (currentEdge.V1 == e.V1 && currentEdge.V2 == e.V2)
                        continue;

                    if (currentEdge.V1 == currentVertex &&
                        !visitedVertices.Exists(v => v.ID == currentEdge.V2.ID))
                    {
                        queue.Enqueue(currentEdge.V2);
                    }
                    else if (currentEdge.V2 == currentVertex &&
                        !visitedVertices.Exists(v => v.ID == currentEdge.V1.ID))
                    {
                        queue.Enqueue(currentEdge.V1);
                    }
                }
            }

            return false;
        }
        private Vertex CheckVertexHit(double x, double y, double r)
        {
            List<Vertex> vertices = _graph.Vertices;

            foreach (Vertex v in vertices)
                if (Distance(v.X, v.Y, x, y) <= r)
                    return v;

            return null;
        }
        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }
        private void Deselect()
        {
            if (_selectStartVertex != null)
                _selectStartVertex.Marked = false;
            if (_selectEndVertex != null)
                _selectEndVertex.Marked = false;
            _selectStartVertex = _selectEndVertex = null;
        }

        #region COMMANDS

        public ClearCommand ClearCommand { get; set; }
        public AddEdgeCommand AddEdgeCommand { get; set; }
        public RemoveEdgeCommand RemoveEdgeCommand { get; set; }
        public LeftMouseDownCommand LeftMouseDownCommand { get; set; }
        public LeftMouseUpCommand LeftMouseUpCommand { get; set; }
        public RightMouseDownCommand RightMouseDownCommand { get; set; }
        public FindMinTreeCommand FindMinTreeCommand { get; set; }
        public GenerateGraphCommand GenerateGraphCommand { get; set; }
        public TakePictureCommand TakePictureCommand { get; set; }

        #endregion

        public ObservableCollection<VertexForDisplay> VertexItems
        {
            get
            {
                var newCollection = new ObservableCollection<VertexForDisplay>();
                var oldCollection = _graph.Vertices;
                foreach (Vertex v in oldCollection)
                    newCollection.Add(new VertexForDisplay(v));
                return newCollection;
            }
        }
        public ObservableCollection<EdgeForDisplay> EdgeItems
        {
            get
            {
                var newCollection = new ObservableCollection<EdgeForDisplay>();
                var oldCollection = _graph.Edges;
                foreach (Edge e in oldCollection)
                    newCollection.Add(new EdgeForDisplay(e) { ArrowOpacity = _graph.Oriented ? 100 : 0 });
                return newCollection;
            }
        }

        public bool Lab1Radio
        {
            get
            {
                return _lab1radio;
            }
            set
            {
                if (!Lab2Radio && !Lab3Radio && !Lab4Radio && !value)
                    return;
                _lab1radio = value;
                OnPropertyChanged("Lab1Radio");
            }
        }
        public bool Lab2Radio
        {
            get
            {
                return _lab2radio;
            }
            set
            {
                if (!Lab1Radio && !Lab3Radio && !Lab4Radio && !value)
                    return;
                _lab2radio = value;
                OnPropertyChanged("Lab2Radio");
            }
        }
        public bool Lab3Radio
        {
            get
            {
                return _lab3radio;
            }
            set
            {
                if (!Lab1Radio && !Lab2Radio && !Lab4Radio && !value)
                    return;
                _lab3radio = value;
                OnPropertyChanged("Lab3Radio");
            }
        }
        public bool Lab4Radio
        {
            get
            {
                return _lab4radio;
            }
            set
            {
                if (!Lab1Radio && !Lab2Radio && !Lab3Radio && !value)
                    return;
                _lab4radio = value;
                OnPropertyChanged("Lab4Radio");
            }
        }
        public bool Lab4Check
        {
            get
            {
                return _lab4check;
            }
            set
            {
                _lab4check = value;
                OnPropertyChanged("Lab4Check");
            }
        }
        public string VertexNumber
        {
            get
            {
                return _vertexNumber;
            }
            set
            {
                if (char.IsDigit(value, value.Length - 1))
                {
                    int parsedValue = int.Parse(value);

                    if (parsedValue > 1 && parsedValue <= _gridSizeX * _gridSizeY)
                    {
                        _vertexNumber = value;

                        if(parsedValue==2 && OddNumber=="0")
                            OddNumber = "2";

                        if (int.Parse(_vertexNumber) < int.Parse(_oddNumber))
                            if (int.Parse(_vertexNumber) % 2 == 0)
                                OddNumber = (int.Parse(_vertexNumber) - 0).ToString();
                            else
                                OddNumber = (int.Parse(_vertexNumber) - 1).ToString();

                        OnPropertyChanged("VertexNumber");
                    }
                }                
            }
        }
        public string OddNumber
        {
            get
            {
                return _oddNumber;
            }
            set
            {
                if (char.IsDigit(value, value.Length - 1))
                {
                    int parsedValue = int.Parse(value);

                    if (parsedValue >= 0 && parsedValue <= int.Parse(VertexNumber) && parsedValue % 2 == 0 && 
                        (double)parsedValue/((double)_gridSizeX * (double)_gridSizeY) <= _oddPropotion &&
                        !(int.Parse(VertexNumber)==2 && parsedValue==0))
                    {
                        _oddNumber = value;
                        OnPropertyChanged("OddNumber");
                    }
                }
            }
        }
        public string TextOutput
        {
            get
            {
                return _textOutput;
            }
            set
            {
                _textOutput = value;
                OnPropertyChanged("TextOutput");
            }
        }
        public string EdgeWeight
        {
            get
            {
                return _edgeWeight;
            }
            set
            {
                _edgeWeight = value;
                OnPropertyChanged("EdgeWeight");
            }
        }
        public double ScreenLeft
        {
            get 
            { 
                return _screenLeft; 
            }
            set 
            { 
                _screenLeft = value;
                OnPropertyChanged("ScreenLeft");
            }
        }
        public double ScreenTop
        {
            get
            {
                return _screenTop;
            }
            set
            {
                _screenTop = value;
                OnPropertyChanged("ScreenTop");
            }
        }

        private double _screenLeft;
        private double _screenTop;
        private bool _lab1radio = true;
        private bool _lab2radio = false;
        private bool _lab3radio = false;
        private bool _lab4radio = false;
        private bool _lab4check = false;
        private int _gridSizeX = 5;
        private int _gridSizeY = 4;
        private double _oddPropotion = 0.8;
        private string _vertexNumber = "8";
        private string _oddNumber = "2";
        private string _textOutput = _startLine;
        private string _edgeWeight = "0";
        private Graph _graph = new Graph();
        private Vertex _leftMouseStartVertex = null;
        private Vertex _selectStartVertex = null;
        private Vertex _selectEndVertex = null;

        private const string _startLine = "\n\t\tПрограму запущено!\n\n";
        GraphGenerator _generator = null;    
    }

    public class VertexForDisplay : Vertex
    {
        public VertexForDisplay(Vertex v) 
            : base(v) { }

        public string Color
        {
            get
            {
                if (this.Marked)
                    return "LightCoral";
                else
                    return "Pink";
            }
        }       
    }

    public class EdgeForDisplay : Edge
    {
        public EdgeForDisplay(Edge e)
            : base(e) { }

        public string Color
        {
            get
            {
                if (this.Marked)
                    return "Blue";
                else
                    return "Black";
            }
        }
        public Thickness Margin
        {
            get 
            {
                return new Thickness(Math.Min(V1.X, V2.X), Math.Min(V1.Y, V2.Y)-12, 0, 0);
            }
        }
        public PointCollection ArrowPoints
        {
            get
            {
                double d = (V2.Y - V1.Y) / (V2.X - V1.X);
                double angle = Math.Atan(d);
                double x = 30 * Math.Sin(angle);
                double y = 30 * Math.Cos(angle);

                Vector A = new Vector(V1.X, V1.Y);
                Vector B = new Vector(V2.X, V2.Y);
                double h = 33, w = 5;
                Vector U = (B - A) / (B - A).Length;
                Vector V = new Vector(-U.Y, U.X);
                Vector v1 = B - h * U + w * V;
                Vector v2 = B - h * U - w * V;                

                PointCollection p = new PointCollection();
                p.Add(new System.Windows.Point(V2.X, V2.Y));
                p.Add(new System.Windows.Point(v1.X, v1.Y));
                p.Add(new System.Windows.Point(v2.X, v2.Y));

                return p;
            }
        }
        public int ArrowOpacity
        {
            get;
            set;
        }
    }
}