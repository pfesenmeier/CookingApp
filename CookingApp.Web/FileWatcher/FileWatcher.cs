namespace CookingApp.Web.FileWatcher;

public record FileChangedEvent;

public class FileChangedEventSource
{
    private FileSystemWatcher FileSystemWatcher { get; }
    private ILogger Logger { get; }

    public Task<FileChangedEvent> WaitForNewChange()
    {
        WaitForChangedResult result = FileSystemWatcher.WaitForChanged(WatcherChangeTypes.All);
        Logger.LogInformation("File event detected: {@Result}", result);
        return Task.FromResult(new FileChangedEvent());
    }

    public FileChangedEventSource(string path, ILogger logger)
    {
        Logger = logger;
        Logger.LogInformation(Environment.CurrentDirectory);
        FileSystemWatcher = new(path)
        {
            IncludeSubdirectories = true,
            // NotifyFilter = Filters
        };
    }
    // private static readonly NotifyFilters Filters = NotifyFilters.LastWrite
    //                                                 | NotifyFilters.Size
    //                                                 | NotifyFilters.CreationTime
    //                                                 | NotifyFilters.DirectoryName
    //                                                 | NotifyFilters.FileName;
}
