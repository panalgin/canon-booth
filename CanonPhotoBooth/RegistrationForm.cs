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
    public partial class RegistrationForm : Form
    {
        private bool IsBrowserInitialized = false;

        public ChromiumWebBrowser Browser { get; set; }

        public RegistrationForm()
        {
            InitializeComponent();
            CreateBrowser();
        }


        private void CreateBrowser()
        {
            var filePath = Path.Combine(Application.StartupPath, "View\\registration.html");

            Browser = new ChromiumWebBrowser(filePath);

            Browser.BrowserSettings = new BrowserSettings()
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled,
                DefaultEncoding = "UTF8",
            };

            this.Controls.Add(Browser);
            Browser.Dock = DockStyle.Fill;

            RegistrationJavascriptHandler handler = new RegistrationJavascriptHandler();

            Browser.RegisterAsyncJsObject("windowsApp", handler);
            Browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
        }

        private void Browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            IsBrowserInitialized = e.IsBrowserInitialized;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.Browser != null)
                this.Browser.Dispose();
        }

        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            EventSink.DevToolsRequested += EventSink_DevToolsRequested;
            EventSink.GameInitialized += EventSink_GameInitialized;
            EventSink.GameReset += EventSink_GameReset;
        }

        private void EventSink_GameReset()
        {
            ScriptRunner.Run(this.Browser, ScriptAction.UnblockNewcomers, null);
        }

        private void EventSink_GameInitialized()
        {
            ScriptRunner.Run(this.Browser, ScriptAction.BlockNewcomers, null);
        }

        private void EventSink_DevToolsRequested(Type requestedFrom)
        {
            if (requestedFrom == this.GetType())
            {
                if (IsBrowserInitialized)
                {
                    try
                    {
                        this.Browser.ShowDevTools();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
}
