using System;
using System.Windows.Forms;

namespace HdfsExplore
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AppConfig.SetValue("hdfsurl", textBox1.Text);
            AppConfig.SetValue("username", textBox2.Text);

            DialogResult = DialogResult.OK;

            Close();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            textBox1.Text = AppConfig.GetValue("hdfsurl");
            textBox2.Text = AppConfig.GetValue("username");
        }
    }
}