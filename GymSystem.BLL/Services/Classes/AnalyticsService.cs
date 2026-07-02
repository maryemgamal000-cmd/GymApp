using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.AnalyticsViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;

            var upcomingSessions = await _unitOfWork.GetRepository<Session>()
                .CountAsync(s => s.StartDate > now, ct);

            var ongoingSessions = await _unitOfWork.GetRepository<Session>()
                .CountAsync(s => s.StartDate <= now && s.EndDate >= now, ct);

            var completedSessions = await _unitOfWork.GetRepository<Session>()
                .CountAsync(s => s.EndDate < now, ct);

            var totalMembers = await _unitOfWork.GetRepository<Member>()
                .CountAsync(ct :ct);

            var totalTrainers = await _unitOfWork.GetRepository<Trainer>()
                .CountAsync(ct:ct);

            var activeMembers = await _unitOfWork.GetRepository<Membership>()
                .CountAsync(x => x.EndDate > now, ct);

            return new AnalyticsViewModel
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                CompletedSessions = completedSessions,
                OngoingSessions = ongoingSessions,
                UpcomingSessions = upcomingSessions,
                TotalTrainers = totalTrainers
            };
        }
    }
}
