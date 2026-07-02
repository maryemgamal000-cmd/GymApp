using AutoMapper;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> ActivateOrDeactivatePlan(int planId, CancellationToken ct)
        {
           var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId,ct);
            if (plan == null)  return Result.NotFound("Plan Not Found"); 

            var existActiveMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && (m.EndDate > DateTime.Now), ct);

            if (plan.IsActive && existActiveMembership)
            {
                return Result.Fail("Can Not Deactivate Plan Has Active Members");
            }
                else
                {
                    plan.IsActive = !plan.IsActive;
                    plan.UpdatedAt=DateTime.Now;
                    _unitOfWork.GetRepository<Plan>().Update(plan);
                    var result = await _unitOfWork.SaveChangesAsync(ct);
                    return result > 0 ? Result.Ok(): Result.Fail("Can Not Change Plan Status"); ;
                }

        
        }


        
        public async Task<Result<IEnumerable<PlanViewModel>?>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);  
            if(plans == null || !plans.Any()) return Result<IEnumerable<PlanViewModel>?>.NotFound("No Plans Available");


            var mappedPlans = _mapper.Map<IEnumerable<Plan>, IEnumerable<PlanViewModel>>(plans);

            return Result<IEnumerable<PlanViewModel>?>.Ok(mappedPlans);  
        }

        public async Task<Result<PlanViewModel?>> GetPlanDetailsByIdAsync(int planId, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId);
            if (plan == null) return Result<PlanViewModel?>.NotFound("Plan not Found");

            var mappedPlan = _mapper.Map<Plan , PlanViewModel>(plan);

            return Result<PlanViewModel?>.Ok(mappedPlan);
        }

        public async Task<Result<PlanToUpdateViewModel?>> GetPlanToUpdateAsync(int planId, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId);

            if (plan == null) return Result <PlanToUpdateViewModel ?>.NotFound("Plan Not Found");

            if (!plan.IsActive)
                return Result<PlanToUpdateViewModel?>.Fail("Can Not Edit Inactive Plan ");

            var existsActiveMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId &&m.EndDate>DateTime.Now);
            if (existsActiveMembership)

            { return Result<PlanToUpdateViewModel?>.Fail("Can Not Edit Plan Has Active Members"); }

            else
            {
                var mappedPlan = _mapper.Map<Plan, PlanToUpdateViewModel>(plan);              

                return Result<PlanToUpdateViewModel?>.Ok(mappedPlan);
            }

        }

        public async Task<Result> UpdatePlanDetailsAsync(int planId,PlanToUpdateViewModel model  ,CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, ct);
            if (plan == null) return Result.NotFound("Plan Not Found");

            var existsActiveMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now);
            if (existsActiveMembership)
            { return Result.Validation("Can Not Edit Plan Has Active Members"); }
            else
            {
              _mapper.Map(model, plan);

              plan.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<Plan>().Update(plan);   //Update Locally
                var result = await _unitOfWork.SaveChangesAsync(ct);
                return result > 0 ? Result.Ok() : Result.Fail("Can Not Edit Plan");
            }


        }

       
    }
}
