using System;
using System.Windows.Forms;

namespace VersionOne.VisualStudio.VSPackage.Forms {
    public partial class RichTextEditorDialog : Form {
        private string htmlText;

        public RichTextEditorDialog() {
            InitializeComponent();
            btnOK.Click += GetHtml;
        }

        private void GetHtml(object sender, EventArgs e) {
            htmlText = ctlRichTextEditor.HtmlData;
        }

        public string HtmlData {
            get { return htmlText; }
            set { ctlRichTextEditor.HtmlData = value; }
        }
    }
}