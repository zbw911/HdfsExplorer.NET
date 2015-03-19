using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HdfsExplore
{
    public partial class ContentViewer : Form
    {
        private readonly string _content;
        private readonly string _title;

        public ContentViewer(string content, string title)
        {
            _content = content;
            _title = title;
            InitializeComponent();
        }

        private void ContentViewr_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = (_content ?? "").Replace("\r\n", "\n").Replace("\n", Environment.NewLine); ;
            this.Text = _title;

            this.Focus();
        }

        private void ContentViewr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }


    }
}
