using Flurl.Http.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mf.Intr.Application;
public static class AppDefaults
{
    public const string ENVIRONMENT_VARIABLE = "INTR_ENVIRONMENT";
    public const string ENVIRONMENT_VALUE_DEV = "development";
    public const string ENVIRONMENT_VALUE_PROD = "production";

    public const string SETTINGS_FILENAME = "appsettings.json";
    public const string SETTINGS_DEV_FILENAME = "appsettings.development.json";
    public const string SETTINGS_PROD_FILENAME = "appsettings.production.json";

    public const string LOGGER_FOLDER = "logs";
    public const string CONFIG_FOLDER = "settings";
    public const string LOGGER_NAME = "log";
    public const string ASSEMBLY_FOLDER = "tasks";
    public const string PLUGIN_FOLDER = "plugins";
    public const string PLUGIN_HANA_FOLDER = "hana";

    public const string NAMED_PARAMETER_COMPANYMANAGER = "namedcompanymanager";
    public const string NAMED_PARAMETER_MANAGER = "namedmanager";
    public const string NAMED_PARAMETER_COMPANYWORKER = "namedcompanyworker";
    public const string NAMED_PARAMETER_WORKER = "namedworker";
    public const string NAMED_PARAMETER_FILEMANAGER = "namedfilemanager";
    public const string NAMED_PARAMETER_FILEWORKER = "namedfileworker";
    public const string NAMED_PARAMETER_SHAREDWORKER = "namedsharedworker";

    public const string JOB_DATA_COMPANYEVENT = "companyevent";

    public const string APP_OPTIONS = "Intr.Application";

    public const string PRIVATE_METHOD_WORKER = "InitWorkablePrivateFields";
    public const string PRIVATE_METHOD_WORKER_COMPANY = "InitCompanyWorkablePrivateFields";
    public const string PRIVATE_METHOD_MANAGER = "InitManageablePrivateFields";
    public const string PRIVATE_METHOD_MANAGER_COMPANY = "InitManageableCompanyPrivateFields";
    public const string PRIVATE_METHOD_MANAGER_FILE = "InitFileManageablePrivateFields";
    public const string PRIVATE_METHOD_WORKER_FILE = "InitFileWorkablePrivateFields";

    public static JsonSerializerOptions DefaultJsonSerializerOptions => new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
