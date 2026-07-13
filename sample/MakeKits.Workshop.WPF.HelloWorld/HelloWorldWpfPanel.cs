using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MakeKits.Workshop.WPF.HelloWorld;

public sealed class HelloWorldWpfPanel : UserControl, IDisposable
{
    private readonly TextBlock _messageText;

    public HelloWorldWpfPanel(WorkshopTheme theme)
    {
        bool isDark = theme == WorkshopTheme.Dark
            || (theme == WorkshopTheme.System && IsSystemDarkTheme());

        Background = new SolidColorBrush(isDark ? Color.FromRgb(0x1E, 0x1E, 0x1E) : Color.FromRgb(0xFA, 0xFA, 0xFA));

        _messageText = new TextBlock
        {
            Text = "Hello, World!",
            FontSize = 32,
            FontWeight = FontWeights.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = new SolidColorBrush(isDark ? Colors.White : Color.FromRgb(0x21, 0x21, 0x21)),
        };

        TextBlock subtitle = new()
        {
            Text = "MakeKits Workshop WPF Sample",
            FontSize = 14,
            Margin = new Thickness(0, 12, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = new SolidColorBrush(isDark ? Color.FromRgb(0xB0, 0xB0, 0xB0) : Color.FromRgb(0x61, 0x61, 0x61)),
        };

        Button greetButton = new()
        {
            Content = "Click Me",
            Margin = new Thickness(0, 24, 0, 0),
            Padding = new Thickness(16, 8, 16, 8),
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        greetButton.Click += (_, _) =>
        {
            _messageText.Text = $"Hello, World! ({DateTime.Now:HH:mm:ss})";
        };

        Content = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                _messageText,
                subtitle,
                greetButton,
            },
        };
    }

    public void Dispose()
    {
    }

    private static bool IsSystemDarkTheme()
    {
        try
        {
            using Microsoft.Win32.RegistryKey? key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            object? value = key?.GetValue("AppsUseLightTheme");
            return value is int appsUseLightTheme && appsUseLightTheme == 0;
        }
        catch
        {
            return false;
        }
    }
}
