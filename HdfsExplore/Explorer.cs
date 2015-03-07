using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using Microsoft.Hadoop.WebHDFS;

namespace HdfsExplore
{
    // Author: Paul Li
    // Create Date: 8/1/2002

    public class Explorer : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ImageList m_imageListTreeView;
        private System.ComponentModel.IContainer components;
        private WebHDFSClient client;

        public Explorer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();





            // Populate TreeView with Drive list
            PopulateDriveList();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Explorer));
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.m_imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // tvFolders
            // 
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.m_imageListTreeView;
            this.tvFolders.Location = new System.Drawing.Point(0, 0);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 2;
            this.tvFolders.Size = new System.Drawing.Size(202, 357);
            this.tvFolders.TabIndex = 2;
            this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
            // 
            // m_imageListTreeView
            // 
            this.m_imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListTreeView.ImageStream")));
            this.m_imageListTreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.m_imageListTreeView.Images.SetKeyName(0, "");
            this.m_imageListTreeView.Images.SetKeyName(1, "");
            this.m_imageListTreeView.Images.SetKeyName(2, "");
            this.m_imageListTreeView.Images.SetKeyName(3, "");
            this.m_imageListTreeView.Images.SetKeyName(4, "");
            this.m_imageListTreeView.Images.SetKeyName(5, "");
            this.m_imageListTreeView.Images.SetKeyName(6, "");
            this.m_imageListTreeView.Images.SetKeyName(7, "");
            this.m_imageListTreeView.Images.SetKeyName(8, "");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(202, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 357);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // lvFiles
            // 
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.Location = new System.Drawing.Point(205, 0);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(395, 357);
            this.lvFiles.TabIndex = 4;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
            this.menuItem1.Text = "&File";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "&Close";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4});
            this.menuItem3.Text = "&Help";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Text = "&About";
            // 
            // Explorer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(600, 357);
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tvFolders);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu1;
            this.Name = "Explorer";
            this.Text = "My Window Explorer";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>


        //This procedure populate the TreeView with the Drive list
        private async void PopulateDriveList()
        {
            TreeNode nodeTreeNode;
            int imageIndex = 0;
            int selectIndex = 0;

            const int Removable = 2;
            const int LocalDisk = 3;
            const int Network = 4;
            const int CD = 5;
            //const int RAMDrive = 6;

            this.Cursor = Cursors.WaitCursor;
            //clear TreeView
            tvFolders.Nodes.Clear();
            nodeTreeNode = new TreeNode("Hadoop Root", 0, 0);
            tvFolders.Nodes.Add(nodeTreeNode);

            //set node collection
            TreeNodeCollection nodeCollection = nodeTreeNode.Nodes;

            //Get Drive list
            client = new WebHDFSClient(new Uri("http://zhangbaowei:50070"), "hadoop");

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

            this.Cursor = Cursors.Default;

        }

        private void tvFolders_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            //Populate folders and files when a folder is selected
            this.Cursor = Cursors.WaitCursor;

            //get current selected drive or folder
            TreeNode nodeCurrent = e.Node;

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
            this.Cursor = Cursors.Default;
        }

        protected void InitListView()
        {
            //init ListView control
            lvFiles.Clear();		//clear control
            //create column header for ListView
            lvFiles.Columns.Add("Name", 150, System.Windows.Forms.HorizontalAlignment.Left);
            lvFiles.Columns.Add("Size", 75, System.Windows.Forms.HorizontalAlignment.Right);
            lvFiles.Columns.Add("Created", 140, System.Windows.Forms.HorizontalAlignment.Left);
            lvFiles.Columns.Add("Modified", 140, System.Windows.Forms.HorizontalAlignment.Left);

        }

        protected async void PopulateDirectory(TreeNode nodeCurrent, TreeNodeCollection nodeCurrentCollection)
        {
            TreeNode nodeDir;
            int imageIndex = 2;		//unselected image index
            int selectIndex = 3;	//selected image index

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

                    string[] stringDirectories = dirs.Select(x => x.PathSuffix).ToArray();
                    string stringFullPath = "";
                    string stringPathName = "";

                    //loop throught all directories
                    foreach (string stringDir in stringDirectories)
                    {
                        stringFullPath = stringDir;
                        stringPathName = GetPathName(stringFullPath);

                        //create node for directories
                        nodeDir = new TreeNode(stringPathName.ToString(), imageIndex, selectIndex);
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
            string[] stringSplit = stringPath.Split('\\');
            int _maxIndex = stringSplit.Length;
            return stringSplit[_maxIndex - 1];
        }

        protected async void PopulateFiles(TreeNode nodeCurrent)
        {

            //Populate listview with files
            string[] lvData = new string[4];

            //clear list
            InitListView();

            if (nodeCurrent.SelectedImageIndex != 0)
            {
                var directoryStatus = await client.GetDirectoryStatus(getFullPath(nodeCurrent.FullPath));



                try
                {
                    var Files = directoryStatus.Files;// Directory.GetFiles(getFullPath(nodeCurrent.FullPath));
                    string stringFileName = "";
                    string dtCreateDate, dtModifyDate;
                    Int64 lFileSize = 0;

                    //loop throught all files
                    foreach (var stringFile in Files)
                    {
                        stringFileName = stringFile.PathSuffix;
                        //FileInfo objFileSize = new FileInfo(stringFileName);
                        lFileSize = stringFile.Length;
                        dtCreateDate = stringFile.AccessTime.ToString(); //GetCreationTime(stringFileName);
                        dtModifyDate = stringFile.ModificationTime.ToString(); //GetLastWriteTime(stringFileName);

                        //create listview data
                        lvData[0] = GetPathName(stringFileName);
                        lvData[1] = formatSize(lFileSize);


                        //is in day light saving time adjust time
                        lvData[2] = dtCreateDate;

                        //not in day light saving time adjust time
                        lvData[3] = dtModifyDate;



                        //Create actual list item
                        ListViewItem lvItem = new ListViewItem(lvData, 0);
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
            string stringParse = "";
            //remove My Computer from path.
            stringParse = stringPath.Replace("Hadoop Root\\", "/").Replace("\\", "/");

            return stringParse;
        }


        protected string formatDate(DateTime dtDate)
        {
            //Get date and time in short format
            string stringDate = "";

            stringDate = dtDate.ToShortDateString().ToString() + " " + dtDate.ToShortTimeString().ToString();

            return stringDate;
        }

        protected string formatSize(Int64 lSize)
        {
            //Format number to KB
            string stringSize = "";
            NumberFormatInfo myNfi = new NumberFormatInfo();

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

        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            //quit application
            this.Close();
        }

    }
}
