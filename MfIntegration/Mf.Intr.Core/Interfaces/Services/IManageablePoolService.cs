using Mf.Intr.Core.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces.Services;

public interface IManageablePoolService
{
    void Set(SchedulerType schedulerType, int key, IManageable value);
    IManageable? Get(SchedulerType schedulerType, int key);
    IManageable GetFromCache(IManageable manageable, SchedulerType schedulerType, int eventId);
}
