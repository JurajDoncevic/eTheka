using Avalonia.Media;
using ReactiveUI;

namespace eTheka.App.Models;
public class CssStyle : ReactiveObject
{
    private readonly string _name;
    private readonly string _cssStylesheet;
    private readonly IBrush _backgroundBrush;

    public CssStyle(string name, string cssStylesheet, IBrush backgroundBrush)
    {
        _name = name;
        _cssStylesheet = cssStylesheet;
        _backgroundBrush = backgroundBrush;
    }

    public string Name => _name;

    public string CssStylesheet => _cssStylesheet;

    public IBrush BackgroundBrush => _backgroundBrush;
}
