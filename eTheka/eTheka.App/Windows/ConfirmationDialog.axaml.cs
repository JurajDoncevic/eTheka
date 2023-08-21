using Avalonia.Controls;
using eTheka.App.ViewModels;

namespace eTheka.App.Windows;
public partial class ConfirmationDialog : Window
{
    private readonly ConfirmationViewModel _viewModel;
    public ConfirmationViewModel ViewModel => _viewModel;

    public ConfirmationDialog(string title, string message, ConfirmationViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
    }

    public ConfirmationDialog(string title, string message)
        : this(title, message, new ConfirmationViewModel(title, message))
    {

    }

    public ConfirmationDialog()
        : this(string.Empty, string.Empty)
    {

    }

    public bool IsConfirmed => _viewModel.IsConfirmed;
}
