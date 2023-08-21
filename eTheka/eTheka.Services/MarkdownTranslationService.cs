using eTheka.Base;
using eTheka.Domain;
using Markdig;
using System.Text.RegularExpressions;

namespace eTheka.Services;

/// <summary>
/// Markdown translation service
/// </summary>
public class MarkdownTranslationService
{
    public enum Styles
    {
        DARK,
        LIGHT
    };

    private string GetStyleCss(Styles style)
        => style switch
        {
            Styles.LIGHT => "body {\nfont-family: 'Montserrat', sans-serif; \n}",
            Styles.DARK => "body {\nfont-family: 'Montserrat', sans-serif; \n}"
        };


    private readonly MarkdownPipeline _pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseEmojiAndSmiley()
            .UseBootstrap()
            .UseSoftlineBreakAsHardlineBreak()
            .Build();

    /// <summary>
    /// Translates markdown to HTML
    /// </summary>
    /// <param name="markdown">Markdown text</param>
    /// <param name="figures">Figures that can be referenced by <code>local:</code></param>
    /// <returns></returns>
    public Result<string> TranslateToHtml(string markdown, IEnumerable<Figure>? figures = null, Styles style = Styles.LIGHT)
        => Results.AsResult(() =>
            Results.AsResult<string>(() => Markdown.ToHtml(markdown, _pipeline))
                .Bind(html => PrependStyle(html, style))
                .Bind(SurroundInHtmlTags)
                .Bind(html => SubstituteLocalFigureReferences(figures, html))
        );

    /// <summary>
    /// Surrounds the translated html text with an html tag
    /// </summary>
    /// <param name="html">HTML text</param>
    /// <returns></returns>
    private Result<string> SurroundInHtmlTags(string html)
    {
        return "<html>\n" + html + "\n</html>";
    }
    
    private Result<string> PrependStyle(string html, Styles style)
    {
        return $"<style>\n{GetStyleCss(style)}\n</style>\n{html}";
    }

    /// <summary>
    /// Substitutes <code>local:</code> figure references with base64 data
    /// </summary>
    /// <param name="figures">Referencable figures</param>
    /// <param name="html">HTML text</param>
    /// <returns></returns>
    private Result<string> SubstituteLocalFigureReferences(IEnumerable<Figure>? figures, string html)
        => Results.AsResult<string>(() =>
        {
            var localRefs = Regex.Matches(html, "src=\"local:.+\"");
            foreach (Match localRef in localRefs)
            {
                string localSrcRef = Regex.Match(localRef.Value, @"src=""local:([^""]*)""").Groups[1].Value;


                var referencedFigure = figures?.FirstOrDefault(f => f.FullName.Equals(localSrcRef));
                if (referencedFigure != null)
                {
                    string extension = referencedFigure.Extension.TrimStart('.');
                    string figureByteReference = $"data:image/{extension};base64," + Convert.ToBase64String(referencedFigure.FileBytes);
                    html = html.Replace($"src=\"local:{localSrcRef}\"", $"src=\"{figureByteReference}\"");
                }

            }

            return html;
        });
}
