using System.Linq;
using System.Windows.Forms;

namespace CanonPhotoBooth
{
    public class BaseJavascriptHandler
    { 
        public virtual void ShowDevTools()
        {
            var mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();

            if (mainForm != null)
            {
                EventSink.InvokeDevToolsRequested(mainForm.GetType());
            }
        }
    }
}