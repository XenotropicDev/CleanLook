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
        private bool disableTextChangeEvent = false;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {            
            addFontComboBox.Items.Clear();
            replacementFontComboBox.Items.Clear();

            replacementFontBinding.DataSource = Properties.Settings.Default.ListOfFonts;
            listBox1.DataSource = replacementFontBinding;

            replacementFontComboBox.Text = Properties.Settings.Default.ReplacementFont;
            checkBox1.Checked = Properties.Settings.Default.Debugging;

            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                FontFamily[] fontFamilies = fontsCollection.Families;
                
                foreach (FontFamily font in fontFamilies)
                {
                    addFontComboBox.Items.Add(font.Name);
                    replacementFontComboBox.Items.Add(font.Name);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addFontToList(addFontComboBox.Text);
            addFontComboBox.Text = String.Empty;
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

        private void replacementFontComboBox_TextChanged(object sender, EventArgs e)
        {
            if (disableTextChangeEvent)
            {
                return;
            }

            disableTextChangeEvent = true;
            saveReplacementFont(replacementFontComboBox.Text);
            disableTextChangeEvent = false;
        }

        private void addFontToList(string fontName)
        {
            if (fontName.Trim().Equals(String.Empty)) return;
            if (Properties.Settings.Default.ListOfFonts.Contains(fontName)) return;
            if (fontName.Equals(Properties.Settings.Default.ReplacementFont)) return;

            Properties.Settings.Default.ListOfFonts.Add(fontName);            
            Properties.Settings.Default.Save();
            replacementFontBinding.ResetBindings(false);
        }

        private void saveReplacementFont(string fontName)
        {
            if (fontName.Trim().Equals(String.Empty)) return;

            if (listBox1.Items.Contains(fontName))
            {
                //TODO fix double pop-up bug
                MessageBox.Show("You are playing with fire trying to set the replacement font to a replaced font.", "On Second thought, let's not do that, tis a silly thing to do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                
                replacementFontComboBox.Text = Properties.Settings.Default.ReplacementFont;             
                return;
            }

            Properties.Settings.Default.ReplacementFont = fontName;
            Properties.Settings.Default.Save();
        }
    }
}
