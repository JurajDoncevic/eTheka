using eTheka.Domain;
using MediatR;

namespace eTheka.Messaging.Notifications;
public class MarkdownFileOpenedNotification : INotification
{
    private readonly MarkdownExtendedFile _markdownFile;

    public MarkdownFileOpenedNotification(MarkdownExtendedFile markdownFile)
    {
        _markdownFile = markdownFile;
    }

    internal MarkdownExtendedFile MarkdownFile => _markdownFile;
}
