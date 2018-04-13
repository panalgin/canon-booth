using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public class RegistrationJavascriptHandler : BaseJavascriptHandler
    {
        public override void ShowDevTools()
        {
            var regForm = Application.OpenForms.OfType<RegistrationForm>().FirstOrDefault();

            if (regForm != null)
                EventSink.InvokeDevToolsRequested(regForm.GetType());
        }
    }
}
