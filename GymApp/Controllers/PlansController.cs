
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Controllers
{
    public class PlansController : Controller
    {

        //dependency injection
        private readonly IGenericRepository<Plan> _planRepository;

        public PlansController(IGenericRepository<Plan> planRepository)
        {
            _planRepository = planRepository;
        }



        //Index Action (Default) ==> Get all plans
        //Get (BaseUrl/Plans)
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planRepository.GetAllAsync(ct:ct);
            return View(plans);
        }



        //Details Action
        //Get (BaseUrl/Plan/Details/id)
        public async Task<IActionResult>Details(int id , CancellationToken ct)
        {
            var plan = await _planRepository.GetByIDAsync(id,ct);

            if(plan is null )
                return NotFound();
            else
                return View(plan);
        }
    }
}
