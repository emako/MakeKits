using Microsoft.Terminal.Wpf;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Terminal;
using Color = System.Windows.Media.Color;

namespace MakeKits.Workshop.Console.Default;

public partial class DefaultHostPanel : UserControl, IDisposable
{
    private DataBinds Binds { get; set; } = new();

    public DefaultHostPanel()
    {
        InitializeComponent();
        DataContext = Binds;
    }

    public DefaultHostPanel(string commandLine)
    {
        InitializeComponent();
        Binds.StartupCommand = commandLine;
        DataContext = Binds;
    }

    public virtual void Dispose()
    {
        basicTermControl?.DisconnectConPTYTerm();
    }
}

public class DataBinds : INotifyPropertyChanged
{
    public void TriggerPropChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

    public string StartupCommand { get; set; } = "powershell.exe";

    public string WorkingDirectory => Path.Combine(Path.GetTempPath(), "MakeKits", "Caches", Configuration.Id);

    private static readonly Color BackroundColor = Color.FromArgb(0, 0, 0, 0);

    public event PropertyChangedEventHandler? PropertyChanged;

    public SolidColorBrush BackroundColorBrush => new(BackroundColor);

    public TerminalTheme Theme { get; set; } = new()
    {
        DefaultBackground = ExtendedTerminalControl.ColorToVal(BackroundColor),
        DefaultForeground = ExtendedTerminalControl.ColorToVal(Colors.White),
        DefaultSelectionBackground = 0xcccccc,
        CursorStyle = CursorStyle.BlinkingBar,
        ColorTable = [0x0C0C0C, 0x1F0FC5, 0x0EA113, 0x009CC1, 0xDA3700, 0x981788, 0xDD963A, 0xCCCCCC, 0x767676, 0x5648E7, 0x0CC616, 0xA5F1F9, 0xFF783B, 0x9E00B4, 0xD6D661, 0xF2F2F2],
    };
}
