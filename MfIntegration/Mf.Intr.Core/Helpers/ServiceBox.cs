using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mf.Intr.Core.Helpers;

public abstract class ServiceBox : IServiceBox
{
    private Dictionary<Type, object> _box = new Dictionary<Type, object>();

    public virtual void Validate()
    {
        if (this.Contains<ILogger>() == false) throw new IntegrationException("Missing ILogger for ServiceBox");
        if (this.Contains<IConfigurationService>() == false) throw new IntegrationException("Missing IConfigurationService for ServiceBox");
        if (this.Contains<IEncryptorService>() == false) throw new IntegrationException("Missing IEncryptorService for ServiceBox");
        if (this.Contains<IUnitOfWork>() == false) throw new IntegrationException("Missing IUnitOfWork for ServiceBox");
        if (this.Contains<ISboServiceLayerPoolService>() == false) throw new IntegrationException("Missing ISboServiceLayerPoolService for ServiceBox");
    }

    public void Add<T>(T obj) where T: class
    {
        _box.TryAdd(typeof(T), obj);
    }

    public T Get<T>() where T: class
    {
        _box.TryGetValue(typeof(T), out var obj);

        if(obj == null)
        {
            throw new IntegrationException($"ServiceBox doesn't contain {nameof(T)}");
        }

        return (T)obj;
    }

    public bool Contains<T>() where T: class
    {
        return _box.ContainsKey(typeof(T));
    }

    public IConfigurationService GetConfiguration()
    {
        return this.Get<IConfigurationService>();
    }

    public IEncryptorService GetEncryptor()
    {
        return this.Get<IEncryptorService>();
    }

    public ILogger GetLogger()
    {
        return this.Get<ILogger>();
    }

    public IUnitOfWork GetUnitOfWork()
    {
        return this.Get<IUnitOfWork>();
    }

    public ISboServiceLayerPoolService GetSboServiceLayerPool()
    {
        return this.Get<ISboServiceLayerPoolService>();
    }
}
