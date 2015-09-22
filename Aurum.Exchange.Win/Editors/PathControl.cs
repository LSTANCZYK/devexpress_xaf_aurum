using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aurum.Exchange;

namespace Aurum.Exchange.Win.Editors
{
    internal partial class PathControl : UserControl
    {
        public PathControl()
        {
            InitializeComponent();
            MaximumSize = new Size { Width = 0, Height = 23 };
        }

        public bool MultipleOpening
        {
            get;
            set;
        }

        public FilePathMode Mode
        {
            get;
            set;
        }

        public PathControlEntityType EntityType
        {
            get;
            set;
        }

        public string Filter
        {
            get;
            set;
        }

        public object Value
        {
            get
            {
                if (String.IsNullOrEmpty(buttonEdit.Text.Trim()))
                {
                    return null;
                }

                if (EntityType == PathControlEntityType.Directory)
                {
                    return new DirectoryPath(buttonEdit.Text);
                }

                if (Mode == FilePathMode.Open && MultipleOpening)
                {
                    return new MultipleFilePath(buttonEdit.Text);
                }

                return new FilePath(buttonEdit.Text);
            }
            set
            {
                if (value == null)
                {
                    buttonEdit.Text = String.Empty;
                }
                else if (value is EntityPath)
                {
                    buttonEdit.Text = value as EntityPath;
                }
                else if (value is MultipleFilePath)
                {
                    buttonEdit.Text = value as MultipleFilePath;
                }
                else
                {
                    buttonEdit.Text = "# ERROR #";
                }
            }
        }

        private void ShowDialog()
        {
            if (EntityType == PathControlEntityType.File)
            {
                FileDialog dialog = null;

                if (Mode == FilePathMode.Open)
                {
                    dialog = new OpenFileDialog { Multiselect = MultipleOpening };
                }
                else
                {
                    dialog = new SaveFileDialog() { };
                }

                if (!String.IsNullOrEmpty(Filter))
                {
                    dialog.Filter = Filter;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    buttonEdit.Text = String.Join(",", dialog.FileNames);
                }

                dialog.Dispose();
            }
            else
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        buttonEdit.Text = dialog.SelectedPath;
                    }
                }
            }
        }

        private void buttonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Clear)
            {
                buttonEdit.Text = String.Empty;
            }
            else ShowDialog();
        }

        private void buttonEdit_DoubleClick(object sender, EventArgs e)
        {
            ShowDialog();
        }
    }

    internal enum PathControlEntityType
    {
        File,
        Directory
    }
}
