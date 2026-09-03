using GymSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity , new()

    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);

        Task<TEntity?> GetByIDAsync(int id, CancellationToken ct = default);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity , bool>> Perdicit, CancellationToken ct = default);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> Perdicit, bool tracking = false, CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null , CancellationToken ct = default);
    
    }
}
