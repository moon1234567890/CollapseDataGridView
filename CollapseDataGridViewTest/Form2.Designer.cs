namespace CollapseDataGridViewTest
{
    partial class Form2
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cdgv_abs = new CollapseDataGridViewTest.CollDataGridView(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cdgv_abs)).BeginInit();
            this.SuspendLayout();
            // 
            // cdgv_abs
            // 
            this.cdgv_abs.AllowUserToAddRows = false;
            this.cdgv_abs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cdgv_abs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cdgv_abs.ImageHeight = 15;
            this.cdgv_abs.ImageWidth = 15;
            this.cdgv_abs.ImgCollapse = global::CollapseDataGridViewTest.Properties.Resources.Collapse;
            this.cdgv_abs.ImgExpand = global::CollapseDataGridViewTest.Properties.Resources.Expand;
            this.cdgv_abs.Location = new System.Drawing.Point(31, 42);
            this.cdgv_abs.Name = "cdgv_abs";
            this.cdgv_abs.RowTemplate.Height = 24;
            this.cdgv_abs.Size = new System.Drawing.Size(639, 320);
            this.cdgv_abs.TabIndex = 0;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 389);
            this.Controls.Add(this.cdgv_abs);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.cdgv_abs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CollDataGridView cdgv_abs;
    }
}