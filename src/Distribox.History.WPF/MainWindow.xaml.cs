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
using GraphSharp.Controls;
using Distribox.FileSystem;
using Distribox.CommonLib;
using System.IO;
using GraphSharp.Algorithms.Layout.Simple.Tree;

namespace Distribox.History.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            CreateGraphToVisualize();

            InitializeComponent();

            AddEventsToGraph();
        }

        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get;
            private set;
        }

        public string FileName
        {
            get;
            private set;
        }

        private void CreateGraphToVisualize()
        {
            var graph = new BidirectionalGraph<object, IEdge<object>>();

            var item = CommonHelper.ReadObject<FileItem[]>("VersionList.txt").First();

            this.FileName = string.Format("Distribox.History - {0}", item.CurrentName);

            int n = item.History.Count();

            GraphNode[] nodes = new GraphNode[n];

            Dictionary<string, GraphNode> dict = new Dictionary<string, GraphNode>();
            for (int i = 0; i < n; i++)
            {
                FileEvent e = item.History.ElementAt(i);
                nodes[i] = new GraphNode(e);
                dict[e.EventId] = nodes[i];

                graph.AddVertex(nodes[i]);

                if (e.ParentId != null)
                {
                    graph.AddEdge(new TaggedEdge<object, string>(dict[e.ParentId], nodes[i], "haha"));
                }
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

        private void AddEventsToGraph()
        {
            foreach (var v in this.graphLayout.Children)
            {
                if (v is VertexControl)
                {
                    VertexControl vc = (VertexControl)v;
                    vc.MouseEnter += MainWindow_MouseEnter;
                    vc.MouseLeave += MainWindow_MouseLeave;
                    vc.PreviewMouseDown += MainWindow_PreviewMouseDown;
                }

                if (v is EdgeControl)
                {
                    EdgeControl ec = (EdgeControl)v;
                }
            }

            SimpleTreeLayoutParameters a = new SimpleTreeLayoutParameters();
            a.LayerGap = 24;
            a.VertexGap = 20;
            this.graphLayout.LayoutParameters = a;
        }

        void MainWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            VertexControl v = (VertexControl)sender;
            GraphNode node = (GraphNode)v.Vertex;
            node.ShowToolTip();
            v.Background = new SolidColorBrush(Color.FromRgb(255, 153, 51));
        }

        void MainWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            VertexControl v = (VertexControl)sender;
            GraphNode node = (GraphNode)v.Vertex;
            node.CloseToolTip();
            v.Background = new SolidColorBrush(Color.FromRgb(51, 153, 255));
        }

        void MainWindow_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }

    public class GraphNode
    {
        private ToolTip toolTip;
        private FileEventType type;

        public GraphNode(FileEvent e)
        {
            this.toolTip = new ToolTip();
            this.toolTip.Content = string.Format("Name: {0}\nSize: {1}\nTime: {2:yyyy-MM-dd HH:mm:ss}\nEvent Id: {3}", e.Name, e.Size, e.When, e.EventId);

            this.type = e.Type;
        }

        public void ShowToolTip()
        {
            this.toolTip.IsOpen = true;
        }

        public void CloseToolTip()
        {
            this.toolTip.IsOpen = false;
        }

        public override string ToString()
        {
            return type.ToString();
        }
    }
}
