using RadencyTestTask1.Entities;
using RadencyTestTask1.Helpers;

namespace RadencyTestTask1.FileProcessing;

public class ReadContinuouslyStrategy : ProcessStrategy
{
    
    private void Reset()
    {
        CancelTasks();
        Config = ConfigReader.ReadConfig();
        TokenSource = new CancellationTokenSource();
        AggregationSaver = new (Config.OutputDirectory);
        SaveTasks = new();
        Console.WriteLine("Restarted the app");
    }

    private void CancelTasks()
    {
        Console.Write("Canceling tasks...");
        TokenSource.Cancel();
        Thread.Sleep(2000);
        Console.Write("Ok\n");
    }
    public override void ProcessDirectory(string path)
    {
        using var watcher = new FileSystemWatcher(path);
        watcher.Filters.Add("*.csv");
        watcher.Filters.Add("*.txt");
        watcher.NotifyFilter = NotifyFilters.FileName;
        watcher.Created += OnCreated;
        watcher.Error += OnError;
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Press C to exit");
        var key = new ConsoleKeyInfo();
        while (!Console.KeyAvailable && key.Key != ConsoleKey.C)
        {

            key = Console.ReadKey(true);
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (key.Key)
            {
                case ConsoleKey.S:
                    Console.WriteLine($"{(watcher.EnableRaisingEvents ? "Stopped the watcher" : "Started the watcher")}");
                    watcher.EnableRaisingEvents = !watcher.EnableRaisingEvents;
                    break;
                case ConsoleKey.R:
                    Reset();
                    break;
                default:
                    break;
            }
        }
        CancelTasks();
        Console.WriteLine("Ending");
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"New file was added: {e.FullPath}";
        Console.WriteLine(value);
        ProcessFile(e.FullPath);
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

    public ReadContinuouslyStrategy(AppConfig config) : base(config)
    {
    }
}