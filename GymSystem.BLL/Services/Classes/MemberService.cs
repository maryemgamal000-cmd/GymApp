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

        public MemberService(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }



        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // check if email exist
            var emailexist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);

            // check if phone exist
            var phoneexist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);

            // if exist return false
            if (phoneexist) return false;

            // else true add member
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    City = model.City,
                    Street = model.Street,
                    BuildingNumber = model.BuildingNumber
                },
                HealthRecord = new HealthRecord()
                {
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    Note = model.HealthRecordViewModel.Note,
                    BloodType = model.HealthRecordViewModel.BloodType
                }
            };

            _unitOfWork.GetRepository<Member>().Add(member); //Add Locally
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }



        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAysnc(CancellationToken ct = default)
        {
            var members = await  _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!members.Any()) return [];

            var memberViewModel = members.Select(m => new MemberViewModel()

            {

                Id = m.Id,
                Name = m.Name,
                Phone = m.Phone,
                Photo = m.Photo,
                Gender = m.Gender.ToString(),
                Email = m.Email,
            }


            );

            return memberViewModel;


        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(MemberId, ct);

            if (member is null) return null;

            var model = new MemberViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                Email = member.Email,
                DateOfBirth = member.DateOfBirth.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City} ",
                Gender = member.Gender.ToString()


            };

            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(m => m.MemberId == MemberId && m.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(activeMembership.PlanId, ct);
                model.PlanName = activePlan?.Name;
                model.MembershipEndDate = activeMembership.EndDate.ToString();
                model.MembershipStartDate = activeMembership.CreatedAt.ToString();
            }


            return model;

        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.MemberID == memberId, ct: ct);
            if (record is null)
            {
                return null;
            }
            else
            {
                return new HealthRecordViewModel()
                {
                    BloodType = record.BloodType,
                    Weight = record.Weight,
                    Height = record.Height,
                    Note = record.Note,
                };
                
        }
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(memberId , ct);
            if (member is null) return null;
            else
                return new MemberToUpdateViewModel()
                {
                    Name = member.Name,
                    Phone = member.Phone,
                    Photo=member.Photo,
                    City =member.Address.City,
                    BuildingNumber=member.Address.BuildingNumber,
                    Street=member.Address.Street,
                    Email=member.Email

                };

        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(id, ct);
            if (member == null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id !=id);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id);

            if (emailExists || phoneExists ) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;


             _unitOfWork.GetRepository<Member>().Update(member); //Update Locally
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;


        }

        public async Task<bool> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync (memberId, ct);
            if(member == null) return false;

            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.StartDate > DateTime.Now, ct);
            if(hasFutureBookings ) return false;
        
          _unitOfWork.GetRepository<Member>().Delete(member); //Delete Locally
            var result = await _unitOfWork.SaveChangesAsync(ct);     
            return result > 0;

        }
    }
}