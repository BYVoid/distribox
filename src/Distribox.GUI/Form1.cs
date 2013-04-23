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

        public Form1()
        {
            InitializeComponent();

            Start();

            UpdateTreeView();
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
            List<FileItem> list = vc.VersionList.AllFiles;

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
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string name = e.Node.ToolTipText;
            FileItem item = vc.VersionList.GetFileByName(name);
            this.visualTree1.SetTree(new Tree(item));
        }
    }
}
