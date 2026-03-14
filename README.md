# Com.H.Web.UI.WordPasteEditor

![NuGet](https://img.shields.io/nuget/v/Com.H.Web.UI.WordPasteEditor?color=blue)
![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.8.1-purple?logo=dotnet)
![Zero Client Dependencies](https://img.shields.io/badge/Client_Dependencies-0-brightgreen)
![License](https://img.shields.io/badge/License-MIT-green)

An ASP.NET Web Forms server control that provides a rich-text editor with **automatic Microsoft Word paste cleanup**.

Under the hood, this control wraps the [word-paste-editor](https://github.com/H7O/word-paste-editor) client-side library — a zero-dependency JavaScript editor that intercepts paste/drop events and sanitizes Word HTML while preserving meaningful formatting.

## Features

- **Word paste cleanup** — strips Office markup (`mso-*` styles, conditional comments, XML namespaces) while keeping tables, colors, fonts, bold/italic, lists, images, and links.
- **Server control** — drop-in `<wpe:Editor>` tag with full ViewState round-tripping, `IPostBackDataHandler` support, and a `ContentChanged` event.
- **UpdatePanel compatible** — works correctly inside async postback panels; content is flushed before every full or partial postback.
- **Debounced sync** — editor content is synced to a hidden input on a configurable debounce interval (default 300 ms).
- **Zero client dependencies** — no jQuery, no frameworks. All JS/CSS is embedded in the assembly and served via `WebResource.axd`.
- **Read-only mode** — set `ReadOnly="true"` to render a non-editable view.

## Installation

```
Install-Package Com.H.Web.UI.WordPasteEditor
```

or with the .NET CLI:

```
dotnet add package Com.H.Web.UI.WordPasteEditor
```

## Quick Start

### 1. Register the tag prefix

```aspx
<%@ Register TagPrefix="wpe"
    Namespace="Com.H.Web.UI.WordPasteEditor"
    Assembly="Com.H.Web.UI.WordPasteEditor" %>
```

### 2. Add the editor to your page

```aspx
<wpe:Editor ID="MyEditor" runat="server"
    Width="100%" Height="300px" />
```

### 3. Read/write content in code-behind

```csharp
// Get the HTML content
string html = MyEditor.Content;

// Set content programmatically
MyEditor.Content = "<p>Hello, world!</p>";
```

### 4. Handle content changes

```csharp
protected void Page_Init(object sender, EventArgs e)
{
    MyEditor.ContentChanged += MyEditor_ContentChanged;
}

private void MyEditor_ContentChanged(object sender, EventArgs e)
{
    // Content was modified by the user
}
```

## Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Content` | `string` | `""` | HTML content of the editor. |
| `ReadOnly` | `bool` | `false` | Disables editing when `true`. |
| `DebounceInterval` | `int` | `300` | Milliseconds to debounce sync to hidden field. |
| `CssClass` | `string` | `"wpe-editor"` | CSS class on the editor `<div>`. |
| `Width` | `string` | `""` | CSS width (e.g. `"100%"`, `"850px"`). |
| `Height` | `string` | `""` | CSS height (e.g. `"300px"`). |

## How It Works

The control renders:
1. A `contenteditable` `<div>` — the visual editor.
2. A `<input type="hidden">` — carries the HTML value through postbacks.

Three embedded JavaScript modules handle the client-side behavior:

| Module | Role |
|---|---|
| `word-cleaner.js` | Detects and sanitizes Word HTML. |
| `word-paste-editor.js` | Contenteditable editor with paste/drop interception. |
| `wpe-bootstrap.js` | Glue between the server control and the client libraries. |

These scripts and a default stylesheet (`wpe-editor.css`) are embedded in the assembly and delivered automatically via `WebResource.axd`.

## Upstream

The client-side JavaScript is based on the [word-paste-editor](https://github.com/H7O/word-paste-editor) library.

## License

[MIT](LICENSE) © Hussein Al Bayati
