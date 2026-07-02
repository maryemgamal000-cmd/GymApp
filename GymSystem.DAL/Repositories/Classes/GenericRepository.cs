using GymSystem.DAL.Data.DBContexts;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {

        private readonly DbContext _dbcontext;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(GymDbContext dbContext)
        {
            _dbcontext = dbContext;
            _dbSet = _dbcontext.Set<TEntity>();
        }



        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
            
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> Perdicit, CancellationToken ct = default)
        {
            return _dbSet.AsNoTracking().AnyAsync(Perdicit ,ct);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> Perdicit, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _dbSet : _dbSet.AsNoTracking();
            return await query.FirstOrDefaultAsync(Perdicit);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _dbSet : _dbSet.AsNoTracking();
            return await query.ToListAsync(ct);

        }

        public async Task<TEntity?> GetByIDAsync(int id, CancellationToken ct = default)
        {

            return await _dbSet.FindAsync(id, ct);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
           


        }
    }
}
