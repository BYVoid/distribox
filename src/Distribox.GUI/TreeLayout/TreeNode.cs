using Distribox.FileSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Distribox.GUI
{
    class TreeNode
    {
        public FileEvent Event { get; set; }
        public List<TreeNode> Children { get; set; }
        public PointF Position { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public TreeNode(FileEvent e)
        {
            this.Event = e;
            this.Children = new List<TreeNode>();
        }

        public void Add(TreeNode node)
        {
            this.Children.Add(node);
        }

        public void Layout()
        {

        }

        public void CalculateSize(float x)
        {

        }
    }
}
