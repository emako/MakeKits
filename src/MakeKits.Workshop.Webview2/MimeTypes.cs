namespace MakeKits.Workshop.Webview2;

public static class MimeTypes
{
    public const string Html = "text/html";
    public const string JavaScript = "application/javascript";
    public const string Css = "text/css";
    public const string Json = "application/json";
    public const string Xml = "application/xml";
    public const string Svg = "image/svg+xml";
    public const string Png = "image/png";
    public const string Jpeg = "image/jpeg";
    public const string Gif = "image/gif";
    public const string Webp = "image/webp";
    public const string Ico = "image/x-icon";
    public const string Avif = "image/avif";
    public const string Woff = "font/woff";
    public const string Woff2 = "font/woff2";
    public const string Ttf = "font/ttf";
    public const string Otf = "font/otf";
    public const string Mp3 = "audio/mpeg";
    public const string Mp4 = "video/mp4";
    public const string Webm = "video/webm";
    public const string Pdf = "application/pdf";
    public const string Binary = "application/octet-stream";
    public const string Text = "text/plain";

    public static string GetContentType(string? extension = null) => $"Content-Type: {GetMimeType(extension)}";

    public static string GetMimeType(string? extension = null) => extension?.ToLowerInvariant() switch
    {
        ".html" or ".htm" => Html,
        ".js" => JavaScript,
        ".css" => Css,
        ".json" => Json,
        ".xml" => Xml,
        ".png" => Png,
        ".jpg" or ".jpeg" => Jpeg,
        ".gif" => Gif,
        ".webp" => Webp,
        ".svg" => Svg,
        ".ico" => Ico,
        ".avif" => Avif,
        ".woff" => Woff,
        ".woff2" => Woff2,
        ".ttf" => Ttf,
        ".otf" => Otf,
        ".mp3" => Mp3,
        ".mp4" => Mp4,
        ".webm" => Webm,
        ".pdf" => Pdf,
        ".txt" => Text,
        ".zip" => "application/zip",
        ".gz" => "application/gzip",
        ".rar" => "application/vnd.rar",
        ".7z" => "application/x-7z-compressed",
        _ => Binary,
    };
}
