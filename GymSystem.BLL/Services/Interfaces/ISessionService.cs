using GymSystem.BLL.Common;
using GymSystem.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Result<IEnumerable<SessionViewModel>?>> GetAllSessionsAsync (CancellationToken ct=default);

        Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainerForDropDownAsync (CancellationToken ct=default);
        Task<Result<IEnumerable<CategorySelectViewModel>>> GetCategoryForDropDownAsync(CancellationToken ct = default);



        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);

        Task<Result<SessionViewModel>> GetSessionDetailsByIdAsync (int sessionId , CancellationToken ct=default);

        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync (int sessionId , CancellationToken ct=default);

        Task<Result> UpdateSessionAsync (int sessionId, UpdateSessionViewModel model,  CancellationToken ct = default);

        Task<Result> RemoveSessionAsync (int sessionId , CancellationToken ct=default);


    }
}
