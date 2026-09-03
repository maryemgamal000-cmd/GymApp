using AutoMapper;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.TrainerViewModels;
using GymSystem.DAL.Data.Models;
using GymSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

  

        public async Task<Result<IEnumerable<TrainerViewModel>?>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
      
            var mappedTrainers = _mapper.Map<IEnumerable<Trainer> , IEnumerable<TrainerViewModel>>(trainers);  
            
            return Result<IEnumerable<TrainerViewModel>?>.Ok(mappedTrainers);  
         
        }

        public async Task<Result<TrainerViewModel?>> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId ,ct);
            if (trainer is null) return Result<TrainerViewModel?>.NotFound("Trainer Not Found");
            else
            {
                var mappedtrainer = _mapper.Map<Trainer , TrainerViewModel>(trainer);   
                return Result<TrainerViewModel?>.Ok(mappedtrainer);   
            }
        }


        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {

            var isEmailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct);
            if (isEmailExists)
                return Result.Fail("This email address is already registered");
            

            var isPhoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct);
            if (isPhoneExists)
                return Result.Fail("This phone number is already registered");


            var mappedtrainer = _mapper.Map<Trainer>(model);


            _unitOfWork.GetRepository<Trainer>().Add(mappedtrainer);

            var saveResult = await _unitOfWork.SaveChangesAsync(ct);

            return saveResult > 0 ? Result.Ok() : Result.Fail("An error occurred while saving the trainer");


        }

        public async Task<Result<UpdateTrainerViewModel?>> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId, ct);

            if (trainer == null)
                return Result<UpdateTrainerViewModel?>.NotFound("Trainer Not Found");

            else
            {
                var updateTrainerViewModel = _mapper.Map<UpdateTrainerViewModel>(trainer);

                return Result<UpdateTrainerViewModel?>.Ok(updateTrainerViewModel);
            }
        }

        public async Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId, ct);
            if (trainer == null)
                return Result.NotFound($"Trainer Not Found");

            var hasFutureSessions = await _unitOfWork.SessionRepository.AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);
            if (hasFutureSessions)
                return Result.Fail("Cannot delete trainer currently assigned to active sessions");
            

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);

            var savedResult = await _unitOfWork.SaveChangesAsync(ct);

            return savedResult > 0 ? Result.Ok() : Result.Fail("An error occurred while deleting the trainer");
          
        }

        public async Task<Result> UpdateTrainerAsync(int trainerId, UpdateTrainerViewModel model, CancellationToken ct = default)
        {

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(trainerId, ct);
            if (trainer == null)
                return Result.NotFound("Trainer Not Found");
            

            var isEmailDuplicate = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct);
            if (isEmailDuplicate)
                return Result.Fail("This email address is already in use by another trainer");
 
            var isPhoneDuplicate = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct);
            if (isPhoneDuplicate)
                return Result.Fail("This phone number is already in use by another trainer");


            _mapper.Map(model, trainer);

            trainer.UpdatedAt = DateTime.Now; 

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var saveResult = await _unitOfWork.SaveChangesAsync(ct);
            return saveResult > 0 ? Result.Ok() : Result.Fail("No changes were made to the trainer details");

           
        }
    }
}
