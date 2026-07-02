
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }



        //Index Action (Default) ==> Get all plans
        //Get (BaseUrl/Plans)
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct);
            if (plans.success) return View(plans.value); 
            return View(new List<PlanViewModel>());

           
        }



        //Details Action
        //Get (BaseUrl/Plan/Details/id)
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _planService.GetPlanDetailsByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(result.value);
            }
        }



        //Get (Edit form) [BaseUEL/Plans/Edit/{id}]
        //Edit Action - Display Edit Form
        [HttpGet]
        public async Task<IActionResult> EditPlan(int id, CancellationToken ct)
        {
            var result = await _planService.GetPlanToUpdateAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(result.value);
            }

        }

        //Post [BaseUEL/Members/EditPlan/{plan}]
        //Edit Action - Submit the edit form
        [HttpPost]
        public async Task<IActionResult> EditPlan([FromRoute] int id, PlanToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _planService.UpdatePlanDetailsAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Plan Updated Successfully";

            }
            else
            {
                TempData["ErrorMessage"] = result.error;

            }
            return RedirectToAction(nameof(Index));
        }

        //Post [BaseUEL/Plans/{id}]
        //Activate OR Deactivate
        [HttpPost]
        public async Task<IActionResult> ChangePlanStatus (int id, CancellationToken ct)
        {
            var result = await _planService.ActivateOrDeactivatePlan(id, ct);
            if (result.success)
                
                TempData["SuccessMessage"] = "Plan Status Changed";
            else
                TempData["ErrorMessage"] = result.error;

            return RedirectToAction(nameof(Index));

        }



    }
}
