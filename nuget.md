# Com.H.Web.UI.WordPasteEditor

An ASP.NET Web Forms server control that provides a rich-text editor with **automatic Microsoft Word paste cleanup**.

This control wraps the [word-paste-editor](https://github.com/H7O/word-paste-editor) client-side library — a zero-dependency JavaScript editor that sanitizes Word HTML while preserving meaningful formatting.

## Features

- **Word paste cleanup** — strips Office markup while keeping tables, colors, fonts, bold/italic, lists, images, and links.
- **Server control** — drop-in `<wpe:Editor>` tag with ViewState, `IPostBackDataHandler`, and `ContentChanged` event.
- **UpdatePanel compatible** — works inside async postback panels.
- **Debounced sync** — configurable debounce interval (default 300 ms).
- **Zero client dependencies** — all JS/CSS is embedded in the assembly.
- **Read-only mode** — `ReadOnly="true"` for non-editable rendering.

## Quick Start

Register the tag prefix:

```aspx
<%@ Register TagPrefix="wpe"
    Namespace="Com.H.Web.UI.WordPasteEditor"
    Assembly="Com.H.Web.UI.WordPasteEditor" %>
```

Add the editor:

```aspx
<wpe:Editor ID="MyEditor" runat="server"
    Width="100%" Height="300px" />
```

Read/write content in code-behind:

```csharp
string html = MyEditor.Content;
MyEditor.Content = "<p>Hello, world!</p>";
```

## Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Content` | `string` | `""` | HTML content of the editor. |
| `ReadOnly` | `bool` | `false` | Disables editing when `true`. |
| `DebounceInterval` | `int` | `300` | Debounce interval in milliseconds. |
| `CssClass` | `string` | `"wpe-editor"` | CSS class on the editor div. |
| `Width` | `string` | `""` | CSS width (e.g. `"100%"`). |
| `Height` | `string` | `""` | CSS height (e.g. `"300px"`). |

## Documentation

Full documentation and source code: [GitHub](https://github.com/H7O/Com.H.Web.UI.WordPasteEditor)

Upstream JS library: [word-paste-editor](https://github.com/H7O/word-paste-editor)
