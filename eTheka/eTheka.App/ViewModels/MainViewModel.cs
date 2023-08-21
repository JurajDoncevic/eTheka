using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using eTheka.App.Messaging.Notifications;
using eTheka.App.Utils;
using eTheka.Base;
using MediatR;
using ReactiveUI;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using eTheka.Services;
using eTheka.Messaging.Notifications;
using eTheka.Messaging.Requests;
using eTheka.Domain;
using eTheka.App.Messaging.Requests;
using eTheka.App.Windows;

namespace eTheka.App.ViewModels;

public class MainViewModel
    : ViewModelBase,
      INotificationHandler<OperationOutcomeNotification>
{
    private bool _isFiguresSidebarOpen = false;
    public bool IsFiguresSidebarOpen { get => _isFiguresSidebarOpen; set => this.RaiseAndSetIfChanged(ref _isFiguresSidebarOpen, value); }

    private string? _operationOutcomeMessage;
    public string? OperationOutcomeMessage
    {
        get => _operationOutcomeMessage;
        set => this.RaiseAndSetIfChanged(ref _operationOutcomeMessage, value);
    }

    private ResultTypes? _operationOutcomeType;
    public ResultTypes? OperationOutcomeType
    {
        get => _operationOutcomeType;
        set => this.RaiseAndSetIfChanged(ref _operationOutcomeType, value);
    }

    private readonly IMediator _mediator;
    private readonly FileService _fileService;
    private readonly MarkdownTranslationService _translationService;

    private Option<MarkdownExtendedFile> _markdownExtendedFile;

    public MainViewModel()
        : this(Dependencies.GetRequiredService<IMediator>(),
               Dependencies.GetRequiredService<FileService>(),
               Dependencies.GetRequiredService<MarkdownTranslationService>())
    {
    }

    public MainViewModel(IMediator mediator, FileService fileService, MarkdownTranslationService translationService)
    {
        _mediator = mediator;
        _fileService = fileService;
        _markdownExtendedFile = Option.None<MarkdownExtendedFile>();
        _translationService = translationService;
    }

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ToggleFiguresSidebarCommand
        => ReactiveCommand.Create((System.Action)(() => IsFiguresSidebarOpen = !IsFiguresSidebarOpen));

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> OpenMarkdownExtendedFileCommand
        => ReactiveCommand.CreateFromTask(async () =>
        {
            if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                var filePickerOption = new FilePickerOpenOptions()
                {
                    AllowMultiple = false,
                    Title = "Open markdown extended file",
                    FileTypeFilter = new FilePickerFileType[] { SupportedFileTypes.MARKDOWN_EXTENDED_FILE }
                };
                var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(filePickerOption);
                if (storageFiles.Any())
                {
                    var filePath = storageFiles.First().Path;

                    var openFileResult = _fileService.OpenMarkdownExtendedFile(filePath);
                    if (openFileResult)
                    {
                        var markdownFile = openFileResult.Data;
                        _markdownExtendedFile = Option.Some(markdownFile);
                        await _mediator.Publish(new MarkdownFileOpenedNotification(markdownFile));
                        await _mediator.Publish(new OperationOutcomeNotification(openFileResult.ResultType, openFileResult.Message));
                    }
                }
            }
        });

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveAsMarkdownExtendedFileCommand
        => ReactiveCommand.CreateFromTask(async () =>
        {
            if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                var filePickerOption = new FilePickerSaveOptions()
                {
                    DefaultExtension = "mdex",
                    ShowOverwritePrompt = true,
                    Title = "Save markdown extended file",
                    FileTypeChoices = new FilePickerFileType[] { SupportedFileTypes.MARKDOWN_EXTENDED_FILE }
                };
                var storageFile = await topLevel.StorageProvider.SaveFilePickerAsync(filePickerOption);
                if (storageFile != null)
                {
                    var markdownText = _mediator.Send(new GetCurrentMarkdownRequest());
                    var figures = _mediator.Send(new GetCurrentFiguresRequest());

                    var markdownFile = new Domain.MarkdownExtendedFile(
                        storageFile.Name.Replace(".mdex", string.Empty),
                        storageFile.Path,
                        await markdownText,
                        await figures
                        );

                    var markdownFileSavingResult =
                        _fileService.SaveMarkdownExtendedFile(
                            storageFile.Path,
                            markdownFile
                            );

                    if (markdownFileSavingResult)
                    {
                        _markdownExtendedFile = Option.Some(markdownFile);
                    }
                    await _mediator.Publish(new OperationOutcomeNotification(markdownFileSavingResult.ResultType, markdownFileSavingResult.Message));

                }

            }
        });

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SaveMarkdownExtendedFileCommand
        => ReactiveCommand.CreateFromTask(async () =>
        {
            if (_markdownExtendedFile)
            {
                var markdownText = await _mediator.Send(new GetCurrentMarkdownRequest());
                var figures = await _mediator.Send(new GetCurrentFiguresRequest());

                var markdownFile = new Domain.MarkdownExtendedFile(_markdownExtendedFile.Value.FileName.Replace(".mdex", string.Empty), _markdownExtendedFile.Value.FilePath, markdownText, figures);

                var markdownFileSavingResult =
                    _fileService.SaveMarkdownExtendedFile(
                        markdownFile.FilePath,
                        markdownFile
                        );

                if (markdownFileSavingResult)
                {
                    _markdownExtendedFile = Option.Some(markdownFile);
                    await _mediator.Publish(new OperationOutcomeNotification(markdownFileSavingResult.ResultType, markdownFileSavingResult.Message));
                }
            }
            else
            {
                SaveAsMarkdownExtendedFileCommand.Execute();
            }
        });

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ExportAsHtmlCommand
        => ReactiveCommand.CreateFromTask(async () =>
        {
            if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                var filePickerOption = new FilePickerSaveOptions()
                {
                    DefaultExtension = "html",
                    ShowOverwritePrompt = true,
                    Title = "Save HTML file",
                    FileTypeChoices = new FilePickerFileType[] { SupportedFileTypes.HTML_FILE }
                };
                var storageFile = await topLevel.StorageProvider.SaveFilePickerAsync(filePickerOption);

                if (storageFile != null)
                {
                    var markdownText = await _mediator.Send(new GetCurrentMarkdownRequest());
                    var figures = await _mediator.Send(new GetCurrentFiguresRequest());

                    var translation = _translationService.TranslateToHtml(markdownText, figures);
                    if (translation)
                    {
                        var htmlSaving = _fileService.SaveHtmlFile(storageFile.Path, translation.Data);

                        await _mediator.Publish(new OperationOutcomeNotification(htmlSaving.ResultType, htmlSaving.Message));
                    }
                    else
                    {
                        await _mediator.Publish(new OperationOutcomeNotification(translation.ResultType, translation.Message));
                    }
                }
            }
        });

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GenerateHtmlPreviewCommand
        => ReactiveCommand.CreateFromTask(async () =>
        {
            await _mediator.Send(new GenerateHtmlPreviewRequest());
        });

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> OpenAboutCommand
        => ReactiveCommand.Create(() =>
        {
            if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                new AboutWindow().ShowDialog(desktop.MainWindow);
            }
        });

    public void HandleClose()
    {
        if (Avalonia.Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow!.Close();
        }
    }

    public async Task Handle(OperationOutcomeNotification notification, CancellationToken cancellationToken)
    {
        OperationOutcomeType = notification.ResultType;
        OperationOutcomeMessage = notification.Message;
    }
}
