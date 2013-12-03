using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Media;
using VersionOne.VisualStudio.VSPackage.Forms;

namespace VersionOne.VisualStudio.VSPackage.PropertyEditors
{
    public class RichTextTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (editorService == null)
            {
                return base.EditValue(context, provider, value);
            }

            string valueWithHtmlTags = ((VSPackage.Descriptors.WorkitemPropertyDescriptor)context.PropertyDescriptor).GetDescriptionRow();
            var modalEditor = new RichTextEditorDialog
            {
                HtmlData = valueWithHtmlTags,
            };

            if (editorService.ShowDialog(modalEditor) == DialogResult.OK)
            {
                return modalEditor.HtmlData != "<P>&nbsp;</P>" ? modalEditor.HtmlData : "<br>";
            }

            return value;
        }    
    }
}