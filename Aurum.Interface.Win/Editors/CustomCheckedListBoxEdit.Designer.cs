namespace Aurum.Interface.Win.Editors
{
    partial class CustomCheckedListBoxEdit
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkAll = new DevExpress.XtraEditors.CheckEdit();
            this.checkedListBox = new Aurum.Interface.Win.Editors.AurumCheckedListBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.checkAll.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBox)).BeginInit();
            this.SuspendLayout();
            // 
            // checkAll
            // 
            this.checkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkAll.Location = new System.Drawing.Point(0, 24);
            this.checkAll.Name = "checkAll";
            this.checkAll.Properties.Caption = "Выбрать всех";
            this.checkAll.Size = new System.Drawing.Size(94, 19);
            this.checkAll.TabIndex = 1;
            this.checkAll.CheckedChanged += new System.EventHandler(this.checkAll_CheckedChanged);
            // 
            // checkedListBox
            // 
            this.checkedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox.Appearance.ForeColor = System.Drawing.SystemColors.GrayText;
            this.checkedListBox.Appearance.Options.UseForeColor = true;
            this.checkedListBox.CheckOnClick = true;
            this.checkedListBox.ItemTextCriteria = null;
            this.checkedListBox.ItemTextCriteriaString = null;
            this.checkedListBox.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(235, 23);
            this.checkedListBox.TabIndex = 0;
            this.checkedListBox.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
            this.checkedListBox.CheckMemberChanged += new System.EventHandler(this.checkedListBox_CheckMemberChanged);
            this.checkedListBox.SelectedIndexChanged += new System.EventHandler(this.decimalEdit1_SelectedIndexChanged);
            // 
            // CustomCheckedListBoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkAll);
            this.Controls.Add(this.checkedListBox);
            this.Name = "CustomCheckedListBoxControl";
            this.Size = new System.Drawing.Size(235, 46);
            ((System.ComponentModel.ISupportInitialize)(this.checkAll.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkedListBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AurumCheckedListBoxControl checkedListBox;
        private DevExpress.XtraEditors.CheckEdit checkAll;

    }
}
