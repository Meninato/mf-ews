using Mf.Intr.Core.Schedulers;
using Mf.Intr.Core.Interfaces;
using Mf.Intr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services;

public class ManageablePoolService : IManageablePoolService
{
    private Dictionary<Tuple<SchedulerType, int>, IManageable> _managerCache;

    public ManageablePoolService()
    {
        _managerCache = new Dictionary<Tuple<SchedulerType, int>, IManageable>();
    }

    public IManageable? Get(SchedulerType schedulerType, int key)
    {
        IManageable? manageable = null;
        Tuple<SchedulerType, int> cacheKey = GetCacheKey(schedulerType, key);
        if(_managerCache.ContainsKey(cacheKey))
        {
            manageable = _managerCache[cacheKey];
        }

        return manageable;
    }

    public void Set(SchedulerType schedulerType, int key, IManageable value)
    {
        Tuple<SchedulerType, int> cacheKey = GetCacheKey(schedulerType, key);
        if (_managerCache.ContainsKey(cacheKey) == false)
        {
            _managerCache.Add(cacheKey, value);
        }
    }

    public IManageable GetFromCache(IManageable manageable, SchedulerType schedulerType, int eventId)
    {
        //I'm informing the company event ID and not the manager for the pool.
        //This fixes the problem when a manager is being reused multiple times.

        IManageable? result = manageable;
        if (manageable.SkipIfBusyStrategy)
        {
            result = Get(schedulerType, eventId);
            if (result == null)
            {
                Set(schedulerType, eventId, manageable);
                result = manageable;
            }
        }

        return result;
    }

    private Tuple<SchedulerType, int> GetCacheKey(SchedulerType schedulerType, int key)
    {
        return new Tuple<SchedulerType, int>(schedulerType, key);
    }
}
