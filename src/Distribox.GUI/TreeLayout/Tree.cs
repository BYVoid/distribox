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

            switch (this.State)
            {
                case TreeState.Normal:
                    g.DrawRectangle(Pens.Red, rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(this.Event.Type.ToString(), new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular), new SolidBrush(Color.Black), rect, format);
                    break;

                case TreeState.Selected:
                    g.FillRectangle(new SolidBrush(Color.Cyan), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(this.Event.Type.ToString(), new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular), new SolidBrush(Color.Black), rect, format);
                    break;

                case TreeState.Hover:
                    g.FillRectangle(new SolidBrush(Color.LightGreen), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(this.Event.Type.ToString(), new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular), new SolidBrush(Color.Black), rect, format);
                    break;

                case TreeState.Current:
                    g.FillRectangle(new SolidBrush(Color.Red), rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawString(this.Event.Type.ToString(), new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular), new SolidBrush(Color.White), rect, format);
                    break;

                default:
                    break;
            }

            foreach (var node in this.Children)
            {
                node.Paint(g);
                g.DrawLine(Pens.Black, this.Position.X, this.Position.Y + BOXHEIGHT / 2, node.Position.X, node.Position.Y - BOXHEIGHT / 2);
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
            this.Layout(0, 0);
        }

        private void Layout(float x, float y)
        {
            this.Position = new PointF(x, y);
            y = y + VERTICALGAP;
            foreach (var node in this.Children)
            {
                node.Layout(x, y);
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
                x = x + HORIZONTALGAP;
                this.Height = Math.Max(this.Height, node.Height + VERTICALGAP);
            }
        }
    }
}
