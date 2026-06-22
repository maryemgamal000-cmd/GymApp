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

            var members = await _memberService.GetAllMemberAysnc();
            return View(members);


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
                var result =await _memberService.CreateMemberAsync(model, ct);

                if (result)
                    TempData["SuccessMessage"] = "Member createwd successfully";
                else
                    TempData["ErrorMessage"] = "Failed to create member";

                return RedirectToAction(nameof(Index));
            }
        }





    }
}