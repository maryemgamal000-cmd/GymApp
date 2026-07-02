using GymSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository :IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategory(CancellationToken ct=default);

        Task<Session?> GetSessionByIdWitTrainerAndCategory( int sessionId, CancellationToken ct = default);

        Task<int> GetCountOfBookedSlots(int sessionId  , CancellationToken ct=default);     
    }
}
