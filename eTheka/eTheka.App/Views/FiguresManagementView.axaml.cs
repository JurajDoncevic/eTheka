using Avalonia.Controls;
using Avalonia.Input;
using eTheka.App.Utils;
using eTheka.App.ViewModels;

namespace eTheka.App.Views;
public partial class FiguresManagementView : UserControl
{
    private readonly FiguresManagementViewModel _viewModel;

    public FiguresManagementView()
        : this(Dependencies.GetRequiredService<FiguresManagementViewModel>())
    {

    }

    public FiguresManagementView(FiguresManagementViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        AddHandler(DragDrop.DropEvent, FiguresDropped);
        InitializeComponent();
    }

    async void FiguresDropped(object? sender, DragEventArgs e)
    {
        _viewModel.OpenDroppedImagesCommand(e.Data);
    }
}
