using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Managers.Implemented;

public abstract class FileManager : Manager, IFileManageable
{
    private FileInfo _fileInfo = null!;
    private DirectoryInfo _directoryInfo = null!;

    public FileInfo File => _fileInfo;
    public DirectoryInfo Directory => _directoryInfo;

    public FileManager(IManagerServiceBox box) : base(box)
    {

    }

    private void InitFileManageablePrivateFields(DirectoryInfo directoryInfo, FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        _directoryInfo = directoryInfo;
    }
}
