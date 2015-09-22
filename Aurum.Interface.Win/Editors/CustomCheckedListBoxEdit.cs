using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using System.Windows.Forms;
//using DevExpress.XtraEditors.Controls;

namespace Aurum.Interface.Win.Editors
{
    /// <summary>
    /// Пользовательский контрол. 
    /// CheckedListBoxControl с дополнительным CheckBox "Выбрать всех"
    /// </summary>
    public partial class CustomCheckedListBoxEdit : UserControl
    {
        public CustomCheckedListBoxEdit()
        {
            InitializeComponent();
        }

        private void decimalEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void checkAll_CheckedChanged(object sender, EventArgs e)
        {
            this.checkedListBox.ItemCheck -= new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
            if (checkAll.CheckState == CheckState.Checked)
            {
                checkedListBox.CheckAll();
            }
            else if (checkAll.CheckState == CheckState.Unchecked)
            {
                checkedListBox.UnCheckAll();
            }

            this.checkedListBox.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
        }

        public object DataSource
        {
            get { return checkedListBox.DataSource; }
            set
            {
                this.checkedListBox.ItemCheck -= new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
                this.checkAll.CheckedChanged -= new System.EventHandler(this.checkAll_CheckedChanged);

                checkedListBox.UnCheckAll();
                checkAll.Checked = false;
                checkedListBox.DataSource = value;

                this.checkedListBox.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
                this.checkAll.CheckedChanged += new System.EventHandler(this.checkAll_CheckedChanged);
            }
        }

        public string DisplayMember
        {
            get { return checkedListBox.DisplayMember; }
            set { checkedListBox.DisplayMember = value; }
        }

        /// <summary>
        ///  Условие отображения текстового представления элемента
        /// </summary>
        public string ItemTextCriteriaString
        {
            get { return checkedListBox.ItemTextCriteriaString; }
            set { checkedListBox.ItemTextCriteriaString = value; }
        }

        public CriteriaOperator ItemTextCriteria
        {
            get { return checkedListBox.ItemTextCriteria; }
            set { checkedListBox.ItemTextCriteria = value; }
        }

        public void SetItemChecked(int index, bool value)
        {
            checkedListBox.SetItemChecked(index, value);
        }
        
        public object GetItemValue(int index)
        {
            return checkedListBox.GetItemValue(index);
        }

        public event DevExpress.XtraEditors.Controls.ItemCheckEventHandler ItemCheck
        {
            add { checkedListBox.ItemCheck += value; }
            remove { checkedListBox.ItemCheck += value; }
        }

        private void checkedListBox_CheckMemberChanged(object sender, EventArgs e)
        {
        }

        private void checkedListBox_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            this.checkAll.CheckedChanged -= new System.EventHandler(this.checkAll_CheckedChanged);

            var dataSource = checkedListBox.DataSource as DevExpress.Xpo.XPCollection;

            if (dataSource != null && checkedListBox.CheckedItems != null)
            {
                if (dataSource.Count == checkedListBox.CheckedItemsCount) //Если количество выбранных равно общему количеству
                {
                    checkAll.CheckState = CheckState.Checked;
                }
                else if (checkedListBox.CheckedItemsCount == 0) //Если количество выбранных равно нулю
                {
                    checkAll.CheckState = CheckState.Unchecked;
                }
                else 
                {
                    checkAll.CheckState = CheckState.Indeterminate;
                } 
            }

            this.checkAll.CheckedChanged += new System.EventHandler(this.checkAll_CheckedChanged);
        }   
    }
}
