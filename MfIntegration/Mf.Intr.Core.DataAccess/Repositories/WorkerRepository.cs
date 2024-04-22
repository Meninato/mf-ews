using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Interfaces.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Repositories;

public class WorkerRepository : Repository<WorkerEntity>, IWorkerRepository
{
    public WorkerRepository(IntrDbContext context) : base(context)
    {
    }

    public override IEnumerable<WorkerEntity> GetAll()
    {
        return GetWorkersWithRelationship().ToList();
    }

    public override async Task<IEnumerable<WorkerEntity>> GetAllAsync()
    {
        return await GetWorkersWithRelationship().ToListAsync();
    }

    public override WorkerEntity? GetById(int id)
    {
        return GetWorkersWithRelationship().FirstOrDefault(w => w.ID == id);
    }

    public override async Task<WorkerEntity?> GetByIdAsync(int id)
    {
        return await GetWorkersWithRelationship().FirstOrDefaultAsync(w => w.ID == id);
    }

    private IQueryable<WorkerEntity> GetWorkersWithRelationship()
    {
        return _context.Workers.Include(w => w.Parameters);
    }
}
