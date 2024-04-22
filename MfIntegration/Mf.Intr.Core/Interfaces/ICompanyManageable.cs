using Mf.Intr.Core.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces;

public interface ICompanyManageable : IManageable
{
    public string? Query { get; }
    public ConnectionEntity Connection { get; }
}
