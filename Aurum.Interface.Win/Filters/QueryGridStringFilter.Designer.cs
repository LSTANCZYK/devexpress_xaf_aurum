namespace Aurum.Interface.Win.Filters
{
    partial class QueryGridStringFilter
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
            this.components = new System.ComponentModel.Container();
            this.textBoxEdit = new Aurum.Interface.Win.Filters.TextBoxEdit();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // buttonExact
            // 
            this.buttonExact.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            // 
            // buttonCase
            // 
            this.buttonCase.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            // 
            // buttonNull
            // 
            this.buttonNull.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            // 
            // textBoxEdit
            // 
            this.textBoxEdit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxEdit.Location = new System.Drawing.Point(0, 0);
            this.textBoxEdit.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxEdit.MinimumSize = new System.Drawing.Size(100, 20);
            this.textBoxEdit.Name = "textBoxEdit";
            this.textBoxEdit.Size = new System.Drawing.Size(253, 20);
            this.textBoxEdit.TabIndex = 5;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 1;
            this.toolTip1.AutoPopDelay = 60000;
            this.toolTip1.InitialDelay = 1;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 0;
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // QueryGridStringFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.textBoxEdit);
            this.Name = "QueryGridStringFilter";
            this.Enter += new System.EventHandler(this.QueryGridStringFilter_Enter);
            this.Resize += new System.EventHandler(this.QueryGridStringFilter_Resize);
            this.Controls.SetChildIndex(this.textBoxEdit, 0);
            this.Controls.SetChildIndex(this.buttonExact, 0);
            this.Controls.SetChildIndex(this.buttonCase, 0);
            this.Controls.SetChildIndex(this.buttonNull, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBoxEdit textBoxEdit;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
