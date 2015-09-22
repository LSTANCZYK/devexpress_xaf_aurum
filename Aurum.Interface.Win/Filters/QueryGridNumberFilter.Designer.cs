namespace Aurum.Interface.Win.Filters
{
    abstract partial class QueryGridNumberFilter<T>
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxNull = new System.Windows.Forms.CheckBox();
            this.decimalEdit1 = new DecimalEdit();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.decimalEdit3 = new DecimalEdit();
            this.decimalEdit2 = new DecimalEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.arrayEdit1 = new ArrayEdit();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(314, 114);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.TabStop = false;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxNull);
            this.tabPage1.Controls.Add(this.decimalEdit1);
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(306, 88);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Простой фильтр";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxNull
            // 
            this.checkBoxNull.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxNull.AutoSize = true;
            this.checkBoxNull.Location = new System.Drawing.Point(288, 11);
            this.checkBoxNull.Name = "checkBoxNull";
            this.checkBoxNull.Size = new System.Drawing.Size(15, 14);
            this.checkBoxNull.TabIndex = 3;
            this.checkBoxNull.TabStop = false;
            this.checkBoxNull.ThreeState = true;
            this.toolTip1.SetToolTip(this.checkBoxNull, "Поиск пустого/непустого значения.\r\nЕсли галочка:\r\n1. Не определена - поиск по умо" +
        "лчанию\r\n2. Установлена - поиск непустого значения\r\n3. Не установлена - поиск пус" +
        "того значения");
            this.checkBoxNull.UseVisualStyleBackColor = true;
            this.checkBoxNull.CheckStateChanged += new System.EventHandler(this.checkBoxNull_CheckStateChanged);
            // 
            // decimalEdit1
            // 
            this.decimalEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.decimalEdit1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.decimalEdit1.Location = new System.Drawing.Point(52, 7);
            this.decimalEdit1.Name = "decimalEdit1";
            this.decimalEdit1.Size = new System.Drawing.Size(230, 20);
            this.decimalEdit1.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(8, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(38, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.decimalEdit3);
            this.tabPage2.Controls.Add(this.decimalEdit2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.comboBox3);
            this.tabPage2.Controls.Add(this.comboBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(306, 88);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Диапазон";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // decimalEdit3
            // 
            this.decimalEdit3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.decimalEdit3.Location = new System.Drawing.Point(207, 7);
            this.decimalEdit3.Name = "decimalEdit3";
            this.decimalEdit3.Size = new System.Drawing.Size(96, 20);
            this.decimalEdit3.TabIndex = 6;
            // 
            // decimalEdit2
            // 
            this.decimalEdit2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.decimalEdit2.Location = new System.Drawing.Point(3, 7);
            this.decimalEdit2.Name = "decimalEdit2";
            this.decimalEdit2.Size = new System.Drawing.Size(96, 20);
            this.decimalEdit2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(146, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "X";
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(166, 6);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(35, 21);
            this.comboBox3.TabIndex = 3;
            this.comboBox3.TabStop = false;
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(105, 6);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(35, 21);
            this.comboBox2.TabIndex = 2;
            this.comboBox2.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.comboBox4);
            this.tabPage3.Controls.Add(this.arrayEdit1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(306, 88);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Множественный выбор";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // comboBox4
            // 
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(5, 9);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(39, 21);
            this.comboBox4.TabIndex = 1;
            this.comboBox4.Visible = false;
            // 
            // arrayEdit1
            // 
            this.arrayEdit1.IsNullable = true;
            this.arrayEdit1.ItemToText = null;
            this.arrayEdit1.Location = new System.Drawing.Point(8, 5);
            this.arrayEdit1.MinimumSize = new System.Drawing.Size(0, 80);
            this.arrayEdit1.Name = "arrayEdit1";
            this.arrayEdit1.Size = new System.Drawing.Size(292, 80);
            this.arrayEdit1.TabIndex = 0;
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
            // QueryGridNumberFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "QueryGridNumberFilter";
            this.Size = new System.Drawing.Size(312, 114);
            this.Enter += new System.EventHandler(this.QueryGridNumberFilter_Enter);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private DecimalEdit decimalEdit1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private DecimalEdit decimalEdit3;
        private DecimalEdit decimalEdit2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.CheckBox checkBoxNull;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox comboBox4;
        private ArrayEdit arrayEdit1;
    }
}
