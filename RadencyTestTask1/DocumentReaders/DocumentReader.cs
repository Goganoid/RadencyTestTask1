using RadencyTestTask1.LineReaders;

namespace RadencyTestTask1.DocumentReaders;

public abstract class DocumentReader
{
    protected string Path { get; set; }
    public DocumentReader(string path)
    {
        Path = path;
    }

    protected abstract ILineReader GetReader();

    public IEnumerable<string> GetLines()
    {
        var reader = GetReader();
        var line = reader.ReadLine();
        while (line != null)
        {
            yield return line;
            line = reader.ReadLine();
        }
        if(line==null) yield break;
    }
}