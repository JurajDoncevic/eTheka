using eTheka.Base;
using eTheka.Domain;
using System.IO.Compression;

namespace eTheka.Services;

/// <summary>
/// File manipulation service
/// </summary>
public class FileService
{
    /// <summary>
    /// Supported image formats
    /// </summary>
    private static List<string> SUPPORTED_IMAGE_FORMATS = new()
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    /// <summary>
    /// Saves the markdown extended file
    /// </summary>
    /// <param name="filepath">Destination filepath</param>
    /// <param name="fileContent">File content to save</param>
    /// <returns></returns>
    public Result SaveMarkdownExtendedFile(Uri filepath, MarkdownExtendedFile fileContent)
        => Results.AsResult(() =>
        {
            using (FileStream zipToOpen = new FileStream(filepath.AbsolutePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    ZipArchiveEntry markdownEntry = archive.CreateEntry("markdown.md");
                    using (StreamWriter writer = new StreamWriter(markdownEntry.Open()))
                    {
                        writer.Write(fileContent.Markdown);
                    }

                    foreach (var figure in fileContent.Figures)
                    {
                        ZipArchiveEntry figureEntry = archive.CreateEntry(figure.Name + figure.Extension);
                        using (var stream = figureEntry.Open())
                        {
                            using (var ms =  new MemoryStream(figure.FileBytes))
                            {
                                ms.WriteTo(stream);
                            }
                        }
                    }
                }
            }

            return Results.OnSuccess("Markdown file saved successfully");
        });

    /// <summary>
    /// Opens a markdown extended file
    /// </summary>
    /// <param name="filepath">Source filepath</param>
    /// <returns>Opened markdown extended file</returns>
    public Result<MarkdownExtendedFile> OpenMarkdownExtendedFile(Uri filepath)
        => Results.AsResult(() =>
        {
            string markdown = string.Empty;
            List<Figure> figures = new List<Figure>();

            using (FileStream zipToOpen = new FileStream(filepath.AbsolutePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry markdownEntry = archive.GetEntry("markdown.md")!;
                    using (StreamReader reader = new StreamReader(markdownEntry.Open()))
                    {
                        markdown = reader.ReadToEnd();
                    }
                    var figureEntries =
                        archive.Entries
                        .Where(entry => SUPPORTED_IMAGE_FORMATS.Map(e => e.TrimStart('.')).Contains(entry.Name.Split(".").LastOrDefault()!));
                    foreach (var figureEntry in figureEntries)
                    {
                        using (var stream = figureEntry.Open())
                        {
                            using (var ms = new MemoryStream())
                            {
                                stream.CopyTo(ms);
                                figures.Add(new Figure(figureEntry.Name.Split(".")[0], figureEntry.Name.Split(".")[1], ms.ToArray()));
                            }
                        }
                    }
                }
            }
            return Results.OnSuccess(new MarkdownExtendedFile(filepath.Segments.Last(), filepath, markdown, figures), "Markdown file opened");
        });

    /// <summary>
    /// Saves a html file
    /// </summary>
    /// <param name="filepath">Destination filepath</param>
    /// <param name="htmlText">HTML text</param>
    /// <returns></returns>
    public Result SaveHtmlFile(Uri filepath, string htmlText)
        => Results.AsResult(() =>
        {
            File.WriteAllText(filepath.AbsolutePath, htmlText);

            return Results.OnSuccess("HTML file saved");
        });

    /// <summary>
    /// Opens an image file
    /// </summary>
    /// <param name="filepath">Source filepath</param>
    /// <returns></returns>
    public Result<ImageFile> OpenImageFile(Uri filepath)
        => Results.AsResult(() =>
        {
            var fileInfo = new FileInfo(filepath.AbsolutePath);
            if (!SUPPORTED_IMAGE_FORMATS.Contains(fileInfo.Extension))
            {
                return Results.OnFailure<ImageFile>($"Unsupported file format for image file: {fileInfo.Extension}");
            }

            var imageBytes = File.ReadAllBytes(filepath.AbsolutePath);

            var imageFile = new ImageFile(fileInfo.Name.Replace(fileInfo.Extension, ""), filepath, fileInfo.Extension, imageBytes);

            return Results.OnSuccess(imageFile, "Image file opened");
        });
}
