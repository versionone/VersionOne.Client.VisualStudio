namespace VersionOne.VisualStudio.VSPackage.Controls
{
    partial class RichTextEditorControl
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
            this.ctlHtmlEditor = new onlyconnect.HtmlEditor();
            this.tsToolMenu = new System.Windows.Forms.ToolStrip();
            this.btnBold = new System.Windows.Forms.ToolStripButton();
            this.btnItalic = new System.Windows.Forms.ToolStripButton();
            this.btnUnderline = new System.Windows.Forms.ToolStripButton();
            this.btnStrikethrough = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveFormat = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnJustifyLeft = new System.Windows.Forms.ToolStripButton();
            this.btnJustifyRight = new System.Windows.Forms.ToolStripButton();
            this.btnJustifyCenter = new System.Windows.Forms.ToolStripButton();
            this.btnJustifyFull = new System.Windows.Forms.ToolStripButton();
            this.btnBulletedList = new System.Windows.Forms.ToolStripButton();
            this.btnNumberedList = new System.Windows.Forms.ToolStripButton();
            this.btnIncreaseIndent = new System.Windows.Forms.ToolStripButton();
            this.btnDecreaseIndent = new System.Windows.Forms.ToolStripButton();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.tsToolMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctlHtmlEditor
            // 
            this.ctlHtmlEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ctlHtmlEditor.DefaultComposeSettings.BackColor = System.Drawing.Color.White;
            this.ctlHtmlEditor.DefaultComposeSettings.DefaultFont = new System.Drawing.Font("Arial", 10F);
            this.ctlHtmlEditor.DefaultComposeSettings.Enabled = false;
            this.ctlHtmlEditor.DefaultComposeSettings.ForeColor = System.Drawing.Color.Black;
            this.ctlHtmlEditor.DefaultPreamble = onlyconnect.EncodingType.UTF8;
            this.ctlHtmlEditor.DocumentEncoding = onlyconnect.EncodingType.WindowsCurrent;
            this.ctlHtmlEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctlHtmlEditor.IsDesignMode = true;
            this.ctlHtmlEditor.Location = new System.Drawing.Point(0, 28);
            this.ctlHtmlEditor.Name = "ctlHtmlEditor";
            this.ctlHtmlEditor.SelectionAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.ctlHtmlEditor.SelectionBackColor = System.Drawing.Color.Empty;
            this.ctlHtmlEditor.SelectionBullets = false;
            this.ctlHtmlEditor.SelectionFont = null;
            this.ctlHtmlEditor.SelectionForeColor = System.Drawing.Color.Empty;
            this.ctlHtmlEditor.SelectionNumbering = false;
            this.ctlHtmlEditor.Size = new System.Drawing.Size(459, 147);
            this.ctlHtmlEditor.TabIndex = 1;
            // 
            // tsToolMenu
            // 
            this.tsToolMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsToolMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBold,
            this.btnItalic,
            this.btnUnderline,
            this.btnStrikethrough,
            this.btnRemoveFormat,
            this.toolStripSeparator1,
            this.btnJustifyLeft,
            this.btnJustifyRight,
            this.btnJustifyCenter,
            this.btnJustifyFull,
            this.btnBulletedList,
            this.btnNumberedList,
            this.btnIncreaseIndent,
            this.btnDecreaseIndent,
            this.btnUndo,
            this.btnRedo});
            this.tsToolMenu.Location = new System.Drawing.Point(0, 0);
            this.tsToolMenu.Name = "tsToolMenu";
            this.tsToolMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsToolMenu.Size = new System.Drawing.Size(459, 28);
            this.tsToolMenu.TabIndex = 2;
            this.tsToolMenu.Text = "toolStrip1";
            // 
            // btnBold
            // 
            this.btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBold.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Bold;
            this.btnBold.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBold.Name = "btnBold";
            this.btnBold.Size = new System.Drawing.Size(25, 25);
            this.btnBold.Text = "Bold";
            // 
            // btnItalic
            // 
            this.btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnItalic.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Italic;
            this.btnItalic.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(25, 25);
            this.btnItalic.Text = "Italic";
            // 
            // btnUnderline
            // 
            this.btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUnderline.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Underline;
            this.btnUnderline.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.Size = new System.Drawing.Size(25, 25);
            this.btnUnderline.Text = "Underline";
            // 
            // btnStrikethrough
            // 
            this.btnStrikethrough.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnStrikethrough.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Strikethrough;
            this.btnStrikethrough.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnStrikethrough.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStrikethrough.Name = "btnStrikethrough";
            this.btnStrikethrough.Size = new System.Drawing.Size(25, 25);
            this.btnStrikethrough.Text = "Strikethrough";
            // 
            // btnRemoveFormat
            // 
            this.btnRemoveFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemoveFormat.Image = global::VersionOne.VisualStudio.VSPackage.Resources.RemoveFormat;
            this.btnRemoveFormat.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRemoveFormat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveFormat.Name = "btnRemoveFormat";
            this.btnRemoveFormat.Size = new System.Drawing.Size(25, 25);
            this.btnRemoveFormat.Text = "Remove format";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // btnJustifyLeft
            // 
            this.btnJustifyLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnJustifyLeft.Image = global::VersionOne.VisualStudio.VSPackage.Resources.JustifyLeft;
            this.btnJustifyLeft.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnJustifyLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJustifyLeft.Name = "btnJustifyLeft";
            this.btnJustifyLeft.Size = new System.Drawing.Size(25, 25);
            this.btnJustifyLeft.Text = "Justify left";
            // 
            // btnJustifyRight
            // 
            this.btnJustifyRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnJustifyRight.Image = global::VersionOne.VisualStudio.VSPackage.Resources.JustifyRight;
            this.btnJustifyRight.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnJustifyRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJustifyRight.Name = "btnJustifyRight";
            this.btnJustifyRight.Size = new System.Drawing.Size(25, 25);
            this.btnJustifyRight.Text = "Justify right";
            // 
            // btnJustifyCenter
            // 
            this.btnJustifyCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnJustifyCenter.Image = global::VersionOne.VisualStudio.VSPackage.Resources.JustifyCenter;
            this.btnJustifyCenter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnJustifyCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJustifyCenter.Name = "btnJustifyCenter";
            this.btnJustifyCenter.Size = new System.Drawing.Size(25, 25);
            this.btnJustifyCenter.Text = "Justify center";
            // 
            // btnJustifyFull
            // 
            this.btnJustifyFull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnJustifyFull.Image = global::VersionOne.VisualStudio.VSPackage.Resources.JustifyFull;
            this.btnJustifyFull.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnJustifyFull.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnJustifyFull.Name = "btnJustifyFull";
            this.btnJustifyFull.Size = new System.Drawing.Size(25, 25);
            this.btnJustifyFull.Text = "Justify full";
            // 
            // btnBulletedList
            // 
            this.btnBulletedList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBulletedList.Image = global::VersionOne.VisualStudio.VSPackage.Resources.BulletedList;
            this.btnBulletedList.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnBulletedList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBulletedList.Name = "btnBulletedList";
            this.btnBulletedList.Size = new System.Drawing.Size(25, 25);
            this.btnBulletedList.Text = "Bulleted list";
            // 
            // btnNumberedList
            // 
            this.btnNumberedList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNumberedList.Image = global::VersionOne.VisualStudio.VSPackage.Resources.NumberedList;
            this.btnNumberedList.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnNumberedList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNumberedList.Name = "btnNumberedList";
            this.btnNumberedList.Size = new System.Drawing.Size(25, 25);
            this.btnNumberedList.Text = "Numbered list";
            // 
            // btnIncreaseIndent
            // 
            this.btnIncreaseIndent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnIncreaseIndent.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Indent;
            this.btnIncreaseIndent.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnIncreaseIndent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIncreaseIndent.Name = "btnIncreaseIndent";
            this.btnIncreaseIndent.Size = new System.Drawing.Size(25, 25);
            this.btnIncreaseIndent.Text = "Increase indent";
            // 
            // btnDecreaseIndent
            // 
            this.btnDecreaseIndent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDecreaseIndent.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Outdent;
            this.btnDecreaseIndent.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnDecreaseIndent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDecreaseIndent.Name = "btnDecreaseIndent";
            this.btnDecreaseIndent.Size = new System.Drawing.Size(25, 25);
            this.btnDecreaseIndent.Text = "Decrease indent";
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Undo;
            this.btnUndo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(25, 25);
            this.btnUndo.Text = "Undo";
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedo.Image = global::VersionOne.VisualStudio.VSPackage.Resources.Redo;
            this.btnRedo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(25, 25);
            this.btnRedo.Text = "Redo";
            // 
            // RichTextEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tsToolMenu);
            this.Controls.Add(this.ctlHtmlEditor);
            this.Name = "RichTextEditorControl";
            this.Size = new System.Drawing.Size(459, 175);
            this.tsToolMenu.ResumeLayout(false);
            this.tsToolMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private onlyconnect.HtmlEditor ctlHtmlEditor;
        private System.Windows.Forms.ToolStrip tsToolMenu;
        private System.Windows.Forms.ToolStripButton btnBold;
        private System.Windows.Forms.ToolStripButton btnItalic;
        private System.Windows.Forms.ToolStripButton btnUnderline;
        private System.Windows.Forms.ToolStripButton btnRemoveFormat;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnBulletedList;
        private System.Windows.Forms.ToolStripButton btnStrikethrough;
        private System.Windows.Forms.ToolStripButton btnNumberedList;
        private System.Windows.Forms.ToolStripButton btnIncreaseIndent;
        private System.Windows.Forms.ToolStripButton btnDecreaseIndent;
        private System.Windows.Forms.ToolStripButton btnJustifyCenter;
        private System.Windows.Forms.ToolStripButton btnJustifyLeft;
        private System.Windows.Forms.ToolStripButton btnJustifyRight;
        private System.Windows.Forms.ToolStripButton btnJustifyFull;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
    }
}
