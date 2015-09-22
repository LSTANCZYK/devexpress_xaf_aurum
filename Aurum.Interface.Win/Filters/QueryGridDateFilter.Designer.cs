namespace Aurum.Interface.Win.Filters
{
    partial class QueryGridDateFilter
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
            this.dateTimeEdit1 = new DateTimeEdit();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dateTimeEdit3 = new DateTimeEdit();
            this.dateTimeEdit2 = new DateTimeEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.comboBoxOperators2 = new System.Windows.Forms.ComboBox();
            this.editShiftDay = new IntegerEdit();
            this.editShiftMonth = new IntegerEdit();
            this.editShiftYear = new IntegerEdit();
            this.comboBoxCondition = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.exactDateTime = new DateTimeEdit();
            this.operatorsComboBox = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(314, 60);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.TabStop = false;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxNull);
            this.tabPage1.Controls.Add(this.dateTimeEdit1);
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(306, 34);
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
            this.checkBoxNull.TabIndex = 2;
            this.checkBoxNull.TabStop = false;
            this.checkBoxNull.ThreeState = true;
            this.toolTip1.SetToolTip(this.checkBoxNull, "Поиск пустого/непустого значения.\r\nЕсли галочка:\r\n1. Не определена - поиск по умо" +
        "лчанию\r\n2. Установлена - поиск непустого значения\r\n3. Не установлена - поиск пус" +
        "того значения");
            this.checkBoxNull.UseVisualStyleBackColor = true;
            this.checkBoxNull.CheckStateChanged += new System.EventHandler(this.checkBoxNull_CheckStateChanged);
            // 
            // dateTimeEdit1
            // 
            this.dateTimeEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimeEdit1.Checked = false;
            this.dateTimeEdit1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEdit1.Location = new System.Drawing.Point(52, 7);
            this.dateTimeEdit1.Name = "dateTimeEdit1";
            this.dateTimeEdit1.ShowCheckBox = true;
            this.dateTimeEdit1.Size = new System.Drawing.Size(230, 20);
            this.dateTimeEdit1.TabIndex = 1;
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
            this.tabPage2.Controls.Add(this.dateTimeEdit3);
            this.tabPage2.Controls.Add(this.dateTimeEdit2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.comboBox3);
            this.tabPage2.Controls.Add(this.comboBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(306, 34);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Диапазон";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dateTimeEdit3
            // 
            this.dateTimeEdit3.Checked = false;
            this.dateTimeEdit3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEdit3.Location = new System.Drawing.Point(207, 7);
            this.dateTimeEdit3.Name = "dateTimeEdit3";
            this.dateTimeEdit3.ShowCheckBox = true;
            this.dateTimeEdit3.Size = new System.Drawing.Size(96, 20);
            this.dateTimeEdit3.TabIndex = 6;
            // 
            // dateTimeEdit2
            // 
            this.dateTimeEdit2.Checked = false;
            this.dateTimeEdit2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeEdit2.Location = new System.Drawing.Point(3, 7);
            this.dateTimeEdit2.Name = "dateTimeEdit2";
            this.dateTimeEdit2.ShowCheckBox = true;
            this.dateTimeEdit2.Size = new System.Drawing.Size(96, 20);
            this.dateTimeEdit2.TabIndex = 5;
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
            this.tabPage3.Controls.Add(this.comboBoxOperators2);
            this.tabPage3.Controls.Add(this.editShiftDay);
            this.tabPage3.Controls.Add(this.editShiftMonth);
            this.tabPage3.Controls.Add(this.editShiftYear);
            this.tabPage3.Controls.Add(this.comboBoxCondition);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(306, 34);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Условие";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // comboBoxOperators2
            // 
            this.comboBoxOperators2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperators2.FormattingEnabled = true;
            this.comboBoxOperators2.Location = new System.Drawing.Point(8, 6);
            this.comboBoxOperators2.Name = "comboBoxOperators2";
            this.comboBoxOperators2.Size = new System.Drawing.Size(38, 21);
            this.comboBoxOperators2.TabIndex = 4;
            this.comboBoxOperators2.TabStop = false;
            // 
            // editShiftDay
            // 
            this.editShiftDay.Digits = 0;
            this.editShiftDay.ForeColor = System.Drawing.SystemColors.GrayText;
            this.editShiftDay.Hint = "Дни";
            this.editShiftDay.Location = new System.Drawing.Point(274, 7);
            this.editShiftDay.Name = "editShiftDay";
            this.editShiftDay.Size = new System.Drawing.Size(27, 20);
            this.editShiftDay.TabIndex = 3;
            // 
            // editShiftMonth
            // 
            this.editShiftMonth.Digits = 0;
            this.editShiftMonth.ForeColor = System.Drawing.SystemColors.GrayText;
            this.editShiftMonth.Hint = "Месяцы";
            this.editShiftMonth.Location = new System.Drawing.Point(223, 7);
            this.editShiftMonth.Name = "editShiftMonth";
            this.editShiftMonth.Size = new System.Drawing.Size(47, 20);
            this.editShiftMonth.TabIndex = 2;
            // 
            // editShiftYear
            // 
            this.editShiftYear.Digits = 0;
            this.editShiftYear.ForeColor = System.Drawing.SystemColors.GrayText;
            this.editShiftYear.Hint = "Года";
            this.editShiftYear.Location = new System.Drawing.Point(189, 7);
            this.editShiftYear.Name = "editShiftYear";
            this.editShiftYear.Size = new System.Drawing.Size(30, 20);
            this.editShiftYear.TabIndex = 1;
            // 
            // comboBoxCondition
            // 
            this.comboBoxCondition.FormattingEnabled = true;
            this.comboBoxCondition.Location = new System.Drawing.Point(52, 6);
            this.comboBoxCondition.Name = "comboBoxCondition";
            this.comboBoxCondition.Size = new System.Drawing.Size(134, 21);
            this.comboBoxCondition.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.exactDateTime);
            this.tabPage4.Controls.Add(this.operatorsComboBox);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(306, 34);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Точный фильтр";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // exactDateTime
            // 
            this.exactDateTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exactDateTime.Checked = false;
            this.exactDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.exactDateTime.Location = new System.Drawing.Point(50, 8);
            this.exactDateTime.Name = "exactDateTime";
            this.exactDateTime.ShowCheckBox = true;
            this.exactDateTime.Size = new System.Drawing.Size(250, 20);
            this.exactDateTime.TabIndex = 3;
            // 
            // operatorsComboBox
            // 
            this.operatorsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.operatorsComboBox.FormattingEnabled = true;
            this.operatorsComboBox.Location = new System.Drawing.Point(6, 7);
            this.operatorsComboBox.Name = "operatorsComboBox";
            this.operatorsComboBox.Size = new System.Drawing.Size(38, 21);
            this.operatorsComboBox.TabIndex = 2;
            this.operatorsComboBox.TabStop = false;
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
            // QueryGridDateFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "QueryGridDateFilter";
            this.Size = new System.Drawing.Size(312, 58);
            this.Enter += new System.EventHandler(this.QueryGridDateFilter_Enter);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private DateTimeEdit dateTimeEdit1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private DateTimeEdit dateTimeEdit3;
        private DateTimeEdit dateTimeEdit2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.CheckBox checkBoxNull;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox comboBoxCondition;
        private IntegerEdit editShiftDay;
        private IntegerEdit editShiftMonth;
        private IntegerEdit editShiftYear;
        private System.Windows.Forms.ComboBox comboBoxOperators2;
        private System.Windows.Forms.TabPage tabPage4;
        private DateTimeEdit exactDateTime;
        private System.Windows.Forms.ComboBox operatorsComboBox;
    }
}
