namespace eTheka.Domain;
/// <summary>
/// Markdown extended file
/// </summary>
public class MarkdownExtendedFile
{
    private readonly string _fileName;
    private readonly Uri _filePath;
    private readonly string _markdown;
    private readonly IEnumerable<Figure> _figures;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="filePath">File path</param>
    /// <param name="markdown">Markdown text</param>
    /// <param name="figures">Referencable figures</param>
    public MarkdownExtendedFile(string fileName, Uri filePath, string markdown, IEnumerable<Figure> figures)
    {
        _fileName = fileName;
        _filePath = filePath;
        _markdown = markdown;
        _figures = figures;
    }
    /// <summary>
    /// File name
    /// </summary>
    public string FileName => _fileName;
    /// <summary>
    /// File path
    /// </summary>
    public Uri FilePath => _filePath;
    /// <summary>
    /// Markdown text
    /// </summary>
    public string Markdown => _markdown;
    /// <summary>
    /// Markdown referencable figures
    /// </summary>
    public IReadOnlyList<Figure> Figures => _figures.ToList();
}
