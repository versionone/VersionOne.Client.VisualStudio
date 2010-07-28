namespace VersionOne.VisualStudio.VSPackage.Controls {
    public partial class ErrorMessageControl {
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lnkOptions = new System.Windows.Forms.LinkLabel();
            this.txtError = new System.Windows.Forms.TextBox();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // linkLabelOptions
            // 
            this.lnkOptions.Location = new System.Drawing.Point(8, 8);
            this.lnkOptions.Name = "linkLabelOptions";
            this.lnkOptions.Size = new System.Drawing.Size(160, 24);
            this.lnkOptions.TabIndex = 0;
            this.lnkOptions.TabStop = true;
            this.lnkOptions.Text = "Click here to configure settings";
            // 
            // textBoxError
            // 
            this.txtError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                              | System.Windows.Forms.AnchorStyles.Left)
                                                                             | System.Windows.Forms.AnchorStyles.Right)));
            this.txtError.Location = new System.Drawing.Point(8, 56);
            this.txtError.Multiline = true;
            this.txtError.Name = "textBoxError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(152, 80);
            this.txtError.TabIndex = 1;
            this.txtError.Text = "";
            // 
            // label1
            // 
            this.lblErrorMessage.Location = new System.Drawing.Point(8, 32);
            this.lblErrorMessage.Name = "label1";
            this.lblErrorMessage.TabIndex = 2;
            this.lblErrorMessage.Text = "Error Message:";
            // 
            // ErrorMessageControl
            // 
            this.Controls.Add(this.lblErrorMessage);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.lnkOptions);
            this.Name = "ErrorMessageControl";
            this.Size = new System.Drawing.Size(168, 144);
            this.ResumeLayout(false);

        }

        #endregion
    }
}