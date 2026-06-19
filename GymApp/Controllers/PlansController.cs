using GymApp.DBContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymApp.Controllers
{
    public class PlansController : Controller
    {
        private readonly GymDbContext dbContext;

        public PlansController()
        {
            dbContext = new GymDbContext();
        }


        //Index Action (Default) ==> Get all plans
        //Get (BaseUrl/Plans)
        public async Task<IActionResult> Index()
        {

            var plans = await dbContext.Plans.ToListAsync();    
            return View(plans);
        }



        //Details Action
        //Get (BaseUrl/Plan/Details/id)
        public async Task<IActionResult>Details(int id)
        {
            var plan = await dbContext.Plans.FindAsync(id);

            if(plan is null )
                return NotFound();
            else
                return View(plan);
        }
    }
}
