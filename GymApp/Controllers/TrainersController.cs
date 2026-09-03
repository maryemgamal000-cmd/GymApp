using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _trainerService.GetAllTrainersAsync(ct);
            return View(result.value);

        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);


            var result = await _trainerService.CreateTrainerAsync(model, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Trainer created successfully";
                return RedirectToAction(nameof(Index));
            }

            else
            {
                TempData["ErrorMessage"] = result.error;
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {

            var result = await _trainerService.GetTrainerDetailsAsync(id, ct);

            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index)); 
            }

            return View(result.value); 
        }

        [HttpGet]
        public async Task<IActionResult> Edit (int id, CancellationToken ct)
        {
            var result = await _trainerService.GetTrainerToUpdateAsync(id, ct);

            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

            return View(result.value);

        }

        [HttpPost]
        public async Task<IActionResult> Edit (int id ,UpdateTrainerViewModel model, CancellationToken ct)
        {

            if (!ModelState.IsValid) return View(model); 


            var result = await _trainerService.UpdateTrainerAsync(id, model, ct); 

            if (result.success) 
            {
                TempData["SuccessMessage"] = "Trainer updated successfully"; 
            }

            else 
            {
                TempData["ErrorMessage"] = result.error ;
            }
            return RedirectToAction(nameof(Index));


        }

        [HttpGet]
        public async Task<IActionResult> Delete (int id,  CancellationToken ct)
        {
            var result = await _trainerService.GetTrainerDetailsAsync(id, ct);

            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error ; 
                return RedirectToAction(nameof(Index));
            }

            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _trainerService.RemoveTrainerAsync(id, ct); 

            if (result.success) 
                TempData["SuccessMessage"] = "Trainer deleted successfully";

            else
                TempData["ErrorMessage"] = result.error;


            return RedirectToAction(nameof(Index)); 
        }


    }
}