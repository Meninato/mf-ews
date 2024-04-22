using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Db;

public interface IDbQueryConverter
{
    PropertyInfo? GetKeyPropertyInfo(object instance);
    string ConvertToInsert<T>(T obj) where T : class;
    string ConvertToUpdate<T>(T obj) where T : class;
}
