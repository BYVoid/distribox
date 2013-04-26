using Distribox.FileSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Distribox.GUI
{
    public enum TreeState
    {
        Normal, Selected, Hover, Current
    }

    public class Tree
    {
        public FileEvent Event { get; set; }
        public List<Tree> Children { get; set; }

        public TreeState State { get; set; } 

        public PointF Position { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        private const float HORIZONTALGAP = 100;
        private const float VERTICALGAP = 50;

        private const float BOXWIDTH = 80;
        private const float BOXHEIGHT = 30;

        public Tree(FileItem item)
        {
            // Event Id -> TreeNode
            Dictionary<string, Tree> dict = new Dictionary<string, Tree>();
            foreach (var e in item.History)
            {
                if (e.ParentId != null)
                {
                    dict[e.EventId] = new Tree(e);
                    dict[e.ParentId].Add(dict[e.EventId]);
                }
                else
                {
                    dict[e.EventId] = this;
                    dict[e.EventId].Event = e;
                    dict[e.EventId].Children = new List<Tree>();
                }
            }

            dict[item.CurrentEventId].State = TreeState.Current;
            this.Layout();
        }

        public Tree(FileEvent e)
        {
            this.Event = e;
            this.Children = new List<Tree>();
        }

        public Tree GetCurrent()
        {
            if (this.State == TreeState.Current)
            {
                return this;
            }
            foreach (var item in this.Children)
            {
                Tree node = item.GetCurrent();
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }

        public void Add(Tree node)
        {
            this.Children.Add(node);
        }

        public void Paint(Graphics g)
        {
            RectangleF rect = new RectangleF(Position.X - BOXWIDTH / 2, Position.Y - BOXHEIGHT / 2, BOXWIDTH, BOXHEIGHT);
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            string text = this.Event.When.ToString("HH:mm:ss");
            Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);

            switch (this.State)
            {
                case TreeState.Normal:
                    g.FillRectangle(new SolidBrush(Color.FromArgb(51, 153, 255)), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(text, font, new SolidBrush(Color.White), rect, format);
                    break;

                case TreeState.Selected:
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 51, 214)), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(text, font, new SolidBrush(Color.White), rect, format);
                    break;

                case TreeState.Hover:
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 153, 51)), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(text, font, new SolidBrush(Color.White), rect, format);
                    break;

                case TreeState.Current:
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 15, 0)), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(text, font, new SolidBrush(Color.White), rect, format);
                    break;

                default:
                    break;
            }

            foreach (var node in this.Children)
            {
                node.Paint(g);
                PointF start = new PointF(this.Position.X, this.Position.Y + BOXHEIGHT / 2);
                PointF stop = new PointF(node.Position.X, node.Position.Y - BOXHEIGHT / 2);
                PointF p1 = new PointF(start.X, start.Y + (VERTICALGAP - BOXHEIGHT) * 0.382f);
                PointF p2 = new PointF(stop.X, start.Y + (VERTICALGAP - BOXHEIGHT) * 0.382f);
                g.DrawLine(Pens.Gray, start, p1);
                g.DrawLine(Pens.Gray, p1, p2);
                g.DrawLine(Pens.Gray, p2, stop);
            }
        }

        public Tree Select(float x, float y)
        {
            if (Math.Abs(x - this.Position.X) < BOXWIDTH / 2 && Math.Abs(y - this.Position.Y) < BOXHEIGHT / 2)
            {
                return this;
            }
            foreach (var node in this.Children)
            {
                Tree select = node.Select(x, y);
                if (select != null)
                {
                    return select;
                }
            }
            return null;
        }

        public void Layout()
        {
            this.CalculateSize();
            Tree[] list = this.GetTreeList().OrderBy(x => x.Event.When).ToArray();
            Dictionary<Tree, float> verticalPosition = new Dictionary<Tree, float>();
            for (int i = 0; i < list.Length; i++)
            {
                verticalPosition[list[i]] = i * VERTICALGAP;
            }
            this.Layout(0, verticalPosition);
        }

        private void Layout(float x, Dictionary<Tree, float> verticalPosition)
        {
            this.Position = new PointF(x, verticalPosition[this]);
            foreach (var node in this.Children)
            {
                node.Layout(x, verticalPosition);
                x = x + node.Width + HORIZONTALGAP;
            }
        }

        private void CalculateSize()
        {
            float x = 0;
            this.Width = 0;
            this.Height = 0;
            foreach (var node in this.Children)
            {
                node.CalculateSize();
                this.Width = Math.Max(this.Width, x + node.Width);
                x = x + node.Width + HORIZONTALGAP;
                this.Height = Math.Max(this.Height, node.Height + VERTICALGAP);
            }
        }

        public List<Tree> GetTreeList()
        {
            List<Tree> tree = new List<Tree>();
            tree.Add(this);
            foreach (var node in this.Children.SelectMany(x => x.GetTreeList()))
                tree.Add(node);
            return tree;
        }
    }
}
