namespace MakeKits.Workshop;

/// <summary>
/// Represents a workshop item with metadata such as name, author, version, and file information.
/// </summary>
public interface IWorkshopItem
{
    /// <summary>
    /// Gets or sets the name of the workshop item.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the author of the workshop item.
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    /// Gets or sets an optional longer description of the workshop.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the version of the workshop item.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the file name of the workshop item.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the file path of the workshop item.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the associated workshop instance for this item.
    /// </summary>
    public IWorkshop Workshop { get; set; }
}
