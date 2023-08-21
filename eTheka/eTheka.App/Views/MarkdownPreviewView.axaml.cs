using Avalonia.Controls;
using eTheka.App.Utils;
using eTheka.App.ViewModels;

namespace eTheka.App.Views;
public partial class MarkdownPreviewView : UserControl
{
    private readonly MarkdownPreviewViewModel _viewModel;

    public MarkdownPreviewView()
        : this(Dependencies.GetRequiredService<MarkdownPreviewViewModel>())
    {

    }

    public MarkdownPreviewView(MarkdownPreviewViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
    }
}
