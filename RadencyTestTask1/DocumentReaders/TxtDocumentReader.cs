using RadencyTestTask1.DocumentReaders;
using RadencyTestTask1.LineReaders;
using RadencyTestTask1.LineReaders;

namespace RadencyTestTask1.DocumentReaders;

public class TxtDocumentReader : DocumentReader
{
    private ILineReader? _reader;
    public TxtDocumentReader(string path) : base(path) { }

    protected override ILineReader GetReader()
    {
        return _reader ??= new LineReader(Path);
    }
}