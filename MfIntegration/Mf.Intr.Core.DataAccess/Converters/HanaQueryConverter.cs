using Mf.Intr.Core.Interfaces.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Converters;

public class HanaQueryConverter : QueryConverter, IDbQueryConverter
{
    public HanaQueryConverter() { }

    public string ConvertToInsert<T>(T obj) where T : class
    {
        throw new NotImplementedException();
    }

    public string ConvertToUpdate<T>(T obj) where T : class
    {
        throw new NotImplementedException();
    }
}
