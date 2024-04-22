using Mf.Intr.Core.Db;
using Mf.Intr.Core.Db.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess;

public static class ConnectionStringBuilder
{
    private static Dictionary<IntrConnectionTypes, Func<ConnectionEntity,string>> _methods;

    static ConnectionStringBuilder()
    {
        _methods = new Dictionary<IntrConnectionTypes, Func<ConnectionEntity, string>>()
        { 
            { IntrConnectionTypes.MSSQL, BuildMsSql },
            { IntrConnectionTypes.HANA, BuildHana }
        };
    }

    public static string BuildConnectionString(ConnectionEntity connection)
    {
        return _methods[connection.ConnectionType].Invoke(connection);
    }

    private static string BuildMsSql(ConnectionEntity connection)
    {
        string server = $"SERVER={connection.Host},{connection.Port};";
        string database = $"DATABASE={connection.Database};UID={connection.DbUser};PASSWORD={connection.DbPassword};";
        string trusted = "TrustServerCertificate=true;Trusted_Connection=False;";
        string extra = "Max Pool Size=1024;Application Name=Mf.Intr.Core.DataAccess;";

        return server + database + trusted + extra;
    }

    private static string BuildHana(ConnectionEntity connection)
    {
        string server = $"Server={connection.Host}:{connection.Port};";
        string tenant = $"databaseName={connection.Database};UserID={connection.DbUser};password={connection.DbPassword};";
        string schema = $"Current Schema={connection.Schema};";

        return server + tenant + schema;
    }
}
