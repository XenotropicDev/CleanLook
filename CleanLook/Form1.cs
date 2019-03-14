using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CleanLook
{
    public partial class SettingsForm : Form
    {
        BindingSource replacementFontBinding = new BindingSource();
        BindingSource systemFontBinding = new BindingSource();        

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            List<string> systemFonts = new List<string>();
            comboBox1.Items.Clear();

            replacementFontBinding.DataSource = Properties.Settings.Default.ListOfFonts;
            listBox1.DataSource = replacementFontBinding;
            txtReplacementFont.Text = Properties.Settings.Default.ReplacementFont;
            checkBox1.Checked = Properties.Settings.Default.Debugging;

            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                FontFamily[] fontFamilies = fontsCollection.Families;
                
                foreach (FontFamily font in fontFamilies)
                {
                    systemFonts.Add(font.Name);
                    comboBox1.Items.Add(font.Name);
                }
            }

            //systemFontBinding.DataSource = systemFonts;
            //comboBox1.DataSource = systemFontBinding;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addFontToList(comboBox1.Text);
            comboBox1.Text = String.Empty;
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
            replacementFontBinding.ResetBindings(false);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Debugging = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void addFontToList(string fontName)
        {
            if (fontName.Trim().Equals(String.Empty)) return;
            if (Properties.Settings.Default.ListOfFonts.Contains(fontName)) return;

            Properties.Settings.Default.ListOfFonts.Add(fontName);            
            Properties.Settings.Default.Save();
            replacementFontBinding.ResetBindings(false);
        }
    }
}
