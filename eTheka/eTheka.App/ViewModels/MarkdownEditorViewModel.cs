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
    private string _markdownText =
        "# heading 1\r\n## heading 2 \r\n### heading 3\r\n**bold text**\r\n\r\nsome ordinary text\r\n\r\nSome bullet points:\r\n* bullet 1\r\n* bullet 2\r\n* bullet 3";
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
