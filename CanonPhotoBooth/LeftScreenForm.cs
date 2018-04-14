using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public partial class LeftScreenForm : Form
    {
        public ChromiumWebBrowser Browser { get; set; }

        public LeftScreenForm()
        {
            InitializeComponent();

            CreateBrowser();
        }

        private void CreateBrowser()
        {
            string filePath = Path.Combine(Application.StartupPath, "View\\screen-common.html");
            Browser = new ChromiumWebBrowser(filePath);

            Browser.BrowserSettings = new BrowserSettings()
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled,
                DefaultEncoding = "UTF8",
            };

            this.Controls.Add(Browser);
            Browser.Dock = DockStyle.Fill;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.Browser != null)
                this.Browser.Dispose();

            Cef.Shutdown();
        }

        private void LeftScreenForm_Load(object sender, EventArgs e)
        {

        }
    }
}
