using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Distribox.GUI
{
    public partial class VisualTree : UserControl
    {
        public Tree CurrentHover { get; set; }
        public Tree CurrentSelect { get; set; }

        private Tree tree;

        private float dx = 80;
        private float dy = 50;
        private float scale = 1.2f;

        public VisualTree()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
        }

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
            if (tree == null) return;
            float x = e.X;
            float y = e.Y;
            x = (x - dx) / scale;
            y = (y - dy) / scale;
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
            if (tree == null) return;

            float x = e.X;
            float y = e.Y;
            x = (x - dx) / scale;
            y = (y - dy) / scale;
            Tree node = tree.Select(x, y);
            if ((node == null || node.State == TreeState.Current) && this.CurrentSelect != null)
            {
                this.CurrentSelect.State = TreeState.Normal;
                this.CurrentSelect = null;
                this.Refresh();
            }
            if (node != null && node.State != TreeState.Current && node != this.CurrentSelect)
            {
                if (this.CurrentSelect != null)
                {
                    this.CurrentSelect.State = TreeState.Normal;
                }
                this.CurrentSelect = node;
                this.CurrentSelect.State = TreeState.Selected;
                this.CurrentHover = null;
                this.Refresh();
                tree.Select(x, y);
            }
        }
    }
}
