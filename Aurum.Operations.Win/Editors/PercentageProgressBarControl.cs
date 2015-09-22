using DevExpress.XtraEditors;
using System;

namespace Aurum.Operations.Win.Editors
{
    public class PercentageProgressBarControl : ProgressBarControl
    {
        public override string EditorTypeName
        {
            get
            {
                return "PercentageProgressEditor";
            }
        }
        public new RepositoryItemPercentageProgress Properties
        {
            get
            {
                return (RepositoryItemPercentageProgress)base.Properties;
            }
        }
        static PercentageProgressBarControl()
        {
            RepositoryItemPercentageProgress.RepositoryItemPercentageProgressRegister();
        }
    }
}
