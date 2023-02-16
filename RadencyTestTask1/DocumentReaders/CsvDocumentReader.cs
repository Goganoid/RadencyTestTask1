using RadencyTestTask1.LineReaders;
namespace RadencyTestTask1.DocumentReaders;

public class CsvDocumentReader : DocumentReader
{
    private ILineReader? _reader;
    public CsvDocumentReader(string path) : base(path) { }

    protected override ILineReader GetReader()
    {
        if (_reader == null)
        {
            _reader = new LineReader(Path);
            // skip header
            _reader.ReadLine();
        }
        return _reader;
    }

}