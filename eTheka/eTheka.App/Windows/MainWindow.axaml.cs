using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using System.Threading;

namespace eTheka.App.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closing += MainWindow_Closing;
    }


    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        using (var source = new CancellationTokenSource())
        {
            var dialog = new ConfirmationDialog("Exit eTheka?", "Do you wish to exit eTheka?");

            dialog.ShowDialog(this)
                .ContinueWith(t =>
                {
                    if(dialog.IsConfirmed)
                    {
                        source.Cancel();
                    }
                },
            TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
        }

    }
}
