using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            Cef.Initialize(new CefSettings()
            {
                
            });

            Browser = new ChromiumWebBrowser("View\\screen-common.html");

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
