namespace Aurum.Interface.Win.Filters
{
    partial class ArrayEdit
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
            if (disposing && valueEdit != null)
            {
                Controls.Remove(valueEdit);
                valueEdit.Dispose();
                valueEdit = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArrayEdit));
            this.listValues = new System.Windows.Forms.ListBox();
            this.buttonDownValue = new System.Windows.Forms.Button();
            this.buttonUpValue = new System.Windows.Forms.Button();
            this.buttonDeleteValue = new System.Windows.Forms.Button();
            this.panelValue = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonEditValue = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.buttonAddValue = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip5 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listValues
            // 
            this.listValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listValues.FormattingEnabled = true;
            this.listValues.HorizontalScrollbar = true;
            this.listValues.IntegralHeight = false;
            this.listValues.Location = new System.Drawing.Point(0, 0);
            this.listValues.Name = "listValues";
            this.listValues.Size = new System.Drawing.Size(145, 52);
            this.listValues.TabIndex = 0;
            this.listValues.SelectedIndexChanged += new System.EventHandler(this.listValues_SelectedIndexChanged);
            // 
            // buttonDownValue
            // 
            this.buttonDownValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDownValue.Image = ((System.Drawing.Image)(resources.GetObject("buttonDownValue.Image")));
            this.buttonDownValue.Location = new System.Drawing.Point(-1, 29);
            this.buttonDownValue.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.buttonDownValue.Name = "buttonDownValue";
            this.buttonDownValue.Size = new System.Drawing.Size(26, 23);
            this.buttonDownValue.TabIndex = 1;
            this.buttonDownValue.TabStop = false;
            this.toolTip5.SetToolTip(this.buttonDownValue, "Переместить вниз");
            this.buttonDownValue.UseVisualStyleBackColor = true;
            this.buttonDownValue.Click += new System.EventHandler(this.OnValueDownClick);
            // 
            // buttonUpValue
            // 
            this.buttonUpValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpValue.Image = ((System.Drawing.Image)(resources.GetObject("buttonUpValue.Image")));
            this.buttonUpValue.Location = new System.Drawing.Point(-1, 0);
            this.buttonUpValue.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.buttonUpValue.Name = "buttonUpValue";
            this.buttonUpValue.Size = new System.Drawing.Size(26, 23);
            this.buttonUpValue.TabIndex = 0;
            this.buttonUpValue.TabStop = false;
            this.toolTip4.SetToolTip(this.buttonUpValue, "Переместить вверх");
            this.buttonUpValue.UseVisualStyleBackColor = true;
            this.buttonUpValue.Click += new System.EventHandler(this.OnValueUpClick);
            // 
            // buttonDeleteValue
            // 
            this.buttonDeleteValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeleteValue.Image = ((System.Drawing.Image)(resources.GetObject("buttonDeleteValue.Image")));
            this.buttonDeleteValue.Location = new System.Drawing.Point(27, 0);
            this.buttonDeleteValue.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.buttonDeleteValue.Name = "buttonDeleteValue";
            this.buttonDeleteValue.Size = new System.Drawing.Size(26, 23);
            this.buttonDeleteValue.TabIndex = 3;
            this.buttonDeleteValue.TabStop = false;
            this.toolTip3.SetToolTip(this.buttonDeleteValue, "Удалить значение");
            this.buttonDeleteValue.UseVisualStyleBackColor = true;
            this.buttonDeleteValue.Click += new System.EventHandler(this.OnValueDeleteClick);
            // 
            // panelValue
            // 
            this.panelValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelValue.Location = new System.Drawing.Point(0, 0);
            this.panelValue.Name = "panelValue";
            this.panelValue.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.panelValue.Size = new System.Drawing.Size(145, 24);
            this.panelValue.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer.Panel2MinSize = 24;
            this.splitContainer.Size = new System.Drawing.Size(203, 80);
            this.splitContainer.SplitterDistance = 52;
            this.splitContainer.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listValues);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.buttonUpValue);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDeleteValue);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDownValue);
            this.splitContainer1.Panel2MinSize = 26;
            this.splitContainer1.Size = new System.Drawing.Size(203, 52);
            this.splitContainer1.SplitterDistance = 145;
            this.splitContainer1.TabIndex = 0;
            // 
            // buttonEditValue
            // 
            this.buttonEditValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditValue.Image = ((System.Drawing.Image)(resources.GetObject("buttonEditValue.Image")));
            this.buttonEditValue.Location = new System.Drawing.Point(-1, 0);
            this.buttonEditValue.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.buttonEditValue.Name = "buttonEditValue";
            this.buttonEditValue.Size = new System.Drawing.Size(26, 23);
            this.buttonEditValue.TabIndex = 2;
            this.buttonEditValue.TabStop = false;
            this.toolTip2.SetToolTip(this.buttonEditValue, "Редактировать значение");
            this.buttonEditValue.UseVisualStyleBackColor = true;
            this.buttonEditValue.Click += new System.EventHandler(this.OnValueEditClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelValue);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.buttonAddValue);
            this.splitContainer2.Panel2.Controls.Add(this.buttonEditValue);
            this.splitContainer2.Panel2MinSize = 26;
            this.splitContainer2.Size = new System.Drawing.Size(203, 24);
            this.splitContainer2.SplitterDistance = 145;
            this.splitContainer2.TabIndex = 11;
            // 
            // buttonAddValue
            // 
            this.buttonAddValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddValue.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddValue.Image")));
            this.buttonAddValue.Location = new System.Drawing.Point(27, 0);
            this.buttonAddValue.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.buttonAddValue.Name = "buttonAddValue";
            this.buttonAddValue.Size = new System.Drawing.Size(26, 23);
            this.buttonAddValue.TabIndex = 0;
            this.buttonAddValue.TabStop = false;
            this.toolTip1.SetToolTip(this.buttonAddValue, "Добавить значение");
            this.buttonAddValue.UseVisualStyleBackColor = true;
            this.buttonAddValue.Click += new System.EventHandler(this.OnValueAddClick);
            // 
            // ArrayEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(0, 80);
            this.Name = "ArrayEdit";
            this.Size = new System.Drawing.Size(203, 80);
            this.Leave += new System.EventHandler(this.ArrayEdit_Leave);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listValues;
        private System.Windows.Forms.Button buttonDownValue;
        private System.Windows.Forms.Button buttonUpValue;
        private System.Windows.Forms.Button buttonDeleteValue;
        private System.Windows.Forms.Panel panelValue;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Button buttonEditValue;
        private System.Windows.Forms.Button buttonAddValue;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip4;
        private System.Windows.Forms.ToolTip toolTip5;
    }
}
