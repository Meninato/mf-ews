using Mf.Intr.Core.Db.Entities;
using Mf.Intr.Core.Interfaces.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.DataAccess.Repositories;
public class EventGeneratorRepository : Repository<EventGeneratorEntity>, IEventGeneratorRepository
{
    public EventGeneratorRepository(IntrDbContext context) : base(context)
    {
    }

    public override IEnumerable<EventGeneratorEntity> GetAll()
    {
        return GetEventGeneratorWithRelationship().ToList();
    }

    public override async Task<IEnumerable<EventGeneratorEntity>> GetAllAsync()
    {
        return await GetEventGeneratorWithRelationship().ToListAsync();
    }

    public override EventGeneratorEntity? GetById(int id)
    {
        return GetEventGeneratorWithRelationship().FirstOrDefault(ev => ev.ID == id);
    }

    public override async Task<EventGeneratorEntity?> GetByIdAsync(int id)
    {
        return await GetEventGeneratorWithRelationship().FirstOrDefaultAsync(ev => ev.ID == id);
    }

    private IQueryable<EventGeneratorEntity> GetEventGeneratorWithRelationship()
    {
        return _context.EventGenerators
            .Include(ev => ev.Manager)
                .ThenInclude(m => m.FirstWorker)
            .Include(ev => ev.Manager)
                .ThenInclude(m => m.Parameters)
            .Include(ev => ev.CompanyEvents!)
                .ThenInclude(companyEv => companyEv.Connection)
            .Include(ev => ev.FileEvent);
    }
}
