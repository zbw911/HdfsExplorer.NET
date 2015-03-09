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

    public partial class Explorer : System.Windows.Forms.Form
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
      
        private MenuItem menuItem5;
        private MenuItem menuItem6;
        private ContextMenuStrip cmsOneRout;



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
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
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
            this.tvFolders.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvFolders_NodeMouseClick);
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
            this.menuItem3,
            this.menuItem5,
            this.menuItem6,
            this.menuItem7});
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
            // menuItem5
            // 
            this.menuItem5.Index = 2;
            this.menuItem5.Text = "CreateFolder";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 3;
            this.menuItem6.Text = "Setting";
            this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 4;
            this.menuItem7.Text = "UploadFile";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
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

        private MenuItem menuItem7;

        

    }
}
