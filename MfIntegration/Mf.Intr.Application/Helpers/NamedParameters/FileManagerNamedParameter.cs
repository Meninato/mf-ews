using Mf.Intr.Core.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Helpers.NamedParameters;

public class FileManagerNamedParameter : ManagerNamedParameter
{
    public DirectoryInfo Directory { get; private set; }
    public FileInfo File { get; private set; }

    public FileManagerNamedParameter(ManagerEntity manager, int eventGeneratorId, bool skipIfBusyStrategy, 
        DirectoryInfo directory, FileInfo file) : base(manager, eventGeneratorId, skipIfBusyStrategy)
    {
        Directory = directory;
        File = file;
    }
}
