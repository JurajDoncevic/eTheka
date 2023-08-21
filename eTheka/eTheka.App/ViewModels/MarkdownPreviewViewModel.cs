using Avalonia.Collections;
using Avalonia.Media;
using eTheka.App.Messaging.Notifications;
using eTheka.App.Messaging.Requests;
using eTheka.App.Models;
using eTheka.App.Utils;
using eTheka.Base;
using eTheka.Messaging.Requests;
using eTheka.Services;
using MediatR;
using ReactiveUI;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace eTheka.App.ViewModels;
public class MarkdownPreviewViewModel
    : ViewModelBase,
      IRequestHandler<GenerateHtmlPreviewRequest>
{
    private string _generatedHtml = string.Empty;
    public string GeneratedHtml
    {
        get => _generatedHtml;
        set => this.RaiseAndSetIfChanged(ref _generatedHtml, value);
    }

    private AvaloniaList<CssStyle> _cssStyles = new()
    {
        new CssStyle("Light", "" , Brushes.White),
        new CssStyle("Dark", "/* Reset some default styling */\r\nbody, h1, h2, h3, p {\r\n  margin: 0;\r\n  padding: 0;\r\n}\r\n\r\n/* Set background color and font */\r\nbody {\r\n  background-color: #1a1a1a;\r\n  color: #ffffff;\r\n  font-family: 'Montserrat', sans-serif;\r\n}\r\n\r\n/* Set black background for the entire HTML */\r\nhtml {\r\n  background-color: #000000;\r\n  color: #ffffff;\r\n}\r\n\r\n/* Style for headings */\r\nh1, h2, h3 {\r\n  color: #ffffff;\r\n  margin-bottom: 10px;\r\n}\r\n\r\n/* Style for paragraphs */\r\np {\r\n  line-height: 1.6;\r\n  margin-bottom: 20px;\r\n}\r\n\r\n/* Links */\r\na {\r\n  color: #bb86fc;\r\n  text-decoration: none;\r\n  transition: color 0.3s;\r\n}\r\n\r\na:hover {\r\n  color: #ff79c6;\r\n}\r\n\r\n/* Container */\r\n.container {\r\n  max-width: 800px;\r\n  margin: 0 auto;\r\n  padding: 20px;\r\n}\r\n\r\n/* Example section */\r\n.section {\r\n  background-color: #121212;\r\n  border-radius: 5px;\r\n  padding: 20px;\r\n  margin-bottom: 30px;\r\n}\r\n\r\n/* Example button */\r\n.button {\r\n  display: inline-block;\r\n  padding: 10px 20px;\r\n  background-color: #6200ea;\r\n  color: #ffffff;\r\n  border: none;\r\n  border-radius: 5px;\r\n  cursor: pointer;\r\n  transition: background-color 0.3s;\r\n}\r\n\r\n.button:hover {\r\n  background-color: #3700b3;\r\n}\r\n\r\n/* Blockquote style */\r\nblockquote {\r\n  background-color: #333333;\r\n  border-left: 4px solid #888888;\r\n  padding: 10px;\r\n  margin: 0;\r\n  margin-bottom: 20px;\r\n  font-style: italic;\r\n}\r\n", Brushes.Transparent)
    };
    public AvaloniaList<CssStyle> CssStyles
    {
        get => _cssStyles;
        set => this.RaiseAndSetIfChanged(ref _cssStyles, value);
    }

    private CssStyle? _selectedStyle;
    public CssStyle? SelectedStyle
    {
        get => _selectedStyle;
        set => this.RaiseAndSetIfChanged(ref _selectedStyle, value);
    }

    private readonly IMediator _mediator;
    private readonly MarkdownTranslationService _translationService;

    public MarkdownPreviewViewModel()
        : this(Dependencies.GetRequiredService<IMediator>(),
               Dependencies.GetRequiredService<MarkdownTranslationService>())
    {

    }

    public MarkdownPreviewViewModel(IMediator mediator, MarkdownTranslationService translationService)
    {
        _mediator = mediator;
        _translationService = translationService;
        _selectedStyle = _cssStyles.FirstOrDefault();
        TrySetPreviewStyleAccordingToTheme();
    }

    private void TrySetPreviewStyleAccordingToTheme()
    {
        if (Avalonia.Application.Current!.ActualThemeVariant.Key.Equals("Light"))
        {
            SelectedStyle = CssStyles[0];
        }
        if (Avalonia.Application.Current!.ActualThemeVariant.Key.Equals("Dark"))
        {
            SelectedStyle = CssStyles[1];
        }
    }

    public async Task Handle(GenerateHtmlPreviewRequest request, CancellationToken cancellationToken)
        => await GenerateHtmlPreview();


    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> GeneratePreviewCommand
        => ReactiveCommand.CreateFromTask(
            async () => await GenerateHtmlPreview()
            );

    private async Task GenerateHtmlPreview()
    {
        var previewHtmlGeneration = await GetPreviewHtml();
        if (previewHtmlGeneration)
        {
            await _mediator.Publish(new OperationOutcomeNotification(previewHtmlGeneration.ResultType, "HTML preview generated"));
        }
        else
        {
            await _mediator.Publish(new OperationOutcomeNotification(previewHtmlGeneration.ResultType, previewHtmlGeneration.Message));
        }

        GeneratedHtml =
            previewHtmlGeneration
            ? previewHtmlGeneration.Data
            : string.Empty;
    }

    private async Task<Result<string>> GetPreviewHtml()
    {
        var figures = _mediator.Send(new GetCurrentFiguresRequest());
        var markdown = _mediator.Send(new GetCurrentMarkdownRequest());

        var translation = _translationService.TranslateToHtml(await markdown, await figures);

        return translation;
    }
}
