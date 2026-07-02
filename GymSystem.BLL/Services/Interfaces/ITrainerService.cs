using GymSystem.BLL.Common;
using GymSystem.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface ITrainerService
    {

        Task<Result<IEnumerable<TrainerViewModel>?>> GetAllTrainersAsync (CancellationToken ct = default);

        Task<Result<TrainerViewModel?>> GetTrainerDetailsAsync(int trainerId , CancellationToken ct = default);

        Task<Result<UpdateTrainerViewModel?>> GetTrainerToUpdateAsync (int trainerId, CancellationToken ct = default);
        Task<Result> UpdateTrainerAsync(int trainerId, UpdateTrainerViewModel model, CancellationToken ct = default);

        Task<Result> CreateTrainerAsync (CreateTrainerViewModel model, CancellationToken ct = default);

        Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default);













    }
}
