using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CanonPhotoBooth
{
    public static class Utility
    {
        public static string HtmlEncode(string unencoded)
        {
            return HttpUtility.JavaScriptStringEncode(unencoded, false);
        }
    }
}
