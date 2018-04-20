using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public static class Saver
    {
        public static void Initialize()
        {
            EventSink.GameFinished += EventSink_GameFinished;
        }

        static DateTime LastHitTime = DateTime.MinValue;
        private static void EventSink_GameFinished(Player winner)
        {
            if (DateTime.Now - LastHitTime > TimeSpan.FromSeconds(15))
            {
                LastHitTime = DateTime.Now;

                Game.Players.All(delegate (Player player)
                {
                    Save(player);

                    return true;
                });
            }
        }

        public static void Save(Player player)
        {
            string filePath = Path.Combine(Application.StartupPath, "data.bin");

            FileStream fs = null;

            if (!File.Exists(filePath))
                fs = File.Create(filePath);

            if (fs == null)
                fs = new FileStream(filePath, FileMode.Append);

            using (StreamWriter w = new StreamWriter(fs))
            {
                string line = JsonConvert.SerializeObject(player);

                w.WriteLine(line);
            }
        }
    }
}
