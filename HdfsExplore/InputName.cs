using System;
using System.Windows.Forms;

namespace HdfsExplore
{
    public partial class InputName : Form
    {


        public InputName()
        {
            InitializeComponent();
        }


        public InputName(string title)
            : this()
        {

            this.Text = title;
        }


        public InputName(string title, string inputtext)
            : this(title)
        {
            StrInput = inputtext;
            this.textBox1.Text = inputtext;
        }

        public string StrInput { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            StrInput = textBox1.Text.Trim();
            DialogResult = DialogResult.OK;

            Close();
        }
    }
}