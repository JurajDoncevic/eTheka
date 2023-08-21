namespace eTheka.Domain;

/// <summary>
/// Image file class. Used to represent an image on disk.
/// </summary>
public class ImageFile
{
    private readonly string _name;
    private readonly Uri _filePath;
    private readonly string _extension;
    private readonly byte[] _fileBytes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Image name - sans extension</param>
    /// <param name="filePath">Image filepath</param>
    /// <param name="extension">Image file extension</param>
    /// <param name="fileBytes">Image file bytes</param>
    public ImageFile(string name, Uri filePath, string extension, byte[] fileBytes)
    {
        _name = name;
        _filePath = filePath;
        _extension = extension.StartsWith('.') ? extension : $".{extension}";
        _fileBytes = fileBytes;
    }
    /// <summary>
    /// Image name with extension
    /// </summary>
    public string FullName => _name + _extension;
    /// <summary>
    /// Image name - filename sans extension
    /// </summary>
    public string Name => _name;
    /// <summary>
    /// Image filepath
    /// </summary>
    public Uri FilePath => _filePath;
    /// <summary>
    /// Image extension
    /// </summary>
    public string Extension => _extension;
    /// <summary>
    /// Image filebytes
    /// </summary>
    public byte[] FileBytes => _fileBytes;

}
