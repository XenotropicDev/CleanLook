using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;

namespace CleanLook
{
    [ComVisible(true)]
    public class CleanLookRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public CleanLookRibbon()
        {
            
        }

        public bool GetStationaryPressed(Office.IRibbonControl control)
        {
            return Properties.Settings.Default.CleanStationary;
        }

        public void CleanStationaryChecked(Office.IRibbonControl control, bool Checked)
        {
            Properties.Settings.Default.CleanStationary = Checked;
            Properties.Settings.Default.Save();
        }

        public bool GetFontsPressed(Office.IRibbonControl control)
        {
            return Properties.Settings.Default.CleanFonts;
        }

        public void CleanFontsChecked(Office.IRibbonControl control, bool Checked)
        {
            Properties.Settings.Default.CleanFonts = Checked;
            Properties.Settings.Default.Save();
        }

        public void OnSettingsButton(Office.IRibbonControl control)
        {
            var settingsForm = new SettingsForm();
            settingsForm.Show();
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("CleanLook.CleanLookRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit http://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;            
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
