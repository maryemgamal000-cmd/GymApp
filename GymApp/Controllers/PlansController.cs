
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Controllers
{
    public class PlansController : Controller
    {
        //private readonly GymDbContext dbContext;

        //public PlansController()
        //{
        //    dbContext = new GymDbContext();
        //}

        //dependency injection
        private readonly IPlanRepository planRepository;

        public PlansController(IPlanRepository repository)
        {
            this.planRepository = repository;
        }



        //Index Action (Default) ==> Get all plans
        //Get (BaseUrl/Plans)
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct:ct);
            return View(plans);
        }



        //Details Action
        //Get (BaseUrl/Plan/Details/id)
        public async Task<IActionResult>Details(int id , CancellationToken ct)
        {
            var plan = await planRepository.GetByIDAsync(id,ct);

            if(plan is null )
                return NotFound();
            else
                return View(plan);
        }
    }
}
