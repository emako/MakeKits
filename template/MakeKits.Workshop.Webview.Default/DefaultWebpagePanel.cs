using System.IO;

namespace MakeKits.Workshop.Webview.Default;

public sealed class DefaultWebpagePanel : EmbeddedResourceWebpagePanel
{
    public override string UserDataFolder { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        @"MakeKits\Workshop\Webview2_Data\"
    );
}
