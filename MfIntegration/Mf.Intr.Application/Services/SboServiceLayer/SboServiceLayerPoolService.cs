using Flurl.Http;
using Mf.Intr.Core.Db;
using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Exceptions;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer;

public class SboServiceLayerPoolService : ISboServiceLayerPoolService
{
    private readonly ConcurrentDictionary<string, SboServiceLayerConnection> _pool = new();

    public ISboServiceLayerConnection Get(string nameOfConnection)
    {
        if(_pool.TryGetValue(nameOfConnection, out var sl) == false)
        {
            throw new IntegrationException($"ServiceLayer was not found in the pool of services by name of connection: {nameOfConnection}");
        }
       
        return sl;
    }

    internal void Add(string nameOfConnection, SboServiceLayerConnection slConnection)
    {
        _pool.TryAdd(nameOfConnection, slConnection);
    }
}
