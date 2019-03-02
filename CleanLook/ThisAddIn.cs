using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
//using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;

namespace CleanLook
{
    public partial class ThisAddIn
    {
        Outlook.Explorer currentExplorer = null;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            currentExplorer = this.Application.ActiveExplorer();
            currentExplorer.SelectionChange += new Outlook
                .ExplorerEvents_10_SelectionChangeEventHandler
                (CurrentExplorer_Event);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new CleanLookRibbon();
        }

        private void CurrentExplorer_Event()
        {
            Outlook.MAPIFolder selectedFolder = this.Application.ActiveExplorer().CurrentFolder;

            try
            {
                if (this.Application.ActiveExplorer().Selection.Count > 0)
                {
                    Object selObject = this.Application.ActiveExplorer().Selection[1];
                    if (selObject is Outlook.MailItem)
                    {
                        Outlook.MailItem mailItem = (selObject as Outlook.MailItem);

                        var sent = mailItem.Sent;
                        var properties = mailItem.ItemProperties;
                        var window = this.Application.ActiveWindow();

                        HtmlDocument hdoc = new HtmlDocument();
                        hdoc.LoadHtml(mailItem.HTMLBody);
                        var bodyNode = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]");

                        bool toggleSave = false;

                        if (Properties.Settings.Default.CleanStationary && bodyNode != null)
                        {
                            //TODO Allow these to be set by the user?
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
                            //TODO: maybe there should be a 1:1 mapping?                        
                            string replacementFont = Properties.Settings.Default.ReplacementFont;
                            var listOfFontsToReplace = Properties.Settings.Default.ListOfFonts;

                            //Replace Fonts according to user settings
                            if (listOfFontsToReplace.Count > 0 && replacementFont.Length > 0)
                            {
                                //var fontNodes = hdoc.DocumentNode.SelectNodes("//*[contains(@style, 'font')]");
                                string xpathcontains = string.Empty;
                                foreach (string font in listOfFontsToReplace)
                                {
                                    xpathcontains += (xpathcontains.Length > 0) ? $" or contains (@style, '{font}')" : $"contains (@style, '{font}')";
                                }
                                var fontNodes = hdoc.DocumentNode.SelectNodes($"//*[{xpathcontains}]");
                                if (fontNodes != null)
                                {
                                    foreach (var node in fontNodes)
                                    {
                                        if (node.Attributes != null && node.Attributes.Count > 0)
                                        {
                                            foreach (var attr in node.Attributes)
                                            {
                                                attr.Value = Regex.Replace(attr.Value, String.Join("|", listOfFontsToReplace.Cast<string>()), replacementFont);
                                                toggleSave = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Save the HTML back to the body if we made any changes.
                        if (toggleSave)
                        {
                            string html = hdoc.DocumentNode.WriteContentTo();
                            mailItem.HTMLBody = html;                            
                        }

                    }
                    else if (selObject is Outlook.ContactItem)
                    {
                        Outlook.ContactItem contactItem = (selObject as Outlook.ContactItem);                        
                    }
                    else if (selObject is Outlook.AppointmentItem)
                    {
                        Outlook.AppointmentItem apptItem = (selObject as Outlook.AppointmentItem);

                    }
                    else if (selObject is Outlook.TaskItem)
                    {
                        Outlook.TaskItem taskItem = (selObject as Outlook.TaskItem);                        
                    }
                    else if (selObject is Outlook.MeetingItem)
                    {
                        Outlook.MeetingItem meetingItem = (selObject as Outlook.MeetingItem);
                        
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
