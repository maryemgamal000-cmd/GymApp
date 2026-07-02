using GymSystem.BLL.Common;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface IPlanService 
    {

        //GetAllPlans
        Task<Result<IEnumerable<PlanViewModel>?>> GetAllPlansAsync(CancellationToken ct = default);

        //GetPlanDetailsById
        Task<Result<PlanViewModel?>> GetPlanDetailsByIdAsync(int planId, CancellationToken ct = default);

        //EditPlan =2Actions
        Task<Result<PlanToUpdateViewModel?>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);
        Task<Result> UpdatePlanDetailsAsync(int planId, PlanToUpdateViewModel model, CancellationToken ct = default);


        //Active/Deactive
        Task<Result> ActivateOrDeactivatePlan(int planId, CancellationToken ct = default);
    }
}
