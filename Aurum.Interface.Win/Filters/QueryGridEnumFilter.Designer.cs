namespace Aurum.Interface.Win.Filters
{
    partial class QueryGridEnumFilter
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
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.checkBoxNull = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox1.MinimumSize = new System.Drawing.Size(0, 49);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(178, 49);
            this.checkedListBox1.TabIndex = 0;
            // 
            // checkBoxNull
            // 
            this.checkBoxNull.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxNull.AutoSize = true;
            this.checkBoxNull.Location = new System.Drawing.Point(187, 3);
            this.checkBoxNull.Name = "checkBoxNull";
            this.checkBoxNull.Size = new System.Drawing.Size(15, 14);
            this.checkBoxNull.TabIndex = 1;
            this.checkBoxNull.TabStop = false;
            this.checkBoxNull.ThreeState = true;
            this.toolTip1.SetToolTip(this.checkBoxNull, "Поиск пустого/непустого значения.\r\nЕсли галочка:\r\n1. Не определена - поиск по умо" +
        "лчанию\r\n2. Установлена - поиск непустого значения\r\n3. Не установлена - поиск пус" +
        "того значения");
            this.checkBoxNull.UseVisualStyleBackColor = true;
            this.checkBoxNull.CheckStateChanged += new System.EventHandler(this.checkBoxNull_CheckStateChanged);
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
            // QueryGridEnumFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxNull);
            this.Controls.Add(this.checkedListBox1);
            this.MinimumSize = new System.Drawing.Size(0, 49);
            this.Name = "QueryGridEnumFilter";
            this.Size = new System.Drawing.Size(200, 49);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.CheckBox checkBoxNull;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
