using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Distribox.FileSystem;

namespace Distribox.GUI
{
    public partial class VisualTree : UserControl
    {
        public Tree CurrentHover { get; set; }
        public Tree CurrentSelect { get; set; }
        public Tree Current
        {
            get
            {
                return tree.GetCurrent();
            }
        }

        private Tree tree;

        private float dx = 80;
        private float dy = 50;
        private float scale = 1.2f;

        private float lastX = -1;
        private float lastY = -1;

        public VisualTree()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        public delegate void NodeDoubleClickHandler(FileEvent e);
        public event NodeDoubleClickHandler NodeDoubleClick;

        public delegate void NodeClickHandler(FileEvent e);
        public event NodeClickHandler NodeClick;

        public void SetTree(Tree tree)
        {
            this.tree = tree;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (tree != null)
            {
                e.Graphics.TranslateTransform(dx, dy);
                e.Graphics.ScaleTransform(scale, scale);
                tree.Paint(e.Graphics);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            float x = e.X;
            float y = e.Y;
            x = (x - dx) / scale;
            y = (y - dy) / scale;

            if (e.Button == MouseButtons.Left)
            {
                if (lastX > 0)
                {
                    this.dx += (e.X - lastX) * 1;
                    this.dy += (e.Y - lastY) * 1;
                }
                lastX = e.X;
                lastY = e.Y;
                this.Refresh();
            }
            else
            {
                lastX = -1;
                lastY = -1;
            }

            if (tree == null) return;

            Tree node = tree.Select(x, y);
            if ((node == null || node.State == TreeState.Current) && this.CurrentHover != null)
            {
                this.CurrentHover.State = TreeState.Normal;
                this.CurrentHover = null;
                this.Refresh();
            }
            if (node != null && node.State != TreeState.Current && node != this.CurrentHover && node != this.CurrentSelect)
            {
                if (this.CurrentHover != null)
                {
                    this.CurrentHover.State = TreeState.Normal;
                }
                this.CurrentHover = node;
                this.CurrentHover.State = TreeState.Hover;
                this.Refresh();
                tree.Select(x, y);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();

            float x = e.X;
            float y = e.Y;
            x = (x - dx) / scale;
            y = (y - dy) / scale;

            this.lastX = e.X;
            this.lastY = e.Y;

            if (tree == null) return;

            Tree node = tree.Select(x, y);
            if ((node == null || node.State == TreeState.Current) && this.CurrentSelect != null)
            {
                this.CurrentSelect.State = TreeState.Normal;
                this.CurrentSelect = null;
                this.Refresh();
            }
            if (node != null && node != this.CurrentSelect)
            {
                if (node.State != TreeState.Current)
                {
                    if (this.CurrentSelect != null)
                    {
                        this.CurrentSelect.State = TreeState.Normal;
                    }
                    this.CurrentSelect = node;
                    this.CurrentSelect.State = TreeState.Selected;
                    this.CurrentHover = null;
                    this.Refresh();
                    if (this.NodeClick != null)
                    {
                        this.NodeClick.Invoke(node.Event);
                    }
                }
                else
                {
                    if (this.NodeClick != null)
                    {
                        this.NodeClick.Invoke(node.Event);
                    }
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (tree == null) return;

            float x = e.X;
            float y = e.Y;
            x = (x - dx) / scale;
            y = (y - dy) / scale;
            Tree node = tree.Select(x, y);

            if (node != null && this.NodeDoubleClick != null)
            {
                this.NodeDoubleClick.Invoke(node.Event);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float delta = e.Delta;
            delta = delta / 500;
            this.scale *= 1 + delta;
            this.dx += delta * (this.dx - e.X);
            this.dy += delta * (this.dy - e.Y);
            this.Refresh();
        }
    }
}
