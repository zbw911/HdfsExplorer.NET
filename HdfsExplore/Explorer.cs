using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Hadoop.WebHDFS;

namespace HdfsExplore
{
    // Author: Paul Li
    // Create Date: 8/1/2002

    public partial class Explorer : Form
    {
        private WebHDFSClient client;

        public Explorer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();


            if (!CreateClient())
            {
                return;
            }

            // Populate TreeView with Drive list
            PopulateDriveList();
        }

        //This procedure populate the TreeView with the Drive list
        private async void PopulateDriveList()
        {
            TreeNode nodeTreeNode;
            var imageIndex = 0;
            var selectIndex = 0;

            const int Removable = 2;
            const int LocalDisk = 3;
            const int Network = 4;
            const int CD = 5;
            //const int RAMDrive = 6;

            Cursor = Cursors.WaitCursor;
            //clear TreeView
            tvFolders.Nodes.Clear();
            nodeTreeNode = new TreeNode("Hadoop Root", 0, 0);
            tvFolders.Nodes.Add(nodeTreeNode);

            //set node collection
            var nodeCollection = nodeTreeNode.Nodes;


            var x = await client.GetDirectoryStatus("/");


            foreach (var mo in x.Directories)
            {
                //switch (int.Parse(mo["DriveType"].ToString()))
                //{
                //    case Removable:			//removable drives
                //        imageIndex = 5;
                //        selectIndex = 5;
                //        break;
                //    case LocalDisk:			//Local drives
                //        imageIndex = 6;
                //        selectIndex = 6;
                //        break;
                //    case CD:				//CD rom drives
                //        imageIndex = 7;
                //        selectIndex = 7;
                //        break;
                //    case Network:			//Network drives
                //        imageIndex = 8;
                //        selectIndex = 8;
                //        break;
                //    default:				//defalut to folder
                //        imageIndex = 2;
                //        selectIndex = 3;
                //        break;
                //}

                imageIndex = 6;
                selectIndex = 6;

                //create new drive node
                nodeTreeNode = new TreeNode(mo.PathSuffix, imageIndex, selectIndex);

                //add new node
                nodeCollection.Add(nodeTreeNode);
            }


            //Init files ListView
            InitListView();

            Cursor = Cursors.Default;
        }

        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Populate folders and files when a folder is selected
            Cursor = Cursors.WaitCursor;

            //get current selected drive or folder
            var nodeCurrent = e.Node;

            //clear all sub-folders
            nodeCurrent.Nodes.Clear();

            if (nodeCurrent.SelectedImageIndex == 0)
            {
                //Selected My Computer - repopulate drive list
                PopulateDriveList();
            }
            else
            {
                //populate sub-folders and folder files
                PopulateDirectory(nodeCurrent, nodeCurrent.Nodes);
            }
            Cursor = Cursors.Default;
        }

        protected void InitListView()
        {
            //init ListView control
            lvFiles.Clear(); //clear control
            //create column header for ListView
            lvFiles.Columns.Add("Name", 150, HorizontalAlignment.Left);
            lvFiles.Columns.Add("Size", 75, HorizontalAlignment.Right);
            lvFiles.Columns.Add("Access", 140, HorizontalAlignment.Left);
            lvFiles.Columns.Add("Modified", 140, HorizontalAlignment.Left);


            lvFiles.Columns.Add("Owner", 75);
            lvFiles.Columns.Add("Permission", 75);


        }

        protected async void PopulateDirectory(TreeNode nodeCurrent, TreeNodeCollection nodeCurrentCollection)
        {
            TreeNode nodeDir;
            var imageIndex = 2; //unselected image index
            var selectIndex = 3; //selected image index

            if (nodeCurrent.SelectedImageIndex != 0)
            {
                //populate treeview with folders
                try
                {
                    var directoryStatus = await client.GetDirectoryStatus(getFullPath(nodeCurrent.FullPath));


                    var dirs = directoryStatus.Directories;

                    //check path
                    //if (!dirs.Any())
                    //{
                    //    MessageBox.Show("Directory or path " + nodeCurrent.ToString() + " does not exist.");
                    //}
                    //else
                    //{
                    //populate files
                    PopulateFiles(nodeCurrent);

                    var stringDirectories = dirs.Select(x => x.PathSuffix).ToArray();
                    var stringFullPath = "";
                    var stringPathName = "";

                    //loop throught all directories
                    foreach (var stringDir in stringDirectories)
                    {
                        stringFullPath = stringDir;
                        stringPathName = GetPathName(stringFullPath);

                        //create node for directories
                        nodeDir = new TreeNode(stringPathName, imageIndex, selectIndex);
                        nodeCurrentCollection.Add(nodeDir);
                    }
                    //}
                }
                catch (IOException e)
                {
                    MessageBox.Show("Error: Drive not ready or directory does not exist.");
                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show("Error: Drive or directory access denided.");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e);
                }
            }
        }

        protected string GetPathName(string stringPath)
        {
            //Get Name of folder
            var stringSplit = stringPath.Split('\\');
            var _maxIndex = stringSplit.Length;
            return stringSplit[_maxIndex - 1];
        }

        protected async void PopulateFiles(TreeNode nodeCurrent)
        {
            //Populate listview with files
            var lvData = new string[6];

            //clear list
            InitListView();

            if (nodeCurrent.SelectedImageIndex != 0)
            {
                var directoryStatus = await client.GetDirectoryStatus(getFullPath(nodeCurrent.FullPath));


                try
                {
                    var files = directoryStatus.Files; // Directory.GetFiles(getFullPath(nodeCurrent.FullPath));
                    var stringFileName = "";
                    string dtCreateDate, dtModifyDate;
                    Int64 lFileSize = 0;

                    //loop throught all files
                    foreach (var file in files)
                    {
                        stringFileName = file.PathSuffix;
                        //FileInfo objFileSize = new FileInfo(stringFileName);
                        lFileSize = file.Length;
                        dtCreateDate = Utility.JavaTicksToDatetime(file.AccessTime).ToString();
                        //GetCreationTime(stringFileName);
                        dtModifyDate = Utility.JavaTicksToDatetime(file.ModificationTime).ToString();
                        //GetLastWriteTime(stringFileName);

                        //create listview data
                        lvData[0] = GetPathName(stringFileName);
                        lvData[1] = formatSize(lFileSize);


                        //is in day light saving time adjust time
                        lvData[2] = dtCreateDate;

                        //not in day light saving time adjust time
                        lvData[3] = dtModifyDate;

                        lvData[4] = file.Owner;
                        lvData[5] = file.Permission;

                        //Create actual list item
                        var lvItem = new ListViewItem(lvData, 0);
                        lvFiles.Items.Add(lvItem);
                    }
                }
                catch (IOException e)
                {
                    MessageBox.Show("Error: Drive not ready or directory does not exist.");
                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show("Error: Drive or directory access denided.");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e);
                }
                //}
            }
        }

        protected string getFullPath(string stringPath)
        {
            //Get Full path
            var stringParse = "";
            //remove My Computer from path.
            stringParse = stringPath.Replace("Hadoop Root\\", "/").Replace("\\", "/");

            return stringParse;
        }

        protected string getFullPath(TreeNode nodeCurrent)
        {
            var path = nodeCurrent.FullPath;

            return getFullPath(path);
        }

        protected string formatSize(Int64 lSize)
        {
            //Format number to KB
            var stringSize = "";
            var myNfi = new NumberFormatInfo();

            Int64 lKBSize = 0;

            if (lSize < 1024)
            {
                if (lSize == 0)
                {
                    //zero byte
                    stringSize = "0";
                }
                else
                {
                    //less than 1K but not zero byte
                    stringSize = "1";
                }
            }
            else
            {
                //convert to KB
                lKBSize = lSize / 1024;
                //format number with default format
                stringSize = lKBSize.ToString("n", myNfi);
                //remove decimal
                stringSize = stringSize.Replace(".00", "");
            }

            return stringSize + " KB";
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            //quit application
            Close();
        }

        private void tvFolders_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (e.Node == null) return;

            //MessageBox.Show(e.Node.FullPath);

            tvFolders.SelectedNode = e.Node;

            InitcmsOneRout();

            cmsOneRout.Show(tvFolders, e.X, e.Y);
        }

        private void InitcmsOneRout()
        {
            cmsOneRout = new ContextMenuStrip();

            var tmiEditRoutStation = new ToolStripMenuItem("修改Owner");
            cmsOneRout.Items.Add(tmiEditRoutStation);

            var changePermission = new ToolStripMenuItem("修改权限");

            changePermission.Click += changePress_Click;
            cmsOneRout.Items.Add(changePermission);
            var tmiDel = new ToolStripMenuItem("删除");

            tmiDel.Click += tmiDel_Click;
            cmsOneRout.Items.Add(tmiDel);

        }

        async void changePress_Click(object sender, EventArgs e)
        {
            //if (lvFiles.SelectedItems.Count == 0)
            //    return;

            if (tvFolders.SelectedNode == null)
                return;

            var remotepath = getFullPath(tvFolders.SelectedNode);
            //var remotefile = remotepath.TrimEnd('/') + "/" + lvFiles.SelectedItems[0].Text;
            var permissions = await client.GetFileStatus(remotepath);

            InputName ip = new InputName("修改权限(" + remotepath + ")", permissions.Permission);

            if (ip.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(ip.StrInput))
                {
                    MessageBox.Show("不能为空项");
                    return;
                }

                await client.SetPermissions(remotepath, ip.StrInput);
            }
        }




        private void tmiDel_Click(object sender, EventArgs e)
        {
            var path = getFullPath(tvFolders.SelectedNode);

            //client = new WebHDFSClient(new Uri("http://zhangbaowei:50070"), "hadoop");

            client.DeleteDirectory(path, true);

            MessageBox.Show("删除完成");

            tvFolders.SelectedNode.Remove();
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            var parentPath = "/";

            if (tvFolders.SelectedNode != null)
            {
                parentPath = getFullPath(tvFolders.SelectedNode);
            }

            var name = new InputName();

            if (name.ShowDialog() != DialogResult.OK)
                return;


            var newname = name.StrInput;


            client.CreateDirectory(parentPath.TrimEnd('/') + "/" + newname);
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            showSetting();
            PopulateDriveList();
        }

        private void showSetting()
        {
            var setting = new Setting();

            if (setting.ShowDialog() == DialogResult.OK)
            {
                CreateClient();
            }
        }

        private bool CreateClient()
        {
            var url = AppConfig.GetValue("hdfsurl");

            var username = AppConfig.GetValue("username");


            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("未进行正确配置");
                showSetting();

                return false;
            }

            client = new WebHDFSClient(new Uri(url), username);

            return true;
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            if (tvFolders.SelectedNode == null)
            {
                MessageBox.Show("未选中上传目标");
                return;
            }

            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;

            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                var remotepath = getFullPath(tvFolders.SelectedNode);


                var filename = of.FileNames;

                foreach (var s in filename)
                {
                    client.CreateFile(s, remotepath.TrimEnd('/') + "/" + Path.GetFileName(s));
                }

                PopulateFiles(tvFolders.SelectedNode);

            }
        }

        private void menuItem8_Click(object sender, EventArgs e)
        {
            var items = lvFiles.SelectedItems;
            var remotepath = getFullPath(tvFolders.SelectedNode);
            foreach (var item in items)
            {


                var filename = ((ListViewItem)item).Text;

                client.DeleteDirectory(remotepath.TrimEnd('/') + "/" + filename);
            }
        }

        private async void lvFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lvFiles.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                //MessageBox.Show("The selected Item Name is: " + item.Text);


                var remotepath = getFullPath(tvFolders.SelectedNode);
                var remotefile = remotepath.TrimEnd('/') + "/" + item.Text;
                var message = await client.OpenFile(remotefile);

                var content = await message.Content.ReadAsStringAsync();


                ContentViewer cv = new ContentViewer(content, remotefile);
                cv.Show();

            }
            else
            {
                this.lvFiles.SelectedItems.Clear();
                MessageBox.Show("No Item is selected");
            }
        }

        private void lvFiles_MouseClick(object sender, MouseEventArgs e)
        {
            //判断鼠标是否右键
            if (e.Button == MouseButtons.Right)
            {
                if (this.lvFiles.SelectedItems.Count > 0)  //选中项
                {

                    var cmslv = new ContextMenuStrip();

                    var tmiEditRoutStation = new ToolStripMenuItem("权限");
                    tmiEditRoutStation.Click += tmiEditRoutStation_Click;
                    cmslv.Items.Add(tmiEditRoutStation);

                    var tmiDelte = new ToolStripMenuItem("删除");
                    tmiDelte.Click += tmiDelte_Click;
                    cmslv.Items.Add(tmiDelte);


                    var downloadFile = new ToolStripMenuItem("下载");
                    downloadFile.Click += downloadFile_Click;
                    cmslv.Items.Add(downloadFile);


                    cmslv.Show(lvFiles, e.Location);
                    //this.lvFiles.ContextMenuStrip = cms选中菜单;
                }
                else   //未选中项
                {
                    //this.lvFiles.ContextMenuStrip = cms未选中菜单;
                }
            }
        }

        async void downloadFile_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            var remotepath = getFullPath(tvFolders.SelectedNode);


            var filefolder = new FolderBrowserDialog();

            if (filefolder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var locafolder = filefolder.SelectedPath;


            foreach (var lvFile in lvFiles.SelectedItems)
            {
                var removefilename = ((ListViewItem)lvFile).Text;
                var remotefile = remotepath.TrimEnd('/') + "/" + removefilename;

                var file = await client.OpenFile(remotefile);

                var filebytes = await file.Content.ReadAsByteArrayAsync();

                var locafile = locafolder + "/" + removefilename;

                System.IO.File.WriteAllBytes(locafile, filebytes);
                //lvFiles.Items.Remove((ListViewItem)lvFile);


            }

            MessageBox.Show("下载成功");
        }

        async void tmiDelte_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            var remotepath = getFullPath(tvFolders.SelectedNode);


            foreach (var lvFile in lvFiles.SelectedItems)
            {
                var remotefile = remotepath.TrimEnd('/') + "/" + ((ListViewItem)lvFile).Text;

                await client.DeleteDirectory(remotefile);

                lvFiles.Items.Remove((ListViewItem)lvFile);
            }


        }

        async void tmiEditRoutStation_Click(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
                return;

            var remotepath = getFullPath(tvFolders.SelectedNode);
            var remotefile = remotepath.TrimEnd('/') + "/" + lvFiles.SelectedItems[0].Text;


            InputName ip = new InputName("修改权限(" + remotefile + ")");

            if (ip.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(ip.StrInput))
                {
                    MessageBox.Show("不能为空项");
                    return;
                }

                await client.SetPermissions(remotefile, ip.StrInput);
            }
        }
    }
}