using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;

[assembly: TagPrefix("Com.H.Web.UI.WordPasteEditor", "wpe")]

[assembly: WebResource("Com.H.Web.UI.WordPasteEditor.Scripts.word-cleaner.js", "application/javascript")]
[assembly: WebResource("Com.H.Web.UI.WordPasteEditor.Scripts.word-paste-editor.js", "application/javascript")]
[assembly: WebResource("Com.H.Web.UI.WordPasteEditor.Scripts.wpe-bootstrap.js", "application/javascript")]
[assembly: WebResource("Com.H.Web.UI.WordPasteEditor.Styles.wpe-editor.css", "text/css")]

namespace Com.H.Web.UI.WordPasteEditor
{
    /// <summary>
    /// A rich-text editor server control that wraps the
    /// <see href="https://github.com/H7O/word-paste-editor">word-paste-editor</see>
    /// client library.  Renders a <c>contenteditable</c> div backed by a
    /// hidden input whose value round-trips through ViewState.
    /// Content is debounce-synced on every keystroke and flushed before
    /// every postback (full and async / UpdatePanel).
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;%@ Register TagPrefix="wpe" Namespace="Com.H.Web.UI.WordPasteEditor"
    ///     Assembly="Com.H.Web.UI.WordPasteEditor" %&gt;
    ///
    /// &lt;wpe:Editor ID="MyEditor" runat="server"
    ///     Width="100%" Height="300px" /&gt;
    /// </code>
    /// </example>
    [DefaultProperty("Content")]
    [ToolboxData("<{0}:Editor runat=server></{0}:Editor>")]
    public class Editor : Control, IPostBackDataHandler
    {
        private const string VS_Content = "C";
        private const string VS_ReadOnly = "RO";
        private const string VS_DebounceInterval = "DI";
        private const string VS_CssClass = "CC";
        private const string VS_Width = "W";
        private const string VS_Height = "H";

        /// <summary>HTML content of the editor.</summary>
        [Bindable(true), Category("Data"), DefaultValue("")]
        public string Content
        {
            get => (string)ViewState[VS_Content] ?? string.Empty;
            set => ViewState[VS_Content] = value;
        }

        /// <summary>When <c>true</c> the editor is not editable.</summary>
        [Category("Behavior"), DefaultValue(false)]
        public bool ReadOnly
        {
            get => (bool)(ViewState[VS_ReadOnly] ?? false);
            set => ViewState[VS_ReadOnly] = value;
        }

        /// <summary>
        /// Milliseconds to debounce the editor-to-hidden-field sync.
        /// Default is 300.
        /// </summary>
        [Category("Behavior"), DefaultValue(300)]
        public int DebounceInterval
        {
            get => (int)(ViewState[VS_DebounceInterval] ?? 300);
            set => ViewState[VS_DebounceInterval] = value;
        }

        /// <summary>
        /// CSS class applied to the editor div.  Default is <c>"wpe-editor"</c>.
        /// </summary>
        [Category("Appearance"), DefaultValue("wpe-editor")]
        public string CssClass
        {
            get => (string)ViewState[VS_CssClass] ?? "wpe-editor";
            set => ViewState[VS_CssClass] = value;
        }

        /// <summary>
        /// Width of the editor div (CSS value, e.g. <c>"850px"</c> or <c>"100%"</c>).
        /// </summary>
        [Category("Appearance"), DefaultValue("")]
        public string Width
        {
            get => (string)ViewState[VS_Width] ?? string.Empty;
            set => ViewState[VS_Width] = value;
        }

        /// <summary>
        /// Height of the editor div (CSS value, e.g. <c>"200px"</c>).
        /// </summary>
        [Category("Appearance"), DefaultValue("")]
        public string Height
        {
            get => (string)ViewState[VS_Height] ?? string.Empty;
            set => ViewState[VS_Height] = value;
        }

        private string EditorClientId => ClientID + "_editor";
        private string HiddenFieldClientId => ClientID + "_hf";

        /// <summary>
        /// Raised when the posted content differs from the ViewState value.
        /// </summary>
        [Category("Action")]
        public event EventHandler ContentChanged;

        /// <summary>Raises the <see cref="ContentChanged"/> event.</summary>
        protected virtual void OnContentChanged(EventArgs e)
            => ContentChanged?.Invoke(this, e);

        /// <inheritdoc />
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RegisterResources();
        }

        /// <inheritdoc />
        protected override void Render(HtmlTextWriter writer)
        {
            if (!Visible) return;

            // contenteditable div
            writer.AddAttribute(HtmlTextWriterAttribute.Id, EditorClientId);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);

            if (!string.IsNullOrEmpty(Width))
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width);
            if (!string.IsNullOrEmpty(Height))
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height);

            if (ReadOnly)
                writer.AddAttribute("data-readonly", "true");
            else
                writer.AddAttribute("contenteditable", "true");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

            // hidden input (name = UniqueID for IPostBackDataHandler routing)
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, HiddenFieldClientId);
            writer.AddAttribute(HtmlTextWriterAttribute.Value, Content);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        /// <inheritdoc />
        public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            var posted = postCollection[postDataKey];
            if (posted != null && posted != Content)
            {
                Content = posted;
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public void RaisePostDataChangedEvent()
            => OnContentChanged(EventArgs.Empty);

        private void RegisterResources()
        {
            var type = typeof(Editor);
            var cs = Page.ClientScript;
            var sm = ScriptManager.GetCurrent(Page);

            // CSS (page-level, once)
            if (!cs.IsClientScriptBlockRegistered(type, "wpe-css"))
            {
                var cssUrl = cs.GetWebResourceUrl(type,
                    "Com.H.Web.UI.WordPasteEditor.Styles.wpe-editor.css");

                cs.RegisterClientScriptBlock(type, "wpe-css",
                    $"<link rel=\"stylesheet\" href=\"{HttpUtility.HtmlAttributeEncode(cssUrl)}\" />\n",
                    addScriptTags: false);
            }

            // JS libraries (page-level, once)
            cs.RegisterClientScriptResource(type,
                "Com.H.Web.UI.WordPasteEditor.Scripts.word-cleaner.js");
            cs.RegisterClientScriptResource(type,
                "Com.H.Web.UI.WordPasteEditor.Scripts.word-paste-editor.js");
            cs.RegisterClientScriptResource(type,
                "Com.H.Web.UI.WordPasteEditor.Scripts.wpe-bootstrap.js");

            // Init call (re-runs after async postback when ScriptManager is present)
            var initScript =
                $"H7.WpeInit('{EditorClientId}','{HiddenFieldClientId}'," +
                $"{{debounce:{DebounceInterval},readOnly:{(ReadOnly ? "true" : "false")}}});";

            if (sm != null)
                ScriptManager.RegisterStartupScript(
                    this, type, "wpe-init-" + ClientID, initScript, addScriptTags: true);
            else
                cs.RegisterStartupScript(
                    type, "wpe-init-" + ClientID, initScript, addScriptTags: true);
        }
    }
}
