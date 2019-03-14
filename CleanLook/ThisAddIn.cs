namespace CleanLook
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Outlook = Microsoft.Office.Interop.Outlook;    

    public partial class ThisAddIn
    {
        private Outlook.Explorer currentExplorer = null;

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new CleanLookRibbon();
        }
        
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.currentExplorer = this.Application.ActiveExplorer();
            this.currentExplorer.SelectionChange += new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(this.CurrentExplorer_Event);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        private void CurrentExplorer_Event()
        {
            Outlook.MAPIFolder selectedFolder = this.Application.ActiveExplorer().CurrentFolder;

            try
            {
                if (this.Application.ActiveExplorer().Selection.Count > 0)
                {
                    object selObject = this.Application.ActiveExplorer().Selection[1];
                    if (selObject is Outlook.MailItem)
                    {
                        Outlook.MailItem mailItem = selObject as Outlook.MailItem;

                        var sent = mailItem.Sent;
                        var properties = mailItem.ItemProperties;
                        var window = this.Application.ActiveWindow();

                        HtmlDocument hdoc = new HtmlDocument();
                        hdoc.LoadHtml(mailItem.HTMLBody);
                        var bodyNode = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]");
                        var headStyleNode = hdoc.DocumentNode.SelectSingleNode("/html[1]/head[1]/style[1]");

                        bool toggleSave = false;

                        if (Properties.Settings.Default.CleanStationary && bodyNode != null)
                        {
                            // TODO Allow these to be set by the user?
                            string[] listOfAttributesToRemove = { "bgcolor", "background" };
                            foreach (string bodyAttribute in listOfAttributesToRemove)
                            {
                                var nodeToRemove = bodyNode.ChildAttributes(bodyAttribute);
                                if (nodeToRemove != null && nodeToRemove.Count() > 0)
                                {
                                    nodeToRemove.First().Remove();
                                    toggleSave = true;
                                }
                            }
                        }

                        if (Properties.Settings.Default.CleanFonts)
                        {                            
                            string replacementFont = Properties.Settings.Default.ReplacementFont;
                            var listOfFontsToReplace = Properties.Settings.Default.ListOfFonts;
                            string regexMatchString = string.Join("|", listOfFontsToReplace.Cast<string>());

                            // Replace Fonts according to user settings
                            if (listOfFontsToReplace.Count > 0 && replacementFont.Length > 0)
                            {
                                // Clean fonts from CSS header
                                string tempInnerHmtl = Regex.Replace(headStyleNode.InnerHtml, regexMatchString, replacementFont);
                                if (!tempInnerHmtl.Equals(headStyleNode.InnerHtml))
                                {
                                    headStyleNode.InnerHtml = tempInnerHmtl;
                                    toggleSave = true;
                                }

                                // Clean fonts from body style tags                                
                                string xpathcontains = string.Empty;
                                foreach (string font in listOfFontsToReplace)
                                {
                                    xpathcontains += (xpathcontains.Length > 0) ? $" or contains (@style, '{font}')" : $"contains (@style, '{font}')";
                                }
                                ////var fontNodes = hdoc.DocumentNode.SelectNodes("//*[contains(@style, 'font')]");
                                var fontNodes = hdoc.DocumentNode.SelectNodes($"//*[{xpathcontains}]");
                                if (fontNodes != null)
                                {
                                    foreach (var node in fontNodes)
                                    {
                                        if (node.Attributes != null && node.Attributes.Count > 0)
                                        {
                                            foreach (var attr in node.Attributes)
                                            {
                                                attr.Value = Regex.Replace(attr.Value, regexMatchString, replacementFont);
                                                toggleSave = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Save the HTML back to the body if we made any changes.
                        if (toggleSave)
                        {
                            string html = hdoc.DocumentNode.WriteContentTo();
                            mailItem.HTMLBody = html;                            
                        }
                    }                    
                }                
            }
            catch (Exception ex)
            {                
                if (Properties.Settings.Default.Debugging)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            }            
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
