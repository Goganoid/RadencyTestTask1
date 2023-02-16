namespace RadencyTestTask1.FileProcessing;

public class ReadOnceStrategy : ProcessStrategy
{
    public override void ProcessDirectory(string path)
    {
        var fileEntries = Directory.GetFiles(path);
        foreach (var filePath in fileEntries)
        {
            Console.WriteLine(filePath);
            if(TokenSource.IsCancellationRequested) return;
            AggregationSaver.ParsedFiles++;
            ProcessFile(filePath);
        }
        Task.WaitAll(SaveTasks.ToArray());
        SaveTasks.Clear();
    }
}