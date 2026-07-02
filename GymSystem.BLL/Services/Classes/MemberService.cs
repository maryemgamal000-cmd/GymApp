using AutoMapper;
using GymSystem.BLL.Common;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MemberViewModels;
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
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork ,IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // check if email exist
            var emailexist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            if (emailexist) return Result.Fail("Email Already Exists");

            // check if phone exist
            var phoneexist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);
            if (phoneexist) return Result.Fail("Phone Already Exists");

            // else true add member
            var member = _mapper.Map<CreateMemberViewModel, Member>(model);

            _unitOfWork.GetRepository<Member>().Add(member); //Add Locally
            var saveResult = await _unitOfWork.SaveChangesAsync(ct);
            return saveResult > 0 ? Result.Ok() : Result.Fail("Failed To Create Member");
        }



        public async Task<Result<IEnumerable<MemberViewModel>>> GetAllMemberAysnc(CancellationToken ct = default)
        {
            var members = await  _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!members.Any() || members == null) return Result< IEnumerable < MemberViewModel >>.NotFound("No Members Available");

            var memberViewModel = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);

            return Result< IEnumerable <MemberViewModel>>.Ok(memberViewModel) ;


        }

        public async Task<Result<MemberViewModel?>> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(MemberId, ct);

            if (member is null) return Result <MemberViewModel ?>.NotFound("Member Not Found");

            var model = _mapper.Map<Member , MemberViewModel>(member);   

            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(m => m.MemberId == MemberId && m.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(activeMembership.PlanId, ct);
                model.PlanName = activePlan?.Name;
                model.MembershipEndDate = activeMembership.EndDate.ToString();
                model.MembershipStartDate = activeMembership.CreatedAt.ToString();
            }


            return Result<MemberViewModel?>.Ok(model);

        }

        public async Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.MemberID == memberId, ct: ct);
            if (record is null)
            {
                return Result<HealthRecordViewModel?>.NotFound("Health Record Not Found For This Member");
            }
            else
            {
               var model =  _mapper.Map<HealthRecord , HealthRecordViewModel>(record);
                return Result<HealthRecordViewModel?>.Ok(model);    


        }
        }

        public async Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(memberId , ct);
            if (member is null) return Result<MemberToUpdateViewModel?>.NotFound("Member Not Found");
            else 
            {
                var model = _mapper.Map<Member, MemberToUpdateViewModel>(member);
                return Result<MemberToUpdateViewModel?>.Ok(model);
            }
        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(id, ct);
            if (member == null) return Result.NotFound("Member Not Found");

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id !=id);
            if (emailExists) return Result.Fail("Email is already registered by another member");

            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id);
            if (phoneExists) return Result.Fail("Phone is already registered by another member");

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;


             _unitOfWork.GetRepository<Member>().Update(member); //Update Locally
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Can Not Update Memeber");


        }

        public async Task<Result> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync (memberId, ct);
            if(member == null) return Result.NotFound("Member Not Found");

            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.StartDate > DateTime.Now, ct);
            if(hasFutureBookings ) return Result.Validation("Can Not Delete Member Has upcoming session bookings");
        
          _unitOfWork.GetRepository<Member>().Delete(member); //Delete Locally
            var result = await _unitOfWork.SaveChangesAsync(ct);     
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Memeber")     ;

        }
    }
}