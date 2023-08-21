using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using eTheka.App.Utils;
using eTheka.Domain;
using eTheka.Messaging.Notifications;
using eTheka.Messaging.Requests;
using eTheka.Services;
using MediatR;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using eTheka.App.Windows;
using eTheka.App.Messaging.Notifications;

namespace eTheka.App.ViewModels;
public class FiguresManagementViewModel
    : ViewModelBase,
    IRequestHandler<GetCurrentFiguresRequest, IEnumerable<Figure>>,
    INotificationHandler<MarkdownFileOpenedNotification>
{
    private AvaloniaList<Figure> _figures = new AvaloniaList<Figure>();
    public AvaloniaList<Figure> Figures { get => _figures; set => this.RaiseAndSetIfChanged(ref _figures, value); }

    private Figure? _selectedFigure;
    public Figure? SelectedFigure
    {
        get => _selectedFigure;
        set => this.RaiseAndSetIfChanged(ref _selectedFigure, value);
    }

    private readonly IMediator _mediator;
    private readonly FileService _fileService;

    public FiguresManagementViewModel()
        : this(Dependencies.GetRequiredService<IMediator>(),
               Dependencies.GetRequiredService<FileService>())
    {

    }

    public FiguresManagementViewModel(IMediator mediator, FileService fileService)
    {
        _mediator = mediator;
        _fileService = fileService;
    }

    public async Task<IEnumerable<Figure>> Handle(GetCurrentFiguresRequest request, CancellationToken cancellationToken)
    {
        return Figures.ToList();
    }

    public async Task Handle(MarkdownFileOpenedNotification notification, CancellationToken cancellationToken)
    {
        Figures.Clear();
        Figures.AddRange(notification.MarkdownFile.Figures);
    }

    public void OpenDroppedImagesCommand(IDataObject data)
    {
        data.GetFiles()?
            .Select(file => _fileService.OpenImageFile(file.Path))
            .ToList()
            .ForEach(result =>
            {
                if (result)
                {
                    var imageFile = result.Data;
                    if (Figures.Count(fig => fig.FullName.Equals(imageFile.FullName)) == 0)
                    {
                        Figures.Add(new Figure(imageFile.Name, imageFile.Extension, imageFile.FileBytes));
                        _mediator.Publish(new OperationOutcomeNotification(Base.ResultTypes.SUCCESS, $"Figure '{imageFile.FullName}' loaded"));
                    }
                    else
                    {
                        _mediator.Publish(new OperationOutcomeNotification(Base.ResultTypes.FAILURE, $"Figure '{imageFile.FullName}' already loaded"));
                    }
                }
                else
                {
                    _mediator.Publish(new OperationOutcomeNotification(Base.ResultTypes.FAILURE, result.Message));
                }
            });
    }

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> OpenImagesCommand
        => ReactiveCommand.CreateFromTask(async () =>
        {
            if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                var filePickerOption = new FilePickerOpenOptions()
                {
                    AllowMultiple = true,
                    Title = "Open image file",
                    FileTypeFilter = new FilePickerFileType[] { SupportedFileTypes.IMAGE_FILES }
                };
                var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(filePickerOption);
                if (storageFiles.Any())
                {
                    var results = storageFiles.Select(storageFile => _fileService.OpenImageFile(storageFile.Path));
                    foreach (var result in results.Where(_ => _))
                    {
                        var imageFile = result.Data;
                        if (Figures.Count(fig => fig.FullName.Equals(imageFile.FullName)) == 0)
                        {
                            Figures.Add(new Figure(imageFile.Name, imageFile.Extension, imageFile.FileBytes));
                            await _mediator.Publish(new OperationOutcomeNotification(Base.ResultTypes.SUCCESS, $"Figure '{imageFile.FullName}' loaded"));
                        }
                        else
                        {
                            await _mediator.Publish(new OperationOutcomeNotification(Base.ResultTypes.FAILURE, $"Figure '{imageFile.FullName}' already loaded"));
                        }
                    }
                }

            }
        });

    public ReactiveCommand<Figure, System.Reactive.Unit> RemoveFigureCommand
        => ReactiveCommand.Create<Figure>(
            async (selectedFigure) =>
            {
                var dialog = new ConfirmationDialog("Remove figure?", $"Do you really wish to remove figure {selectedFigure.FullName}?");

                if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    && desktop.MainWindow is not null)
                {
                    await dialog.ShowDialog(desktop.MainWindow);
                    if (dialog.IsConfirmed)
                    {
                        Figures.Remove(selectedFigure);
                        await _mediator.Publish(new OperationOutcomeNotification(Base.ResultTypes.SUCCESS, $"Figure '{selectedFigure.FullName}' removed"));
                    }
                }


            });

}
