namespace VersionOne.VisualStudio.VSPackage.Controls {
    partial class OptionsPageControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblMinLogLevel = new System.Windows.Forms.Label();
            this.cboMinLogLevel = new System.Windows.Forms.ComboBox();
            this.lblUrl = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.lblSampleUrl = new System.Windows.Forms.Label();
            this.chkIntegrated = new System.Windows.Forms.CheckBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkUseProxy = new System.Windows.Forms.CheckBox();
            this.grpProxySettings = new System.Windows.Forms.GroupBox();
            this.lblProxyUrl = new System.Windows.Forms.Label();
            this.txtProxyUrl = new System.Windows.Forms.TextBox();
            this.lblProxyUsername = new System.Windows.Forms.Label();
            this.txtProxyUsername = new System.Windows.Forms.TextBox();
            this.lblProxyPassword = new System.Windows.Forms.Label();
            this.txtProxyPassword = new System.Windows.Forms.TextBox();
            this.lblProxyDomain = new System.Windows.Forms.Label();
            this.txtProxyDomain = new System.Windows.Forms.TextBox();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.pnlContent.SuspendLayout();
            this.grpProxySettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlContent.AutoScroll = true;
            this.pnlContent.Controls.Add(this.lblUrl);
            this.pnlContent.Controls.Add(this.txtUrl);
            this.pnlContent.Controls.Add(this.lblSampleUrl);
            this.pnlContent.Controls.Add(this.chkIntegrated);
            this.pnlContent.Controls.Add(this.lblUser);
            this.pnlContent.Controls.Add(this.txtUserName);
            this.pnlContent.Controls.Add(this.lblPassword);
            this.pnlContent.Controls.Add(this.txtPassword);
            this.pnlContent.Controls.Add(this.chkUseProxy);
            this.pnlContent.Controls.Add(this.grpProxySettings);
            this.pnlContent.Controls.Add(this.btnTestConnection);
            this.pnlContent.Controls.Add(this.lblMinLogLevel);
            this.pnlContent.Controls.Add(this.cboMinLogLevel);
            this.pnlContent.Location = new System.Drawing.Point(0, 0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(339, 329);
            this.pnlContent.TabIndex = 0;
            // 
            // lblMinLogLevel
            // 
            this.lblMinLogLevel.AutoSize = true;
            this.lblMinLogLevel.Location = new System.Drawing.Point(10, 303);
            this.lblMinLogLevel.Name = "lblMinLogLevel";
            this.lblMinLogLevel.Size = new System.Drawing.Size(166, 13);
            this.lblMinLogLevel.TabIndex = 11;
            this.lblMinLogLevel.Text = "Minimum severity of log messages";
            // 
            // cboMinLogLevel
            // 
            this.cboMinLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboMinLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMinLogLevel.FormattingEnabled = true;
            this.cboMinLogLevel.Location = new System.Drawing.Point(180, 300);
            this.cboMinLogLevel.Name = "cboMinLogLevel";
            this.cboMinLogLevel.Size = new System.Drawing.Size(140, 21);
            this.cboMinLogLevel.TabIndex = 12;
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Location = new System.Drawing.Point(12, 12);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(87, 13);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "Application URL:";
            // 
            // txtUrl
            // 
            this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUrl.Location = new System.Drawing.Point(108, 12);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(212, 20);
            this.txtUrl.TabIndex = 1;
            // 
            // lblSampleUrl
            // 
            this.lblSampleUrl.AutoSize = true;
            this.lblSampleUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblSampleUrl.Location = new System.Drawing.Point(115, 34);
            this.lblSampleUrl.Name = "lblSampleUrl";
            this.lblSampleUrl.Size = new System.Drawing.Size(180, 12);
            this.lblSampleUrl.TabIndex = 2;
            this.lblSampleUrl.Text = "For example,  http://myserver/VersionOne";
            // 
            // chkIntegrated
            // 
            this.chkIntegrated.AutoSize = true;
            this.chkIntegrated.Location = new System.Drawing.Point(108, 51);
            this.chkIntegrated.Name = "chkIntegrated";
            this.chkIntegrated.Size = new System.Drawing.Size(192, 17);
            this.chkIntegrated.TabIndex = 3;
            this.chkIntegrated.Text = "Windows Integrated Authentication";
            this.chkIntegrated.UseVisualStyleBackColor = true;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(12, 75);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(58, 13);
            this.lblUser.TabIndex = 4;
            this.lblUser.Text = "Username:";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(108, 72);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(212, 20);
            this.txtUserName.TabIndex = 5;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 99);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(108, 96);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(212, 20);
            this.txtPassword.TabIndex = 7;
            // 
            // chkUseProxy
            // 
            this.chkUseProxy.AutoSize = true;
            this.chkUseProxy.Location = new System.Drawing.Point(107, 122);
            this.chkUseProxy.Name = "chkUseProxy";
            this.chkUseProxy.Size = new System.Drawing.Size(202, 17);
            this.chkUseProxy.TabIndex = 8;
            this.chkUseProxy.Text = "Use proxy for VersionOne connection";
            this.chkUseProxy.UseVisualStyleBackColor = true;
            // 
            // grpProxySettings
            // 
            this.grpProxySettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProxySettings.Controls.Add(this.lblProxyUrl);
            this.grpProxySettings.Controls.Add(this.txtProxyUrl);
            this.grpProxySettings.Controls.Add(this.lblProxyUsername);
            this.grpProxySettings.Controls.Add(this.txtProxyUsername);
            this.grpProxySettings.Controls.Add(this.lblProxyPassword);
            this.grpProxySettings.Controls.Add(this.txtProxyPassword);
            this.grpProxySettings.Controls.Add(this.lblProxyDomain);
            this.grpProxySettings.Controls.Add(this.txtProxyDomain);
            this.grpProxySettings.Location = new System.Drawing.Point(5, 140);
            this.grpProxySettings.Name = "grpProxySettings";
            this.grpProxySettings.Size = new System.Drawing.Size(325, 117);
            this.grpProxySettings.TabIndex = 9;
            this.grpProxySettings.TabStop = false;
            this.grpProxySettings.Text = "Proxy settings";
            // 
            // lblProxyUrl
            // 
            this.lblProxyUrl.AutoSize = true;
            this.lblProxyUrl.Location = new System.Drawing.Point(6, 22);
            this.lblProxyUrl.Name = "lblProxyUrl";
            this.lblProxyUrl.Size = new System.Drawing.Size(93, 13);
            this.lblProxyUrl.TabIndex = 0;
            this.lblProxyUrl.Text = "Proxy server URL:";
            // 
            // txtProxyUrl
            // 
            this.txtProxyUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProxyUrl.Location = new System.Drawing.Point(121, 19);
            this.txtProxyUrl.Name = "txtProxyUrl";
            this.txtProxyUrl.Size = new System.Drawing.Size(194, 20);
            this.txtProxyUrl.TabIndex = 1;
            // 
            // lblProxyUsername
            // 
            this.lblProxyUsername.AutoSize = true;
            this.lblProxyUsername.Location = new System.Drawing.Point(6, 46);
            this.lblProxyUsername.Name = "lblProxyUsername";
            this.lblProxyUsername.Size = new System.Drawing.Size(58, 13);
            this.lblProxyUsername.TabIndex = 2;
            this.lblProxyUsername.Text = "Username:";
            // 
            // txtProxyUsername
            // 
            this.txtProxyUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProxyUsername.Location = new System.Drawing.Point(121, 43);
            this.txtProxyUsername.Name = "txtProxyUsername";
            this.txtProxyUsername.Size = new System.Drawing.Size(194, 20);
            this.txtProxyUsername.TabIndex = 3;
            // 
            // lblProxyPassword
            // 
            this.lblProxyPassword.AutoSize = true;
            this.lblProxyPassword.Location = new System.Drawing.Point(8, 70);
            this.lblProxyPassword.Name = "lblProxyPassword";
            this.lblProxyPassword.Size = new System.Drawing.Size(56, 13);
            this.lblProxyPassword.TabIndex = 4;
            this.lblProxyPassword.Text = "Password:";
            // 
            // txtProxyPassword
            // 
            this.txtProxyPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProxyPassword.Location = new System.Drawing.Point(121, 67);
            this.txtProxyPassword.Name = "txtProxyPassword";
            this.txtProxyPassword.PasswordChar = '*';
            this.txtProxyPassword.Size = new System.Drawing.Size(194, 20);
            this.txtProxyPassword.TabIndex = 5;
            // 
            // lblProxyDomain
            // 
            this.lblProxyDomain.AutoSize = true;
            this.lblProxyDomain.Location = new System.Drawing.Point(8, 94);
            this.lblProxyDomain.Name = "lblProxyDomain";
            this.lblProxyDomain.Size = new System.Drawing.Size(46, 13);
            this.lblProxyDomain.TabIndex = 6;
            this.lblProxyDomain.Text = "Domain:";
            // 
            // txtProxyDomain
            // 
            this.txtProxyDomain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProxyDomain.Location = new System.Drawing.Point(121, 91);
            this.txtProxyDomain.Name = "txtProxyDomain";
            this.txtProxyDomain.Size = new System.Drawing.Size(194, 20);
            this.txtProxyDomain.TabIndex = 7;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestConnection.Location = new System.Drawing.Point(205, 261);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(115, 24);
            this.btnTestConnection.TabIndex = 10;
            this.btnTestConnection.Text = "&Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            // 
            // OptionsPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlContent);
            this.Name = "OptionsPageControl";
            this.Size = new System.Drawing.Size(339, 329);
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.grpProxySettings.ResumeLayout(false);
            this.grpProxySettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.CheckBox chkUseProxy;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.ComboBox cboMinLogLevel;
        private System.Windows.Forms.CheckBox chkIntegrated;
        private System.Windows.Forms.GroupBox grpProxySettings;
        private System.Windows.Forms.Label lblProxyUrl;
        private System.Windows.Forms.TextBox txtProxyUrl;
        private System.Windows.Forms.Label lblProxyUsername;
        private System.Windows.Forms.TextBox txtProxyUsername;
        private System.Windows.Forms.Label lblProxyPassword;
        private System.Windows.Forms.TextBox txtProxyPassword;
        private System.Windows.Forms.Label lblProxyDomain;
        private System.Windows.Forms.TextBox txtProxyDomain;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label lblSampleUrl;
        private System.Windows.Forms.Label lblMinLogLevel;

    }
}