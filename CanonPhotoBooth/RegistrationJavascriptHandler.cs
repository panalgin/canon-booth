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
    public class RegistrationJavascriptHandler : BaseJavascriptHandler
    {
        public override void ShowDevTools()
        {
            var regForm = Application.OpenForms.OfType<RegistrationForm>().FirstOrDefault();

            if (regForm != null)
                EventSink.InvokeDevToolsRequested(regForm.GetType());
        }

        public bool RegisterPlayer(string json)
        {
            var visitor = JsonConvert.DeserializeObject<Visitor>(json);

            if (Game.IsJoinable())
            {
                EventSink.InvokeVisitorRegistered(visitor);

                return true;
            }
            else
                return false;
        }
    }
}
