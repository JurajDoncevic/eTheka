namespace eTheka.Domain;

/// <summary>
/// Markdown extended document figure
/// </summary>
public class Figure
{
    /// <summary>
    /// Constructor
    /// </summary>
    public Figure()
    {
        Name = string.Empty;
        Extension = string.Empty;
        FileBytes = new byte[0];
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Figure name</param>
    /// <param name="extension">Figure image extension containing '.'</param>
    /// <param name="fileBytes">Figure file bytes</param>
    public Figure(string name, string extension, byte[] fileBytes)
    {
        Name = name;
        Extension = extension.StartsWith('.') ? extension : $".{extension}";
        FileBytes = fileBytes;
    }
    /// <summary>
    /// Full name of the figure; name with extension
    /// </summary>
    public string FullName => Name + Extension;
    /// <summary>
    /// Figure name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Figure image extension containing '.'
    /// </summary>
    public string Extension { get; set; }
    /// <summary>
    /// Figure file bytes
    /// </summary>
    public byte[] FileBytes { get; set; }
}
