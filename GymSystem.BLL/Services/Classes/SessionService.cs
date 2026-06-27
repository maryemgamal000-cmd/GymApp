using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork , IGenericRepository<Session> sessionRepository) {
            _unitOfWork = unitOfWork;
          
        } 
        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
          var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct);
            if (sessions == null || !sessions.Any())
                return null;
            else
            {
                var mappedSessions = sessions.Select(s => new SessionViewModel() 
                { 
                Id = s.Id,  
                Capacity= s.Capacity,   
                StartDate = s.StartDate,    
                EndDate = s.EndDate,    
                Description = s.Description,    
                TrainerName=s.Trainer.Name,
                CategoryName=s.Category.CategoryName,
                
                });


                foreach (var session in mappedSessions) 
                {
                    session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id, ct);
                    //N + 1 Problem

                }
                return mappedSessions;  
            }
      
        }
    }
}
