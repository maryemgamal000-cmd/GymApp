using GymSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GymSystem.PL.Controllers
{
    public class SessionController : Controller 
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index(CancellationToken ct) 
        {

          var sessions  = await _sessionService.GetAllSessionsAsync(ct);

            return View(sessions);


        }
    }
}
