using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;

namespace Aurum.Interface.Win.Editors
{
    public partial class PasswordControl : XtraUserControl
    {
        public PasswordControl()
        {
            InitializeComponent();
            MaximumSize = new Size(0, 25);
            MinimumSize = new Size(0, 23);
        }

        public string Value
        {
            get { return passwordEdit.Text; }
            set { passwordEdit.Text = value; }
        }

        private void RepresentInputLanguage(CultureInfo culture)
        {
            if (culture == null)
            {
                layoutLabel.Text = "?";
                return;
            }

            layoutLabel.Text = culture.TwoLetterISOLanguageName.ToUpper();
            layoutLabel.ToolTip = culture.NativeName;
        }

        void ParentForm_InputLanguageChanged(object sender, InputLanguageChangedEventArgs e)
        {
            RepresentInputLanguage(e.Culture);
        }

        private void PasswordControl_Load(object sender, EventArgs e)
        {
            ParentForm.InputLanguageChanged += ParentForm_InputLanguageChanged;
            try
            {
                RepresentInputLanguage(InputLanguage.CurrentInputLanguage.Culture);
            }
            catch
            {
                RepresentInputLanguage(null);
            }
        }
    }
}
