using Mf.Intr.Core.Interfaces;

namespace Mf.Intr.Core.Workers.Implemented;

public abstract class FileWorker<TOutput, TInput> : Worker<TOutput, TInput>, IFileWorkable
{
    private FileInfo _fileInfo = null!;
    private DirectoryInfo _directoryInfo = null!;

    public FileInfo File => _fileInfo;
    public DirectoryInfo Directory => _directoryInfo;

    protected FileWorker(IWorkerServiceBox box) : base(box)
    {
    }

    private void InitFileWorkablePrivateFields(DirectoryInfo directoryInfo, FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        _directoryInfo = directoryInfo;
    }
}

public abstract class FileWorker<TOutput> : Worker<TOutput>, IFileWorkable
{
    private FileInfo _fileInfo = null!;
    private DirectoryInfo _directoryInfo = null!;

    public FileInfo File => _fileInfo;
    public DirectoryInfo Directory => _directoryInfo;

    protected FileWorker(IWorkerServiceBox box) : base(box)
    {
    }

    private void InitFileWorkablePrivateFields(DirectoryInfo directoryInfo, FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        _directoryInfo = directoryInfo;
    }
}
