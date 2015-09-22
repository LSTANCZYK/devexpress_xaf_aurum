namespace Aurum.Interface.Win.Editors
{
    partial class PasswordControl
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
            this.passwordEdit = new DevExpress.XtraEditors.TextEdit();
            this.layoutLabel = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.passwordEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // passwordEdit
            // 
            this.passwordEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordEdit.Location = new System.Drawing.Point(0, 2);
            this.passwordEdit.Name = "passwordEdit";
            this.passwordEdit.Properties.AutoHeight = false;
            this.passwordEdit.Properties.PasswordChar = '*';
            this.passwordEdit.Size = new System.Drawing.Size(334, 20);
            this.passwordEdit.TabIndex = 0;
            // 
            // layoutLabel
            // 
            this.layoutLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.layoutLabel.Appearance.BackColor = System.Drawing.Color.Gray;
            this.layoutLabel.Appearance.ForeColor = System.Drawing.Color.White;
            this.layoutLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.layoutLabel.Location = new System.Drawing.Point(332, 2);
            this.layoutLabel.Name = "layoutLabel";
            this.layoutLabel.Size = new System.Drawing.Size(34, 20);
            this.layoutLabel.TabIndex = 1;
            this.layoutLabel.Text = "EN";
            // 
            // PasswordControl
            // 
            this.Appearance.BorderColor = System.Drawing.Color.Silver;
            this.Appearance.Options.UseBorderColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutLabel);
            this.Controls.Add(this.passwordEdit);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PasswordControl";
            this.Size = new System.Drawing.Size(365, 25);
            this.Load += new System.EventHandler(this.PasswordControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.passwordEdit.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit passwordEdit;
        private DevExpress.XtraEditors.LabelControl layoutLabel;
    }
}
