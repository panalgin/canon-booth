using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
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
        private bool IsBrowserInitialized = false;

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

            LeftJavascriptHandler handler = new LeftJavascriptHandler();

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

        private void LeftScreenForm_Load(object sender, EventArgs e)
        {
            EventSink.DevToolsRequested += EventSink_DevToolsRequested;
            EventSink.PlayerJoined += EventSink_PlayerJoined;
            EventSink.GameInitialized += EventSink_GameInitialized;
            EventSink.GameTriggered += EventSink_GameTriggered;
            EventSink.GameStarted += EventSink_GameStarted;
            EventSink.GameUpdated += EventSink_GameUpdated;
        }

        private void EventSink_GameUpdated(int timeLeft)
        {
            ScriptRunner.Run(this.Browser, ScriptAction.GameUpdated, timeLeft);
        }

        private void EventSink_GameStarted()
        {
            ScriptRunner.Run(this.Browser, ScriptAction.GameStarted, null);
        }

        private void EventSink_GameTriggered()
        {
            ScriptRunner.Run(this.Browser, ScriptAction.CountdownStarted, null);
        }

        private void EventSink_GameInitialized()
        {
            ScriptRunner.Run(this.Browser, ScriptAction.GetReady, null);
        }

        private void EventSink_DevToolsRequested(Type requestedFrom)
        {
            if (requestedFrom == this.GetType())
            {
                if (IsBrowserInitialized)
                {
                    this.Browser.ShowDevTools();
                }
            }
        }

        private void EventSink_PlayerJoined(Player player)
        {
            if (player.Board == World.Boards[0])
            {
                string data = JsonConvert.SerializeObject(player);
                ScriptRunner.Run(this.Browser, ScriptAction.PlayerJoined, Utility.HtmlEncode(data));
            }
        }
    }
}
