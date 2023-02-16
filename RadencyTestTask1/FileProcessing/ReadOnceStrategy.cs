using RadencyTestTask1.Entities;

namespace RadencyTestTask1.FileProcessing;
/// <summary>
/// Reads all files in directory once and exits. Solely for testing purposes.
/// </summary>
public class ReadOnceStrategy : ProcessStrategy
{
    public override void ProcessDirectory(string path)
    {
        var fileEntries = Directory.GetFiles(path);
        foreach (var filePath in fileEntries)
        {
            Console.WriteLine(filePath);
            if(TokenSource.IsCancellationRequested) return;
            ProcessFile(filePath);
        }
        Task.WaitAll(SaveTasks.ToArray());
        SaveTasks.Clear();
    }

    public ReadOnceStrategy(AppConfig config) : base(config)
    {
    }
}