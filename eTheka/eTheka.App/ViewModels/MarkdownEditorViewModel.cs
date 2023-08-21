using AvaloniaEdit.Document;
using eTheka.App.Utils;
using eTheka.Messaging.Notifications;
using eTheka.Messaging.Requests;
using MediatR;
using ReactiveUI;
using System.Threading;
using System.Threading.Tasks;

namespace eTheka.App.ViewModels;
public class MarkdownEditorViewModel
    : ViewModelBase,
    IRequestHandler<GetCurrentMarkdownRequest, string>,
    INotificationHandler<MarkdownFileOpenedNotification>
{
    private string _markdownText = string.Empty;
    public string MarkdownText
    {
        get => _markdownText;
        set => this.RaiseAndSetIfChanged(ref _markdownText, value);
    }

    private readonly IMediator _mediator;

    public MarkdownEditorViewModel()
        : this(Dependencies.GetRequiredService<IMediator>())
    {
    }

    public MarkdownEditorViewModel(IMediator mediator) : base()
    {
        _mediator = mediator;
    }

    public async Task<string> Handle(GetCurrentMarkdownRequest request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(MarkdownText);
    }

    public async Task Handle(MarkdownFileOpenedNotification notification, CancellationToken cancellationToken)
    {
        MarkdownText = notification.MarkdownFile.Markdown;
    }
}
