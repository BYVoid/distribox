using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickGraph;

namespace Distribox.History.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CreateGraphToVisualize();

            InitializeComponent();
        }

        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get;
            private set;
        }

        private void CreateGraphToVisualize()
        {
            var graph = new BidirectionalGraph<object, IEdge<object>>();

            Random rd = new Random();
            int n = 10;

            for (int i = 0; i < n; i++)
            {
                graph.AddVertex(i);
            }

            for (int i = 1; i < n; i++)
            {
                graph.AddEdge(new Edge<object>(rd.Next((i+1)/2), i));
            }

            foreach (var vertex in graph.Vertices)
            {
                foreach (var edge in graph.InEdges(vertex))
                {
                    Console.WriteLine(edge);
                }
            }

            GraphToVisualize = graph;
        }
    }
}
