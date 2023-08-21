using eTheka.App.Windows;
using ReactiveUI;
using System.Reactive;

namespace eTheka.App.ViewModels;
public class ConfirmationViewModel : ViewModelBase
{
    private readonly string _title;
    private readonly string _message;
    private bool _isConfirmed = false;

    public ConfirmationViewModel()
        : this("No title given", "?")
    {

    }
    public ConfirmationViewModel(string title = "No title given", string message = "?")
    {
        _title = title;
        _message = message;
    }

    public string Title => _title;

    public string Message => _message;

    public bool IsConfirmed => _isConfirmed;

    public ReactiveCommand<ConfirmationDialog, Unit> PressedYesCommand
        => ReactiveCommand.Create<ConfirmationDialog>((dialog) => { _isConfirmed = true; dialog.Close(); });

    public ReactiveCommand<ConfirmationDialog, Unit> PressedNoCommand
        => ReactiveCommand.Create<ConfirmationDialog>((dialog) => { _isConfirmed = false; dialog.Close(); });
}