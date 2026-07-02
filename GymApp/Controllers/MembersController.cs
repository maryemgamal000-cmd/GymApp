using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MemberViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }


        //Get (All) [BaseUEL/Members/Index]
        //Index Action => List All Members
        public async Task<IActionResult> Index()
        {

            var result = await _memberService.GetAllMemberAysnc();
            if (result.success) return View(result.value);
            else
            {
                TempData["ErrorMessage"]=result.error;
                return View(Enumerable.Empty<MemberViewModel>());
            }
        }



        //Get (Creation form) [BaseUEL/Members/Create]
        //Create Action => Create a member
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        //Post 
        //Create Action => Submit the member
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {

            if (!ModelState.IsValid)
            {
                return View(nameof(Create), model);
            }
            else
            {
                var result = await _memberService.CreateMemberAsync(model, ct);

                if (result.success)
                    TempData["SuccessMessage"] = "Member created successfully";
                else
                    TempData["ErrorMessage"] = result.error;

                return RedirectToAction(nameof(Index));
            }
        }


        //Get BaseUrL/Members/MemberDetails/{id}
        //MemberDetails - show one member's details
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

            return View(result.value);

        }

        //Get BaseUrL/Members/HealthRecordDetails/{id}
        //HealthRecordDetails - show one member's HealthRecordDetails

        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {

            var result = await _memberService.GetMemberHealthRecordAsync(id, ct);

            if (!result.success)
            {

                TempData["Errormessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(result.value);
            }

        }


        //Get (Edit form) [BaseUEL/Members/EditMember/{id}]
        //Edit Action - Display Edi Form
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberToUpdateAsync(id, ct);
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

        //Post [BaseUEL/Members/EditMember/{member}]
        //Edit Action - Submit the edit form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if (result.success)

                TempData["SuccessMessage"] = "Member updated successfully";
            else

                TempData["ErrorMessage"] = result.error;

            return RedirectToAction(nameof(Index));

        }



        //Get [BaseUEL/Members/Delete/{id}]
        //Delete - Show Form
        [HttpGet]
        public async Task<IActionResult> Delete (int id , CancellationToken ct) 
        {
          var result = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if(!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        //Post [BaseUEL/Members/DeleteConfirmed/{id}]
        //DeleteConfirmed - Submit Form
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed ([FromRoute]int id , CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            if(result.success)
                TempData["SuccessMessage"]= "Member Deleted successfully";
           
            else
            
                TempData["ErrorMessage"]= result.error;
          
            return RedirectToAction(nameof(Index));
        }

    }
}