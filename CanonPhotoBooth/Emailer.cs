using Mandrill;
using Mandrill.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public static class Emailer
    {
        public static void Send(Player player, string fileName)
        {
            Task.Run(async () =>
            {
                byte[] data = File.ReadAllBytes(fileName);

                var api = new MandrillApi("1_xltjpipEzKbs51aUb2Nw");

                var message = new MandrillMessage("canon@confluence.me", player.Email,
                                "Unlock the scientist in you",
                                "Share your GIF in social media using the hashtags #CanonME and #Think_Science");

                var attachment = new MandrillAttachment("image/gif", "animation.gif", data);
                message.Attachments.Add(attachment);

                var result = await api.Messages.SendAsync(message);
            });
        }
    }
}
