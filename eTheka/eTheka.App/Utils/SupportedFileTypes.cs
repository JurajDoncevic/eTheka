using Avalonia.Platform.Storage;

namespace eTheka.App.Utils;
public static class SupportedFileTypes
{
    public readonly static FilePickerFileType IMAGE_FILES =
        new("All Images")
        {
            Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.webp" },
            AppleUniformTypeIdentifiers = new[] { "public.image" },
            MimeTypes = new[] { "image/*" }
        };

    public readonly static FilePickerFileType MARKDOWN_EXTENDED_FILE =
        new("Markdown extended file")
        {
            Patterns = new[] { "*.mdex" },
            AppleUniformTypeIdentifiers = new[] { "public.content" },
            MimeTypes = new[] { "application/zip" }
        };

    public readonly static FilePickerFileType HTML_FILE =
    new("HTML file")
    {
        Patterns = new[] { "*.html" },
        AppleUniformTypeIdentifiers = new[] { "public.html" },
        MimeTypes = new[] { "text/html" }
    };
}
