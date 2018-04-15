using CanonPhotoBooth.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public class LeftJavascriptHandler : BaseJavascriptHandler
    {
        public override void ShowDevTools()
        {
            var leftForm = Application.OpenForms.OfType<LeftScreenForm>().FirstOrDefault();

            if (leftForm != null)
                EventSink.InvokeDevToolsRequested(leftForm.GetType());
        }
    }
}
