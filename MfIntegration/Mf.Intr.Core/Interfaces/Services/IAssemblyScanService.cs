using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services;

public interface IAssemblyScanService
{
    void Load(string directoryPath);
    void Add(string fileName);
    Type? GetType(string classType);
    bool ImplementsWorkable(Type workerType);
    Type GetWorkableType(string? classType);
    bool ImplementsManageable(Type managerType);
    Type GetManageableType(string? classType);
    bool ImplementsCompanyWorkable(Type workableType);
    Type GetCompanyWorkableType(string? classType);
    Type GetCompanyManageableType(string? classType);
    bool ImplementsCompanyManageable(Type manageableType);
    Type GetFileWorkableType(string? classType);
    bool ImplementsFileWorkable(Type workableType);
    bool ImplementsFileManageable(Type manageableType);
    Type GetFileManageableType(string? classType);
    Type GetHanaPluginType(string? classType);
    bool ImplementsHanaPluginContext(Type hanaPlugin);
    bool ImplementsSharedWorkable(Type workableType);
    Type GetSharedWorkableType(string? classType);
}
