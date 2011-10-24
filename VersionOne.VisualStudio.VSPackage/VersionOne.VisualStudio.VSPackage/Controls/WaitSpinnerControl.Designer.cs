namespace VersionOne.VisualStudio.VSPackage.Controls {
    partial class WaitSpinnerControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lblWait = new System.Windows.Forms.Label();
            this.pbSpinner = new System.Windows.Forms.PictureBox();
            this.pnlSpinner = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).BeginInit();
            this.pnlSpinner.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWait
            // 
            this.lblWait.AutoSize = true;
            this.lblWait.Location = new System.Drawing.Point(54, 13);
            this.lblWait.Name = "lblWait";
            this.lblWait.Size = new System.Drawing.Size(174, 13);
            this.lblWait.TabIndex = 0;
            this.lblWait.Text = "Please wait, operation in progress...";
            // 
            // pbSpinner
            // 
            this.pbSpinner.Location = new System.Drawing.Point(3, 3);
            this.pbSpinner.Name = "pbSpinner";
            this.pbSpinner.Size = new System.Drawing.Size(32, 32);
            this.pbSpinner.TabIndex = 1;
            this.pbSpinner.TabStop = false;
            this.pbSpinner.Image = Resources.SpinnerImage;
            // 
            // pnlSpinner
            // 
            this.pnlSpinner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSpinner.Controls.Add(this.lblWait);
            this.pnlSpinner.Controls.Add(this.pbSpinner);
            this.pnlSpinner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSpinner.Location = new System.Drawing.Point(0, 0);
            this.pnlSpinner.Name = "pnlSpinner";
            this.pnlSpinner.Size = new System.Drawing.Size(246, 40);
            this.pnlSpinner.TabIndex = 2;
            // 
            // WaitSpinnerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlSpinner);
            this.Name = "WaitSpinnerControl";
            this.Size = new System.Drawing.Size(246, 40);
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).EndInit();
            this.pnlSpinner.ResumeLayout(false);
            this.pnlSpinner.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblWait;
        private System.Windows.Forms.PictureBox pbSpinner;
        private System.Windows.Forms.Panel pnlSpinner;
    }
}
