namespace Aurum.Interface.Win.Filters
{
    partial class FilterWindowControl
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
            this.verticalLayoutPanel1 = new Aurum.Interface.Win.Filters.VerticalLayoutPanel();
            this.SuspendLayout();
            // 
            // verticalLayoutPanel1
            // 
            this.verticalLayoutPanel1.AutoScroll = true;
            this.verticalLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verticalLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.verticalLayoutPanel1.Name = "verticalLayoutPanel1";
            this.verticalLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.verticalLayoutPanel1.TabIndex = 0;
            // 
            // FilterWindowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.verticalLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FilterWindowControl";
            this.ResumeLayout(false);

        }

        #endregion

        private VerticalLayoutPanel verticalLayoutPanel1;
    }
}
