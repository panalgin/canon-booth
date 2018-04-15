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
    public partial class RightScreenForm : Form
    {
        public ChromiumWebBrowser Browser { get; set; }

        public RightScreenForm()
        {
            InitializeComponent(); CreateBrowser();
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

        private void RightScreenForm_Load(object sender, EventArgs e)
        {
            EventSink.PlayerJoined += EventSink_PlayerJoined;
            EventSink.GameInitialized += EventSink_GameInitialized;
            EventSink.GameTriggered += EventSink_GameTriggered;
            EventSink.GameStarted += EventSink_GameStarted;
            EventSink.GameUpdated += EventSink_GameUpdated;
            EventSink.PlayerUpdated += EventSink_PlayerUpdated;
            EventSink.GameFinished += EventSink_GameFinished;
            EventSink.GifGenerated += EventSink_GifGenerated;
        }

        private void EventSink_GifGenerated(int playerIndex, string filePath)
        {
            if (playerIndex == 1)
            {
                ScriptRunner.Run(this.Browser, ScriptAction.GifGenerated, Utility.HtmlEncode(filePath));
            }
        }

        private void EventSink_GameFinished()
        {
            ScriptRunner.Run(this.Browser, ScriptAction.GameFinished, null);
        }

        private void EventSink_PlayerUpdated(Player player)
        {
            if (player.Board == World.Boards[1])
            {
                var data = JsonConvert.SerializeObject(player);
                ScriptRunner.Run(this.Browser, ScriptAction.PlayerUpdated, Utility.HtmlEncode(data));
            }
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

        private void EventSink_PlayerJoined(Player player)
        {
            if (player.Board == World.Boards[1])
            {
                string data = JsonConvert.SerializeObject(player);
                ScriptRunner.Run(this.Browser, ScriptAction.PlayerJoined, Utility.HtmlEncode(data));
            }
        }
    }
}
