using DevExpress.Accessibility;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;

namespace Aurum.Operations.Win.Editors
{
    [UserRepositoryItem("RepositoryItemPercentageProgressRegister")]
    public class RepositoryItemPercentageProgress : RepositoryItemProgressBar
    {
        internal const string PercentageProgressEditorName = "PercentageProgressEditor";
        public override string EditorTypeName
        {
            get
            {
                return "PercentageProgressEditor";
            }
        }
        static RepositoryItemPercentageProgress()
        {
            RepositoryItemPercentageProgress.RepositoryItemPercentageProgressRegister();
        }
        public static void RepositoryItemPercentageProgressRegister()
        {
            if (EditorRegistrationInfo.Default.Editors.Contains("PercentageProgressEditor"))
            {
                return;
            }
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("PercentageProgressEditor", typeof(PercentageProgressEditor), typeof(RepositoryItemPercentageProgress), typeof(ProgressBarViewInfo), new ProgressBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(ProgressBarAccessible)));
        }
    }
}
