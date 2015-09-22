using DevExpress.ExpressApp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurum.Menu.Templates
{
    public interface ISupportStoreSettingsEx : ISupportStoreSettings
    {
        event EventHandler SettingsSaved;
    }
}
