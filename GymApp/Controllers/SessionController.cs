using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.CodeAnalysis.Operations;
using System.Threading.Tasks;

namespace GymSystem.PL.Controllers
{
    public class SessionController : Controller 
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct) 
        {

          var result  = await _sessionService.GetAllSessionsAsync(ct);

            if (result.success)
                return View(result.value);
            else
            {
                return View(new List<SessionViewModel>());
            }


        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {

            await PopulateDropDownListAsync();
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {

            if (!ModelState.IsValid) 
            {
                await PopulateDropDownListAsync();
                return View(model);
            }

            var result = await _sessionService.CreateSessionAsync( model, ct);
            if(result.success) 
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }

            else 
            {
                TempData["ErrorMessage"] = result.error;
                await PopulateDropDownListAsync();
                return View(model);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Details (int id , CancellationToken ct) 
        {
          var result = await _sessionService.GetSessionDetailsByIdAsync(id);
            if (result.success)
            {
                return View(result.value);

            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index)); 
            }
        
        
        }


        [HttpGet]
        public async Task<IActionResult> Edit (int  id , CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id , ct);
            if (result.success) 
            {
                var trainerList = await _sessionService.GetTrainerForDropDownAsync(ct);
                var trainersData = trainerList.success ? trainerList.value : Enumerable.Empty<TrainerSelectViewModel>();
           
                ViewBag.Trainers = new SelectList(trainersData, "Id", "Name");
                 
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"]=result.error;  
                return RedirectToAction(nameof (Index));        
            }
        }



        [HttpPost]
        public async Task<IActionResult> Edit (int id, UpdateSessionViewModel model ,CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var trainerList = await _sessionService.GetTrainerForDropDownAsync(ct);
                var trainersData = trainerList.success ? trainerList.value : Enumerable.Empty<TrainerSelectViewModel>();
                ViewBag.Trainers =  new SelectList(trainersData, "Id", "Name");
                return View(model);

            }


            var result = await  _sessionService.UpdateSessionAsync(id, model, ct);
            if (result.success) 
            {
                TempData["SuccessMessage"] = "Session Updated";
                return RedirectToAction(nameof(Index));    
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                var trainerList = await _sessionService.GetTrainerForDropDownAsync(ct);
                var trainersData = trainerList.success ? trainerList.value : Enumerable.Empty<TrainerSelectViewModel>();
                ViewBag.Trainers = new SelectList(trainersData, "Id", "Name");
                return View(model);
            }



        }



        [HttpGet]
        public async Task<IActionResult> Delete (int id , CancellationToken ct)
        {
            var result = await _sessionService.GetSessionDetailsByIdAsync(id);
            if (result.success)
            {
                return View(result.value);
               
            }
            else 
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }    
        }


        [HttpPost]  
        public async Task<IActionResult> DeleteConfirmed (int id , CancellationToken ct)
        { 
            var result = await _sessionService.RemoveSessionAsync(id, ct);

            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Session Deleted Successfully" : result.error;
            return RedirectToAction(nameof(Index));


        }










        private async Task PopulateDropDownListAsync() 
        {
            var trainerList = await _sessionService.GetTrainerForDropDownAsync();
            var trainersData = trainerList.success ? trainerList.value : Enumerable.Empty<TrainerSelectViewModel>();
            ViewBag.Trainers =  new SelectList(trainersData, "Id", "Name");

            var categoriesList = await _sessionService.GetCategoryForDropDownAsync();
            var categoriesData = categoriesList.success ? categoriesList.value : Enumerable.Empty<CategorySelectViewModel>();
            ViewBag.Categories =  new SelectList(categoriesData, "Id", "CategoryName");

        }


    }
}
