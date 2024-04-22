using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Interfaces.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Repositories;
public class ManagerRepository : Repository<ManagerEntity>, IManagerRepository
{
    public ManagerRepository(IntrDbContext context) : base(context)
    {
    }

    public override IEnumerable<ManagerEntity> GetAll()
    {
        return GetManagerWithRelationship().ToList();
    }

    public override async Task<IEnumerable<ManagerEntity>> GetAllAsync()
    {
        return await GetManagerWithRelationship().ToListAsync();
    }

    public override ManagerEntity? GetById(int id)
    {
        return GetManagerWithRelationship().FirstOrDefault(m => m.ID == id);
    }

    public override async Task<ManagerEntity?> GetByIdAsync(int id)
    {
        return await GetManagerWithRelationship().FirstOrDefaultAsync(m => m.ID == id);
    }

    private IQueryable<ManagerEntity> GetManagerWithRelationship()
    {
        return _context.Managers
            .Include(m => m.Parameters)
            .Include(m => m.FirstWorker);
    }
}
