using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces;

public interface IServiceBox
{
    T Get<T>() where T: class;
    ILogger GetLogger();
    IEncryptorService GetEncryptor();
    IConfigurationService GetConfiguration();
    IUnitOfWork GetUnitOfWork();
    ISboServiceLayerPoolService GetSboServiceLayerPool();
    void Add<T>(T obj) where T : class;
    void Validate();
    bool Contains<T>() where T : class;
}