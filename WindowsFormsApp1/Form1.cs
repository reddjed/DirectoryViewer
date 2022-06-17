using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        ImageList imageList = new ImageList();

        UndoRedoStack<string> filePath = new UndoRedoStack<string>();

        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(Environment.GetLogicalDrives());
            comboBox1.SelectedIndex = 0;

            System.Drawing.Image image = Image.FromFile(@"ico\fold.jpg");

            imageList.Images.Add("fold", image);

            listView1.LargeImageList = imageList;

            button1.Enabled = false;
            button2.Enabled = false;

            //TreeNode root = new TreeNode(comboBox1.SelectedItem.ToString(), 0, 0);
            //root.Nodes.Add(new TreeNode());
            //treeView1.Nodes.Add(root);

            treeView1.BeforeExpand += TreeView1_BeforeExpand;
            treeView1.AfterExpand += TreeView1_AfterExpand;
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;

            //FileSystemWatcher watcher = new FileSystemWatcher();
            //watcher.Created += Watcher_Created;

            //watcher.Path = @"C:\Users\us\source\repos\TreeView";
            //watcher.EnableRaisingEvents = true;
        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Process.Start(e.Node.FullPath);
            //throw new NotImplementedException();
        }

        //private void Watcher_Created(object sender, FileSystemEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private void TreeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {

        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

            e.Node.Nodes.Clear();
            //string[] files = Directory.GetFiles(e.Node.FullPath);
            try
            {
                string[] dirs = Directory.GetDirectories(e.Node.FullPath);
                label1.Text = e.Node.FullPath;
                foreach (var d in dirs)
                {
                    DirectoryInfo di = new DirectoryInfo(d);
                    TreeNode node = new TreeNode(di.Name, 2, 2);

                    node.ImageKey = "folder";
                    node.Tag = di;
                    node.Nodes.Add(new TreeNode());

                    e.Node.Nodes.Add(node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
                return;
            }
            //foreach (var f in files)
            //{
            //    FileInfo fi = new FileInfo(f);
            //    TreeNode node = new TreeNode(fi.Name, 2, 2);

            //    e.Node.Nodes.Add(node);
            //}

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            imageList.Images.RemoveByKey(".exe");
            listView1.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            filePath.Clear();
            button1.Enabled = false;
            button2.Enabled = false;

            treeView1.Nodes.Clear();

            TreeNode root = new TreeNode(comboBox1.SelectedItem.ToString(), 0, 0);
            root.Nodes.Add(new TreeNode());
            treeView1.Nodes.Add(root);

            label1.Text = comboBox1.SelectedItem.ToString();
            string[] dirs = { };
            string[] files = { };
            try
            {
                dirs = Directory.GetDirectories(comboBox1.SelectedItem.ToString());
                files = Directory.GetFiles(comboBox1.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source);
            }

            filePath.Push(comboBox1.SelectedItem.ToString());

            foreach (var f in files)
            {
                FileInfo fi = new FileInfo(f);

                if (!imageList.Images.ContainsKey(fi.Extension))
                {
                    Icon iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName);
                    imageList.Images.Add(fi.Extension, iconForFile);
                }

                item = new ListViewItem(fi.Name);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, fi.FullName),
                    new ListViewItem.ListViewSubItem(item, fi.LastAccessTime.ToShortDateString())
                };

                item.ImageKey = fi.Extension;

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            foreach (var d in dirs)
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                item = new ListViewItem(dir.Name);

                item.ImageIndex = 0;
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, dir.FullName),
                    new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())
                };

                item.ImageKey = "fold";
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            imageList.Images.RemoveByKey(".exe");

            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;

            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            label1.Text = e.Node.FullPath;

            if (newSelected.Level == 0)
            {
                string[] dirs = Directory.GetDirectories(e.Node.FullPath);
                string[] files = Directory.GetFiles(e.Node.FullPath);

                foreach (var f in files)
                {
                    FileInfo fi = new FileInfo(f);

                    if (!imageList.Images.ContainsKey(fi.Extension))
                    {
                        Icon iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName);
                        imageList.Images.Add(fi.Extension, iconForFile);
                    }

                    item = new ListViewItem(fi.Name);
                    subItems = new ListViewItem.ListViewSubItem[]
                    {
                        new ListViewItem.ListViewSubItem(item, fi.FullName),
                        new ListViewItem.ListViewSubItem(item, fi.LastAccessTime.ToShortDateString())
                    };

                    item.ImageKey = fi.Extension;

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }

                foreach (var d in dirs)
                {
                    DirectoryInfo dir = new DirectoryInfo(d);
                    item = new ListViewItem(dir.Name);
                    item.ImageIndex = imageList.Images.Count - 1;
                    subItems = new ListViewItem.ListViewSubItem[]
                    {
                        new ListViewItem.ListViewSubItem(item, dir.FullName),
                        new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())
                    };

                    item.ImageKey = "fold";

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }
            }
            else
            {
                try
                {
                    foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
                    {

                        item = new ListViewItem(dir.Name);
                        item.ImageIndex = imageList.Images.Count - 1;
                        subItems = new ListViewItem.ListViewSubItem[]
                        {
                            new ListViewItem.ListViewSubItem(item, dir.FullName),
                            new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())
                        };

                        item.ImageKey = "fold";

                        item.SubItems.AddRange(subItems);
                        listView1.Items.Add(item);
                    }
                    foreach (FileInfo file in nodeDirInfo.GetFiles())
                    {

                        if (!imageList.Images.ContainsKey(file.Extension))
                        {
                            Icon iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(file.FullName);
                            imageList.Images.Add(file.Extension, iconForFile);
                        }

                        item = new ListViewItem(file.Name, 1);
                        subItems = new ListViewItem.ListViewSubItem[]
                        {
                            new ListViewItem.ListViewSubItem(item, file.FullName),
                            new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString())
                        };

                        item.ImageKey = file.Extension;

                        item.SubItems.AddRange(subItems);
                        listView1.Items.Add(item);
                    }

                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source);
                    return;
                }

            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            FileInfo fileInfo = new FileInfo(listView1.Items[listView1.Items.IndexOf(listView1.FocusedItem)].SubItems[1].Text);

            if (fileInfo.Exists == true)
            {
                Process.Start(listView1.Items[listView1.Items.IndexOf(listView1.FocusedItem)].SubItems[1].Text);
            }

            else
            {
                imageList.Images.RemoveByKey(".exe");

                string[] dirs = { };
                string[] files = { };

                try
                {
                    dirs = Directory.GetDirectories(listView1.Items[listView1.Items.IndexOf(listView1.FocusedItem)].SubItems[1].Text);
                    files = Directory.GetFiles(listView1.Items[listView1.Items.IndexOf(listView1.FocusedItem)].SubItems[1].Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source);
                    return;
                }
                filePath.Push(listView1.Items[listView1.Items.IndexOf(listView1.FocusedItem)].SubItems[1].Text);

                button1.Enabled = true;

                label1.Text = listView1.Items[listView1.Items.IndexOf(listView1.FocusedItem)].SubItems[1].Text;

                listView1.Items.Clear();
                ListViewItem.ListViewSubItem[] subItems;
                ListViewItem item = null;

                foreach (var f in files)
                {
                    FileInfo fi = new FileInfo(f);

                    if (!imageList.Images.ContainsKey(fi.Extension))
                    {
                        Icon iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName);
                        imageList.Images.Add(fi.Extension, iconForFile);
                    }

                    item = new ListViewItem(fi.Name);

                    subItems = new ListViewItem.ListViewSubItem[]
                    {
                    new ListViewItem.ListViewSubItem(item, fi.FullName),
                    new ListViewItem.ListViewSubItem(item, fi.LastAccessTime.ToShortDateString())
                    };

                    item.ImageKey = fi.Extension;

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }

                foreach (var d in dirs)
                {
                    DirectoryInfo dir = new DirectoryInfo(d);
                    item = new ListViewItem(dir.Name);
                    item.ImageIndex = imageList.Images.Count - 1;
                    subItems = new ListViewItem.ListViewSubItem[]
                    {
                    new ListViewItem.ListViewSubItem(item, dir.FullName),
                    new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())
                    };

                    item.ImageKey = "fold";

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            imageList.Images.RemoveByKey(".exe");

            filePath.Undo();

            if (!filePath.UndoEnable)
            {
                button1.Enabled = false;
            }

            button2.Enabled = true;

            string[] dirs = { };
            string[] files = { };

            dirs = Directory.GetDirectories(filePath.CurrentItem);
            files = Directory.GetFiles(filePath.CurrentItem);

            label1.Text = filePath.CurrentItem;

            listView1.Items.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (var f in files)
            {
                FileInfo fi = new FileInfo(f);

                if (!imageList.Images.ContainsKey(fi.Extension))
                {
                    Icon iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName);
                    imageList.Images.Add(fi.Extension, iconForFile);
                }

                item = new ListViewItem(fi.Name);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, fi.FullName),
                    new ListViewItem.ListViewSubItem(item, fi.LastAccessTime.ToShortDateString())
                };

                item.ImageKey = fi.Extension;

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            foreach (var d in dirs)
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                item = new ListViewItem(dir.Name);
                item.ImageIndex = imageList.Images.Count - 1;
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, dir.FullName),
                    new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())
                };

                item.ImageKey = "fold";

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imageList.Images.RemoveByKey(".exe");

            filePath.Redo();

            if (!filePath.RedoEnable)
            {
                button2.Enabled = false;
            }

            button1.Enabled = true;

            string[] dirs = { };
            string[] files = { };

            dirs = Directory.GetDirectories(filePath.CurrentItem);
            files = Directory.GetFiles(filePath.CurrentItem);

            label1.Text = filePath.CurrentItem;

            listView1.Items.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (var f in files)
            {
                FileInfo fi = new FileInfo(f);

                if (!imageList.Images.ContainsKey(fi.Extension))
                {
                    Icon iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName);
                    imageList.Images.Add(fi.Extension, iconForFile);
                }

                item = new ListViewItem(fi.Name);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, fi.FullName),
                    new ListViewItem.ListViewSubItem(item, fi.LastAccessTime.ToShortDateString())
                };

                item.ImageKey = fi.Extension;

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            foreach (var d in dirs)
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                item = new ListViewItem(dir.Name);
                item.ImageIndex = imageList.Images.Count - 1;
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, dir.FullName),
                    new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())
                };

                item.ImageKey = "fold";

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
        }

        public class UndoRedoStack<T>
        {
            private Stack<T> undoStack = new Stack<T>();
            private Stack<T> redoStack = new Stack<T>();

            public T CurrentItem { get; private set; }

            public bool UndoEnable
            {
                get { return undoStack.Count > 0; }
            }

            public bool RedoEnable
            {
                get { return redoStack.Count > 0; }
            }

            public void Push(T item)
            {
                if (!CurrentItemIsEmpty)
                    undoStack.Push(CurrentItem);

                CurrentItem = item;
                redoStack.Clear();
            }

            public void Undo()
            {
                if (undoStack.Count == 0)
                    return;

                if (!CurrentItemIsEmpty)
                    redoStack.Push(CurrentItem);

                CurrentItem = undoStack.Pop();
            }

            public void Redo()
            {
                if (redoStack.Count == 0)
                {
                    return;
                }


                if (!CurrentItemIsEmpty)
                    undoStack.Push(CurrentItem);

                CurrentItem = redoStack.Pop();
            }

            public void Clear()
            {
                CurrentItem = default(T);
                undoStack.Clear();
                redoStack.Clear();
            }

            private bool CurrentItemIsEmpty
            {
                get { return object.Equals(CurrentItem, default(T)); }
            }
        }
    }
}
