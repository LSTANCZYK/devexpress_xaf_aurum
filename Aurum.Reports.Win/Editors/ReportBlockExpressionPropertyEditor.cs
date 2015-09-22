using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Aurum.Reports.Win.Editors
{
    /// <summary>
    /// Редактор свойства выражения в блоке отчета
    /// </summary>
    [PropertyEditor(typeof(string), ReportWizardItem.ExpressionPropertyEditorAlias, false)]
    public class ReportWizardExpressionPropertyEditor : PopupExpressionPropertyEditor
    {
        public ReportWizardExpressionPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) 
        {
        }

        protected override object CreateControlCore()
        {
            ReportWizardExpressionEdit edit = new ReportWizardExpressionEdit();
            return edit;
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemReportWizardExpressionEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            RepositoryItemReportWizardExpressionEdit expressionEdit = item as RepositoryItemReportWizardExpressionEdit;
            expressionEdit.EditingObject = CurrentObject;
            expressionEdit.DataColumnInfo = new ReportWizardDataInfo(ObjectTypeInfo, MemberInfo);
            expressionEdit.DataColumnInfo.Update(CurrentObject);
        }
    }

    public class RepositoryItemReportWizardExpressionEdit : RepositoryItemPopupExpressionEdit
    {
        static RepositoryItemReportWizardExpressionEdit() { RepositoryItemReportWizardExpressionEdit.RegisterEdit(); }

        internal ReportWizardDataInfo DataColumnInfo { get; set; }

        internal static void RegisterEdit()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(RegisteredEditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(RegisteredEditorName, typeof(ReportWizardExpressionEdit),
                    typeof(RepositoryItemReportWizardExpressionEdit), typeof(ButtonEditViewInfo),
                    new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
            }
        }

        internal static string RegisteredEditorName
        {
            get { return typeof(ReportWizardExpressionEdit).Name; }
        }

        public override void Assign(RepositoryItem item)
        {
            base.Assign(item);
            RepositoryItemReportWizardExpressionEdit sourceItem = (RepositoryItemReportWizardExpressionEdit)item;
            DataColumnInfo = sourceItem.DataColumnInfo;
        }
    }

    /// <summary>
    /// Редактор выражения в блоке отчета
    /// </summary>
    // Копия PopupExpressionEdit
    [ToolboxItem(false)]
    public class ReportWizardExpressionEdit : ButtonEdit, IGridInplaceEdit
    {
        static ReportWizardExpressionEdit() { RepositoryItemReportWizardExpressionEdit.RegisterEdit(); }

        protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo)
        {
            base.OnClickButton(buttonInfo);
            Properties.DataColumnInfo.Update(GridEditingObject);
            Properties.DataColumnInfo.UnboundExpression = (string)EditValue;
            using (UnboundColumnExpressionEditorForm expressionEditorForm = new ReportWizardExpressionEditorForm(Properties.DataColumnInfo, null))
            {
                if (expressionEditorForm.ShowDialog() == DialogResult.OK)
                {
                    EditValue = expressionEditorForm.Expression;
                }
            }
        }

        public override string EditorTypeName
        {
            get { return RepositoryItemReportWizardExpressionEdit.RegisteredEditorName; }
        }

        public new RepositoryItemReportWizardExpressionEdit Properties
        {
            get { return (RepositoryItemReportWizardExpressionEdit)base.Properties; }
        }

        #region IGridInplaceEdit Members
        
        public object GridEditingObject
        {
            get { return Properties.EditingObject; }
            set { Properties.EditingObject = value; }
        }

        #endregion
    }

    public class ReportWizardExpressionEditorForm : XafExpressionEditorForm
    {
        public ReportWizardExpressionEditorForm(object contextInstance, IDesignerHost designerHost)
            : base(contextInstance, designerHost) 
        { 
        }

        protected override ExpressionEditorLogic CreateExpressionEditorLogic()
        {
            return new ReportWizardExpressionEditorLogic(this, (IDataColumnInfo)ContextInstance);
        }
    }

    #region DevExpress.ExpressApp.Win\Editors\ExpressionPropertyEditor

    internal class XafDataColumnInfo : IDataColumnInfo 
    {
		private IMemberInfo memberInfo;
		private ITypeInfo ownerTypeInfo;

		public XafDataColumnInfo(ITypeInfo ownerTypeInfo, IMemberInfo memberInfo) 
        {
			this.ownerTypeInfo = ownerTypeInfo;
			this.memberInfo = memberInfo;
		}

		public string Caption 
        {
			get { return CaptionHelper.GetMemberCaption(ownerTypeInfo, memberInfo.Name); }
		}

		public List<IDataColumnInfo> Columns 
        {
			get 
            {
				List<IDataColumnInfo> result = new List<IDataColumnInfo>();
				foreach(IMemberInfo member in ownerTypeInfo.Members) 
                {
					if(member.IsVisible && member.IsPublic) 
                    {
						result.Add(new XafDataColumnInfo(member.Owner, member));
					}
				}
				return result;
			}
		}

		public DataControllerBase Controller 
        {
			get { return null; }
		}

		public string FieldName 
        {
			get { return memberInfo.BindingName; }
		}

		public Type FieldType 
        {
			get { return memberInfo.MemberType; }
		}

		public string Name 
        {
			get { return memberInfo.Name; }
		}
		
        public string UnboundExpression { get; set; }
		
        // virtual
        public virtual void Update(object currentObject) 
        {
			ElementTypePropertyAttribute typeMemberAttribute = memberInfo.FindAttribute<ElementTypePropertyAttribute>(true);
			ITypeInfo targetTypeInfo = ownerTypeInfo;
			if(currentObject != null) 
            {
				targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(currentObject.GetType());
                if (targetTypeInfo != null && typeMemberAttribute != null)
                {
                    IMemberInfo objectTypeMember = targetTypeInfo.FindMember(typeMemberAttribute.Name);
                    if (objectTypeMember != null)
                    {
                        ITypeInfo elementTypeInfo = XafTypesInfo.Instance.FindTypeInfo(objectTypeMember.GetValue(currentObject) as Type);
                        ownerTypeInfo = elementTypeInfo ?? ownerTypeInfo;
                    }
                }
			}
		}
	}

    internal class AggregatesClickHelper : ItemClickHelper
    {
        protected override void FillItemsTable()
        {
            this.AddItemTable("Avg()", CaptionHelper.GetLocalizedText("Texts", "AggregatesAvgDescription"), -1);
            this.AddItemTable("Count", CaptionHelper.GetLocalizedText("Texts", "AggregatesCountDescription"), -1);
            this.AddItemTable("Max()", CaptionHelper.GetLocalizedText("Texts", "AggregatesMaxDescription"), -1);
            this.AddItemTable("Min()", CaptionHelper.GetLocalizedText("Texts", "AggregatesMinDescription"), -1);
            this.AddItemTable("Sum()", CaptionHelper.GetLocalizedText("Texts", "AggregatesSumDescription"), -1);
        }

        public AggregatesClickHelper(IExpressionEditor editor)
            : base(editor) { }
        
        public override int GetCursorOffset(string item)
        {
            return 1;
        }
    }

    #endregion

    internal class ReportWizardDataInfo : XafDataColumnInfo
    {
        public Dictionary<string, string> Parameters;

        public ReportWizardDataInfo(ITypeInfo ownerTypeInfo, IMemberInfo memberInfo) 
            : base(ownerTypeInfo, memberInfo) { }

        public override void Update(object currentObject)
        {
            base.Update(currentObject);

            // Параметры отчета
            if (currentObject is ReportWizardItem)
            {
                ReportWizardItem item = (ReportWizardItem)currentObject;
                Parameters = new Dictionary<string, string>();
                item.FillParameters(Parameters);
            }
        }
    }

    internal class ReportWizardExpressionEditorLogic : ExpressionEditorLogicEx
    {
        private string additionalFunctionsTitle = "Aggregate operations";
        private string parametersTitle;

        public ReportWizardExpressionEditorLogic(IExpressionEditor editor, IDataColumnInfo columnInfo)
            : base(editor, columnInfo)
        {
            additionalFunctionsTitle = CaptionHelper.GetLocalizedText("Texts", "Aggregates");
            parametersTitle = editor.GetResourceString("Parameters.Text");
        }

        protected override object[] GetListOfInputTypesObjects()
        {
            List<object> result = new List<object>(base.GetListOfInputTypesObjects());
            result.Add(additionalFunctionsTitle);
            result.Add(parametersTitle);
            return result.ToArray();
        }

        protected override ItemClickHelper GetItemClickHelper(string selectedItemText, IExpressionEditor editor)
        {
            if (selectedItemText == additionalFunctionsTitle)
                return new AggregatesClickHelper(editor);
            return base.GetItemClickHelper(selectedItemText, editor);
        }

        protected override void FillParametersTable(Dictionary<string, string> itemsTable)
        {
            ReportWizardDataInfo reportInfo = contextInstance as ReportWizardDataInfo;
            if (reportInfo != null && reportInfo.Parameters != null)
            {
                foreach (KeyValuePair<string, string> keyValue in reportInfo.Parameters)
                    itemsTable.Add(keyValue.Key, keyValue.Value);
            }
        }
    }
}
