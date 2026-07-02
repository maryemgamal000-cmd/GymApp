using AutoMapper;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Data.Models.Enums;
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
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork , IGenericRepository<Session> sessionRepository , IMapper mapper ) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task <Result<IEnumerable<SessionViewModel>?>> GetAllSessionsAsync(CancellationToken ct = default)
        {
          var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct);
            if (sessions == null || !sessions.Any())
                return Result<IEnumerable<SessionViewModel>?>.NotFound("No Sessions Available");
            else
            {
                var mappedSessions = _mapper.Map< IEnumerable<Session>, IEnumerable<SessionViewModel>>(sessions);


                foreach (var session in mappedSessions) 
                {
                    session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id, ct);
                    //N + 1 Problem

                }
                return Result<IEnumerable<SessionViewModel>?>.Ok(mappedSessions);  
            }
      
        }

        public async Task <Result<IEnumerable<CategorySelectViewModel>>> GetCategoryForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
            if (categories is null) return Result < IEnumerable < CategorySelectViewModel >>.NotFound("Categories Not Found");


            var mappedSelectCategories = _mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(categories);

            return Result<IEnumerable<CategorySelectViewModel>>.Ok(mappedSelectCategories);
        }
        public async Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainerForDropDownAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (trainers is null) return Result<IEnumerable<TrainerSelectViewModel>>.NotFound("Trainers Not Found"); ;


            var mappedSelectTrainers = _mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerSelectViewModel>>(trainers);
            return Result<IEnumerable<TrainerSelectViewModel>>.Ok(mappedSelectTrainers);
        }






        public async Task<Result<SessionViewModel>> GetSessionDetailsByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionByIdWitTrainerAndCategory(sessionId, ct);
            if(session is null) { return Result<SessionViewModel>.NotFound("Session not found"); }
          else
            {

               var mappedSession =  _mapper.Map<Session , SessionViewModel>(session);
                mappedSession.AvailableSlots=  mappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId , ct);
                return Result<SessionViewModel>.Ok(mappedSession);
            }
  
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIDAsync(sessionId);

            if(session == null) 
                return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if (session.EndDate < DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Cannot Edit Completed Session ");

            if (session.StartDate <= DateTime.Now)
                 return Result<UpdateSessionViewModel>.Fail("Cannot Update Session Has Already Started");



            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId);    
            if(bookingCount > 0 )
                return Result<UpdateSessionViewModel>.Fail("Cannot Update Session Has Booked");


            var mappedSession = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);

            
        }


        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("EndDate Must Be After StartDate");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Startdate Must Be In The Future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity Must Be Between 1 and 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIDAsync(model.CategoryId);
            if (category is null) return Result.NotFound("Category Not Found"); ;

            var isvalid = Enum.TryParse<Speciality>(category.CategoryName, true, out var categorySpeciality);
            if (!isvalid) return Result.Validation("Cannot Create This Session To This Trainer");

            var session = _mapper.Map<CreateSessionViewModel, Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session); //Add locally
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIDAsync(sessionId,ct);

            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now && session.EndDate >= DateTime.Now )
                return Result.Fail("Cannot Delete Session Has Not Ended Yet");

            if (session.StartDate > DateTime.Now)
                return Result.Fail("Cannot Delete Upcoming Session ");

            var bookedCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId,ct);
            if (bookedCount > 0)
                return Result.Fail("Cannot Delete Session Has Bookings");


            _unitOfWork.SessionRepository.Delete(session);   //Delete Locally
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Session");   
        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIDAsync(sessionId, ct: ct);
            if (session == null) 
                return Result.NotFound("Session not Found");


            if (session.StartDate <= DateTime.Now)
                return Result.Validation("Cannot Edit Session Has Already Started");


            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlots(sessionId);
            if (bookingCount > 0)
                return Result.Validation("Cannot Update Session Has Booked");





            if (model.EndDate <= model.StartDate) 
                return Result.Validation(" EndDate Must Be After StartDate");

            if (model.StartDate <= DateTime.Now) 
                return Result.Validation("Startdate Must Be In The Future");





            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(model.TrainerId);
            if (trainer is null) 
                return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIDAsync(session.CategoryId);
      

            var isvalid = Enum.TryParse<Speciality>(category?.CategoryName, true, out var categorySpeciality);
            if (!isvalid) return Result.Validation("Cannot Create This Session To This Trainer");


            var mappedSession = _mapper.Map(model , session);  
            session.UpdatedAt = DateTime.Now;   

            _unitOfWork.SessionRepository.Update(mappedSession);
            var result =await  _unitOfWork.SaveChangesAsync(ct);
            return result > 0  ? Result.Ok() : Result.Fail("Failed to Update Session");


         
        }
    }
}
