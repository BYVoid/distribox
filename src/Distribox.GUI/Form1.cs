using Distribox.CommonLib;
using Distribox.FileSystem;
using Distribox.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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

            this.visualTree1.NodeDoubleClick += visualTree1_NodeDoubleClick;
            this.visualTree1.NodeClick += visualTree1_NodeClick;
        }

        void visualTree1_NodeClick(FileEvent e)
        {
            string text = "";
            if (e.SHA1 != null)
            {
                text = File.ReadAllText(Config.MetaFolderData.File(e.SHA1));
            }
            this.textBox1.Text = text;
        }

        void visualTree1_NodeDoubleClick(FileEvent e)
        {
            string file = GetTempFile(e);

            this.Command("start", file);
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
                if (item.ToolTipText == current || current == null)
                {
                    this.treeView1.SelectedNode = item;

                    UpdateVisualTree();

                    break;
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
                this.visualTree1_NodeClick(this.visualTree1.Current.Event);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (this.visualTree1.CurrentSelect != null)
            {
                string sha1 = this.visualTree1.CurrentSelect.Event.SHA1;

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "All Files(*.*)|*.*";
                saveFileDialog1.Title = "Save As...";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (sha1 == null)
                    {
                        File.WriteAllText(saveFileDialog1.FileName, "");
                    }
                    else
                    {
                        File.Copy(Config.MetaFolderData.File(sha1), saveFileDialog1.FileName, true);
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            using (InviteDialog invite = new InviteDialog())
            {
                invite.ShowDialog();

                if (invite.Port != -1)
                {
                    protocol.InvitePeer(new Peer(IPAddress.Parse("127.0.0.1"), invite.Port));
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (this.visualTree1.CurrentSelect != null)
            {
                string fileA = Config.RootFolder.File(this.visualTree1.Current.Event.Name);
                string fileB = GetTempFile(this.visualTree1.CurrentSelect.Event);
                this.Command("kdiff3", string.Format("{0} {1}", fileA, fileB));
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (this.visualTree1.CurrentSelect != null)
            {
                string fileA = Config.RootFolder.File(this.visualTree1.Current.Event.Name);
                string fileB = GetTempFile(this.visualTree1.CurrentSelect.Event);

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.Arguments = string.Format("/c {0} {1}", "kdiff3", string.Format("{0} {1} -o {1}", fileA, fileB));
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();

                if (File.Exists(fileB + ".orig"))
                {
                    File.WriteAllText(fileA, File.ReadAllText(fileB));
                }
            }
        }

        private string GetTempFile(FileEvent e)
        {
            AbsolutePath tmpPath = new AbsolutePath(Path.GetTempPath() + CommonHelper.GetRandomHash());
            Directory.CreateDirectory(tmpPath);

            string name = e.Name.Substring(e.Name.IndexOf("/") + 1, e.Name.Length - e.Name.IndexOf("/") - 1);
            if (e.SHA1 == null)
            {
                File.WriteAllText(tmpPath.File(name), "");
            }
            else
            {
                File.Copy(Config.MetaFolderData.File(e.SHA1), tmpPath.File(name));
            }

            return tmpPath.File(name);
        }

        private void Command(string exe, string args, Action<object, EventArgs> callback = null)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = string.Format("/c {0} {1}", exe, args);
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            if (callback != null)
            {
                process.Exited += new EventHandler(callback);
            }
        }
    }
}
