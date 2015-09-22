namespace Aurum.Interface.Win.Filters
{
    partial class QueryGridStringFilterBase
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonExact = new System.Windows.Forms.Button();
            this.buttonCase = new System.Windows.Forms.Button();
            this.buttonNull = new System.Windows.Forms.Button();
            this.SuspendLayout();
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
            // buttonExact
            // 
            this.buttonExact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExact.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.buttonExact.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExact.Location = new System.Drawing.Point(256, 0);
            this.buttonExact.Name = "buttonExact";
            this.buttonExact.Size = new System.Drawing.Size(23, 20);
            this.buttonExact.TabIndex = 10;
            this.buttonExact.TabStop = false;
            this.toolTip1.SetToolTip(this.buttonExact, "Точный поиск");
            this.buttonExact.UseVisualStyleBackColor = true;
            this.buttonExact.Click += new System.EventHandler(this.OnExactClick);
            // 
            // buttonCase
            // 
            this.buttonCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCase.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.buttonCase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCase.Location = new System.Drawing.Point(282, 0);
            this.buttonCase.Margin = new System.Windows.Forms.Padding(0);
            this.buttonCase.Name = "buttonCase";
            this.buttonCase.Size = new System.Drawing.Size(23, 20);
            this.buttonCase.TabIndex = 11;
            this.buttonCase.TabStop = false;
            this.toolTip1.SetToolTip(this.buttonCase, "Поиск с учетом регистра");
            this.buttonCase.UseVisualStyleBackColor = true;
            this.buttonCase.Click += new System.EventHandler(this.OnCaseClick);
            // 
            // buttonNull
            // 
            this.buttonNull.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNull.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.buttonNull.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNull.Location = new System.Drawing.Point(308, 0);
            this.buttonNull.Name = "buttonNull";
            this.buttonNull.Size = new System.Drawing.Size(23, 20);
            this.buttonNull.TabIndex = 12;
            this.buttonNull.TabStop = false;
            this.toolTip1.SetToolTip(this.buttonNull, "Поиск пустых значений");
            this.buttonNull.UseVisualStyleBackColor = true;
            this.buttonNull.Click += new System.EventHandler(this.OnNullsClick);
            // 
            // QueryGridStringFilterBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.buttonNull);
            this.Controls.Add(this.buttonCase);
            this.Controls.Add(this.buttonExact);
            this.Name = "QueryGridStringFilterBase";
            this.Size = new System.Drawing.Size(332, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;

        /// <summary>
        /// Кнопка точного поиска
        /// </summary>
        protected System.Windows.Forms.Button buttonExact;

        /// <summary>
        /// Кнопка поиска с учетом регистра
        /// </summary>
        protected System.Windows.Forms.Button buttonCase;

        /// <summary>
        /// Кнопка поиска пустых значений
        /// </summary>
        protected System.Windows.Forms.Button buttonNull;
    }
}
