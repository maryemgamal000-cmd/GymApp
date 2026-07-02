using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ActivateOrDeactivatePlan(int planId, CancellationToken ct)
        {
           var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId,ct);
            if (plan == null)  return false; 

            var existActiveMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && (m.EndDate > DateTime.Now), ct);

            if (plan.IsActive && existActiveMembership)
            {
                return false;
            }
                else
                {
                    plan.IsActive = !plan.IsActive;
                    plan.UpdatedAt=DateTime.Now;
                    _unitOfWork.GetRepository<Plan>().Update(plan);
                    var result = await _unitOfWork.SaveChangesAsync(ct);
                    return result > 0;
                }

        
        }

        

        public async Task<IEnumerable<PlanViewModel>?> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);  
            if(plans == null) return null;
            var mappedPlans = plans.Select(p => new PlanViewModel() 
            { 
              Description = p.Description,  
              Price = p.Price,
              Name = p.Name,    
              IsActive = p.IsActive,    
              DurationDays = p.DurationDays,    
              Id = p.Id,
            });


            return mappedPlans;  
        }

        public async Task<PlanViewModel?> GetPlanDetailsByIdAsync(int planId, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId);
            if (plan == null) return null;

            var mappedPlan = new PlanViewModel()
            {
                Description = plan.Description,
                Price = plan.Price,
                Name = plan.Name,
                IsActive = plan.IsActive,
                DurationDays = plan.DurationDays,
                //Id = plan.Id
            };

            return mappedPlan;
        }

        public async Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId);

            if (plan == null || !plan.IsActive) return null;

            var existsActiveMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId &&m.EndDate>DateTime.Now);
            if (existsActiveMembership)

            { return null; }

            else
            {
                var mappedPlan = new PlanToUpdateViewModel()
                {
                    Price = plan.Price,
                    Description = plan.Description,
                    PlanName = plan.Name,
                    DurationDays = plan.DurationDays,

                };

                return mappedPlan;
            }

        }

        public async Task<bool> UpdatePlanDetailsAsync(int planId,PlanToUpdateViewModel model  ,CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, ct);
            if (plan == null) return false;

            var existsActiveMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now);
            if (existsActiveMembership)
            { return false; }
            else
            {
                
                plan.Description = model.Description;
                plan.Price = model.Price;
                plan.DurationDays = model.DurationDays;
                plan.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<Plan>().Update(plan);   //Update Locally
                var result = await _unitOfWork.SaveChangesAsync(ct);
                return result > 0;
            }


        }

       
    }
}
