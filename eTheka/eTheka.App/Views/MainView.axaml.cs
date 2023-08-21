using Avalonia.Controls;
using eTheka.App.Utils;
using eTheka.App.ViewModels;

namespace eTheka.App.Views;
public partial class MainView : UserControl
{
    private readonly MainViewModel _viewModel;
    public MainView()
        : this(Dependencies.GetRequiredService<MainViewModel>())
    {

    }

    public MainView(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
    }
}
