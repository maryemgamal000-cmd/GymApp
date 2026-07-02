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
        Task<IEnumerable<PlanViewModel>?> GetAllPlansAsync(CancellationToken ct = default);

        //GetPlanDetailsById
        Task<PlanViewModel?> GetPlanDetailsByIdAsync(int planId, CancellationToken ct=default);

        //EditPlan =2Actions
        Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);
        Task<bool> UpdatePlanDetailsAsync(int planId, PlanToUpdateViewModel model, CancellationToken ct = default);


        //Active/Deactive
        Task<bool> ActivateOrDeactivatePlan( int planId ,CancellationToken ct = default);
    }
}
