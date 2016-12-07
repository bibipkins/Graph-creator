using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphViewModel
{
    public class GenerateGraphCommand : ICommand
    {
        public GenerateGraphCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            GraphModel.GenerateGraph();
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class ClearCommand : ICommand
    {
        public ClearCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            GraphModel.ClearGraph();
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class AddEdgeCommand : ICommand
    {
        public AddEdgeCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            GraphModel.AddEdgeFromSelected();
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class RemoveEdgeCommand : ICommand
    {
        public RemoveEdgeCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            GraphModel.RemoveEdgeFromSelected();
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class FindMinTreeCommand : ICommand
    {
        public FindMinTreeCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            GraphModel.FindMinTree();
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class LeftMouseDownCommand : ICommand
    {
        public LeftMouseDownCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            MouseButtonEventArgs e = parameter as MouseButtonEventArgs;
            Canvas c = e.Device.Target as Canvas;

            if (c != null)
            {
                Point p = e.GetPosition(c);
                GraphModel.SelectVertices(p.X, p.Y);
                GraphModel.AddVertex(p.X, p.Y);
                GraphModel.LeftMouseDragStart(p.X, p.Y);
            }
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class LeftMouseUpCommand : ICommand
    {
        public LeftMouseUpCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            MouseButtonEventArgs e = parameter as MouseButtonEventArgs;
            Canvas c = e.Device.Target as Canvas;

            if (c != null)
            {
                Point p = e.GetPosition(c);
                GraphModel.LeftMouseDragEnd(p.X, p.Y);
            }
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class RightMouseDownCommand : ICommand
    {
        public RightMouseDownCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            MouseButtonEventArgs e = parameter as MouseButtonEventArgs;
            Canvas c = e.Device.Target as Canvas;

            if (c != null)
            {
                Point p = e.GetPosition(c);
                GraphModel.RemoveVertex(p.X, p.Y);
            }
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }

    public class TakePictureCommand : ICommand
    {
        public TakePictureCommand(GraphViewModel graphviewModel)
        {
            this.GraphModel = graphviewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            GraphModel.TakePicture();
        }

        public GraphViewModel GraphModel { get; set; }
        public event EventHandler CanExecuteChanged;
    }
}