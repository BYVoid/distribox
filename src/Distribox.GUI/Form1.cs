using Distribox.CommonLib;
using Distribox.FileSystem;
using Distribox.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Distribox.GUI
{
    public partial class Form1 : Form
    {
        private AntiEntropyProtocol protocol;
        private VersionControl vc;

        private string lastVersionList = null;

        public Form1()
        {
            InitializeComponent();

            Start();

            UpdateTreeView();

            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Start();
            timer.Tick += timer_Tick;
        }

        private void Start()
        {
            // Get config
            int port = Config.ListenPort;

            // Initialize folder
            CommonHelper.InitializeFolder();

            // Start peer service
            StartPeer(port);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            string current = vc.VersionList.Serialize();
            if (this.lastVersionList != current)
            {
                this.lastVersionList = current;
                this.UpdateTreeView();
            }
        }

        private void StartPeer(int port)
        {
            string peerListName = Config.PeerListFilePath;

            // Initialize anti entropy protocol
            vc = new VersionControl();
            protocol = new AntiEntropyProtocol(port, peerListName, vc);

            // Initialize file watcher
            FileWatcher watcher = new FileWatcher();
            watcher.Created += vc.Created;
            watcher.Changed += vc.Changed;
            watcher.Deleted += vc.Deleted;
            watcher.Renamed += vc.Renamed;
            watcher.Idle += vc.Flush;
        }

        private void UpdateTreeView()
        {
            this.treeView1.BeginUpdate();

            List<FileItem> list = vc.VersionList.AllFiles;

            string current = null;
            if (this.treeView1.SelectedNode != null)
            {
                current = this.treeView1.SelectedNode.ToolTipText;
            }

            this.treeView1.Nodes.Clear();


            Dictionary<string, TreeNode> dict = new Dictionary<string, TreeNode>();

            foreach (var item in list)
            {
                dict[item.CurrentName] = new TreeNode();
            }

            foreach (var item in list)
            {
                int k = item.CurrentName.LastIndexOf("/");
                if (k != -1)
                {
                    string parent = item.CurrentName.Substring(0, k);
                    dict[item.CurrentName].Text = item.CurrentName.Substring(k + 1, item.CurrentName.Length - k - 1);
                    dict[item.CurrentName].ToolTipText = item.CurrentName;
                    dict[parent].Nodes.Add(dict[item.CurrentName]);
                }
                else
                {
                    dict[item.CurrentName].Text = item.CurrentName;
                    dict[item.CurrentName].ToolTipText = item.CurrentName;
                    this.treeView1.Nodes.Add(dict[item.CurrentName]);
                }
            }

            foreach (var item in dict.Values)
            {
                if (item.ToolTipText == current)
                {
                    this.treeView1.SelectedNode = item;

                    UpdateVisualTree();
                }
            }

            this.treeView1.EndUpdate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.visualTree1.CurrentSelect == null)
            {
                return;
            }

            string fileId = this.visualTree1.CurrentSelect.Event.FileId;
            string eventId = this.visualTree1.CurrentSelect.Event.EventId;

            vc.CheckOut(fileId, eventId);

            UpdateVisualTree();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string name = e.Node.ToolTipText;
            FileItem item = vc.VersionList.GetFileByName(name);
            this.visualTree1.SetTree(new Tree(item));
            this.tabControl2.TabPages[0].Text = name;
        }

        private void UpdateVisualTree()
        {
            if (this.treeView1.SelectedNode != null)
            {
                string name = this.treeView1.SelectedNode.ToolTipText;
                FileItem item = vc.VersionList.GetFileByName(name);
                this.visualTree1.SetTree(new Tree(item));
                this.tabControl2.TabPages[0].Text = name;
            }
        }
    }
}
