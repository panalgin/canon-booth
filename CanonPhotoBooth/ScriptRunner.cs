using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonPhotoBooth
{
    public enum ScriptAction
    {
        BlockNewcomers,
        PlayerJoined
    }

    public static class ScriptRunner
    {

        private static Dictionary<ScriptAction, ScriptInfo> ScriptEntities = new Dictionary<ScriptAction, ScriptInfo>()
        {
            { ScriptAction.BlockNewcomers, new ScriptInfo("View\\js\\async\\block-newcomers.js", false) },
            { ScriptAction.PlayerJoined, new ScriptInfo("View\\js\\async\\player-joined.js", false) }
            /*{ ScriptAction.GerberTaskResolved, new ScriptInfo("View\\js\\async\\gerber-resolved.js", false) },
            { ScriptAction.SvgTaskResolved, new ScriptInfo("View\\js\\async\\svg-resolved.js", false ) },
            { ScriptAction.ListPackagesReplied, new ScriptInfo("View\\js\\async\\list-packages-replied.js", false ) },
            { ScriptAction.PnpTaskResolved, new ScriptInfo("View\\js\\async\\pnp-resolved.js", false) },

            /*{ ScriptAction.CommandSent, new ScriptInfo("View\\js\\async\\command-sent.js", false) },
            { ScriptAction.CommandFailed, new ScriptInfo("View\\js\\async\\command-failed.js", false) },
            { ScriptAction.CommandReceived, new ScriptInfo("View\\js\\async\\command-received.js", false) },
            { ScriptAction.Connected, new ScriptInfo("View\\js\\async\\connected.js", false) },
            { ScriptAction.Disconnected, new ScriptInfo("View\\js\\async\\disconnected.js", false) },
            { ScriptAction.PositionChanged, new ScriptInfo("View\\js\\cached\\position-changed.js", false) }*/
        };

        public static void Run(ChromiumWebBrowser browser, ScriptAction action, params object[] parameters)
        {
            var entity = ScriptEntities[action];

            browser.BeginInvoke(new Action(() =>
            {
                string m_Script = string.Empty;

                if (entity.Exists)
                {
                    if (entity.Cacheable == false)
                    {
                        using (StreamReader reader = new StreamReader(entity.FileLocation))
                        {
                            m_Script = reader.ReadToEnd();
                        }
                    }
                    else
                        m_Script = entity.CachedData;

                    var formattedScript = string.Format(m_Script, parameters);

                    browser.ExecuteScriptAsync(formattedScript);
                }
                else
                {
                    throw new FileNotFoundException(entity.FileLocation + " bulunamadı.");
                }
            }));
        }

        private class ScriptInfo
        {
            public string FileLocation { get; private set; }
            public string CachedData { get; private set; }
            public bool Cacheable { get; private set; }

            public bool Exists { get; private set; }

            public ScriptInfo(string path, bool cacheable)
            {
                this.FileLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                this.Cacheable = cacheable;

                if (File.Exists(this.FileLocation))
                {
                    this.Exists = true;

                    using (StreamReader reader = new StreamReader(this.FileLocation))
                    {
                        this.CachedData = reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
