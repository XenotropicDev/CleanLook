using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CleanLook
{
    public partial class SettingsForm : Form
    {
        BindingSource bs = new BindingSource();

        //TODO prevent empty or duplicate entries into list

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            bs.DataSource = Properties.Settings.Default.ListOfFonts;
            listBox1.DataSource = bs;
            txtReplacementFont.Text = Properties.Settings.Default.ReplacementFont;
            checkBox1.Checked = Properties.Settings.Default.Debugging;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ListOfFonts.Add(txtAdd.Text);            
            txtAdd.Text = String.Empty;
            Properties.Settings.Default.Save();
            bs.ResetBindings(false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ReplacementFont = txtReplacementFont.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ListOfFonts.RemoveAt(listBox1.SelectedIndex);
            Properties.Settings.Default.Save();
            bs.ResetBindings(false);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Debugging = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }
    }
}
