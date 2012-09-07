using System;
using onlyconnect;
using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage.Controls {
    public partial class RichTextEditorControl : UserControl {
        private const string ReadyStateComplete = "complete";

        private string readyState;
        private string textToLoad;

        public RichTextEditorControl() {
            InitializeComponent();

            ctlHtmlEditor.Enabled = true;
            ctlHtmlEditor.ReadyStateChanged += ctlHtmlEditor_ReadyStateChanged;

            btnBold.Click += btnBold_Click;
            btnItalic.Click += btnItalic_Click;
            btnUnderline.Click += btnUnderline_Click;
            btnStrikethrough.Click += btnStrikethrough_Click;
            btnRemoveFormat.Click += btnRemoveFormat_Click;
            btnBulletedList.Click += btnBulletedList_Click;
            btnNumberedList.Click += btnNumberedList_Click;
            btnIncreaseIndent.Click += btnIncreaseIndent_Click;
            btnDecreaseIndent.Click += btnDecreaseIndent_Click;
            btnJustifyCenter.Click += btnJustifyCenter_Click;
            btnJustifyLeft.Click += btnJustifyLeft_Click;
            btnJustifyRight.Click += btnJustifyRight_Click;
            btnJustifyFull.Click += btnJustifyFull_Click;
            btnUndo.Click += btnUndo_Click;
            btnRedo.Click += btnRedo_Click;
        }

        public string HtmlData {
            get {
                try {
                    return ctlHtmlEditor.HtmlDocument2.GetBody().innerHTML;
                } catch {
                    return string.Empty;
                }
            }
            set {
                if (readyState == ReadyStateComplete) {
                    ctlHtmlEditor.HtmlDocument2.GetBody().innerHTML = value;
                } else {
                    textToLoad = value;
                }
            }
        }

        #region Event handlers

        // NOTE state lifecycle is loading <-> interactive -> complete
        private void ctlHtmlEditor_ReadyStateChanged(object sender, ReadyStateChangedEventArgs e) {
            readyState = e.ReadyState;
            
            if (readyState == ReadyStateComplete) {
                ctlHtmlEditor.HtmlDocument2.GetBody().style.SetFontFamily("Verdana,Arial,Helvetica,sans-serif");               
                ctlHtmlEditor.HtmlDocument2.GetBody().style.SetFontSize("10px");               
                
                if (textToLoad != null) {
                    ctlHtmlEditor.HtmlDocument2.GetBody().innerHTML = textToLoad;
                    textToLoad = null;
                }
            }
        }

        private void btnRedo_Click(object sender, EventArgs e) {
            ctlHtmlEditor.Redo();
        }

        private void btnUndo_Click(object sender, EventArgs e) {
            ctlHtmlEditor.Undo();
        }

        private void btnJustifyFull_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SetSelectionJustifyFull();
        }

        private void btnJustifyRight_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void btnJustifyLeft_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void btnJustifyCenter_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void btnDecreaseIndent_Click(object sender, EventArgs e) {
            ctlHtmlEditor.Outdent();
        }

        private void btnIncreaseIndent_Click(object sender, EventArgs e) {
            ctlHtmlEditor.Indent();
        }

        private void btnBulletedList_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SetUnorderedList();
        }

        private void btnNumberedList_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SetOrderedList();
        }

        private void btnRemoveFormat_Click(object sender, EventArgs e) {
            ctlHtmlEditor.ClearSelectionFormatting();
        }

        private void btnStrikethrough_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SetSelectionStrikethrough();
        }

        private void btnUnderline_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SetSelectionUnderline();
        }

        private void btnItalic_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SetSelectionItalic();
        }

        private void btnBold_Click(object sender, EventArgs e) {
            ctlHtmlEditor.SetSelectionBold();
        }

        #endregion
    }
}