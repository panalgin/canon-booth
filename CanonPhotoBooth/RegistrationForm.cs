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
    public partial class RegistrationForm : Form
    {
        public ChromiumWebBrowser Browser { get; set; }

        public RegistrationForm()
        {
            InitializeComponent();
            CreateBrowser();
        }


        private void CreateBrowser()
        {
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

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }
    }
}
