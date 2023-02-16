namespace RadencyTestTask1.FileProcessing;

public class ReadContinouslyStrategy : ProcessStrategy
{
    public override void ProcessDirectory(string path)
    {
        using var watcher = new FileSystemWatcher(path);
        watcher.Filters.Add("*.csv");
        watcher.Filters.Add("*.txt");
        watcher.NotifyFilter = NotifyFilters.FileName;
        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Press ESC to exit");
        ConsoleKeyInfo key = new ConsoleKeyInfo();

        while (!Console.KeyAvailable && key.Key != ConsoleKey.Escape)
        {

            key = Console.ReadKey(true);
            Console.WriteLine(key.Key);
            if (key.Key == ConsoleKey.C)
            {
                Console.WriteLine("Canceling tasks...");
                TokenSource.Cancel();
                Thread.Sleep(2500);
                break;
            }
        }
        Console.WriteLine("Ending");
    }
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        Console.WriteLine($"Changed: {e.FullPath}");
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
        ProcessFile(e.FullPath);
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e) =>
        Console.WriteLine($"Deleted: {e.FullPath}");

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"Renamed:");
        Console.WriteLine($"    Old: {e.OldFullPath}");
        Console.WriteLine($"    New: {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private static void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException);
        }
    }
}