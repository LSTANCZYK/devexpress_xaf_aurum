namespace Aurum.Interface.Win.Editors
{
    partial class FontEdit
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
            this.editFontFamily = new DevExpress.XtraEditors.FontEdit();
            this.editUnderline = new DevExpress.XtraEditors.CheckButton();
            this.editItalic = new DevExpress.XtraEditors.CheckButton();
            this.editBold = new DevExpress.XtraEditors.CheckButton();
            this.editSize = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.editFontFamily.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editSize.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // editFontFamily
            // 
            this.editFontFamily.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editFontFamily.Location = new System.Drawing.Point(0, 0);
            this.editFontFamily.Name = "editFontFamily";
            this.editFontFamily.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editFontFamily.Size = new System.Drawing.Size(86, 20);
            this.editFontFamily.TabIndex = 0;
            this.editFontFamily.TextChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // editUnderline
            // 
            this.editUnderline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editUnderline.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
            this.editUnderline.Appearance.Options.UseFont = true;
            this.editUnderline.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.editUnderline.Location = new System.Drawing.Point(125, 0);
            this.editUnderline.Name = "editUnderline";
            this.editUnderline.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.editUnderline.Size = new System.Drawing.Size(25, 20);
            this.editUnderline.TabIndex = 1;
            this.editUnderline.Text = "U";
            this.editUnderline.TextChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // editItalic
            // 
            this.editItalic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editItalic.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.editItalic.Appearance.Options.UseFont = true;
            this.editItalic.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.editItalic.Location = new System.Drawing.Point(150, 0);
            this.editItalic.Name = "editItalic";
            this.editItalic.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.editItalic.Size = new System.Drawing.Size(25, 20);
            this.editItalic.TabIndex = 2;
            this.editItalic.Text = "I";
            this.editItalic.TextChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // editBold
            // 
            this.editBold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editBold.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.editBold.Appearance.Options.UseFont = true;
            this.editBold.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.editBold.Location = new System.Drawing.Point(175, 0);
            this.editBold.Name = "editBold";
            this.editBold.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
            this.editBold.Size = new System.Drawing.Size(25, 20);
            this.editBold.TabIndex = 3;
            this.editBold.Text = "B";
            this.editBold.TextChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // editSize
            // 
            this.editSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editSize.Location = new System.Drawing.Point(86, 0);
            this.editSize.Name = "editSize";
            this.editSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.editSize.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.editSize.Properties.Items.AddRange(new object[] {
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24"});
            this.editSize.Size = new System.Drawing.Size(39, 20);
            this.editSize.TabIndex = 4;
            this.editSize.TextChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // FontEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.editSize);
            this.Controls.Add(this.editBold);
            this.Controls.Add(this.editItalic);
            this.Controls.Add(this.editUnderline);
            this.Controls.Add(this.editFontFamily);
            this.MaximumSize = new System.Drawing.Size(0, 20);
            this.MinimumSize = new System.Drawing.Size(200, 20);
            this.Name = "FontEdit";
            this.Size = new System.Drawing.Size(200, 20);
            ((System.ComponentModel.ISupportInitialize)(this.editFontFamily.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editSize.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.FontEdit editFontFamily;
        private DevExpress.XtraEditors.CheckButton editUnderline;
        private DevExpress.XtraEditors.CheckButton editItalic;
        private DevExpress.XtraEditors.CheckButton editBold;
        private DevExpress.XtraEditors.ComboBoxEdit editSize;
    }
}
