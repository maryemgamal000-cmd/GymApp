using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MemberViewModels;
using GymSystem.DAL.Data.Models;
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

        private readonly IGenericRepository<Member> _memberRepository;

        public MemberService(IGenericRepository<Member> memberRepository) {
            _memberRepository = memberRepository;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // check if email exist
            var emailexist = await _memberRepository.AnyAsync(x => x.Email == model.Email, ct);

            // check if phone exist
            var phoneexist = await _memberRepository.AnyAsync(x => x.Phone == model.Phone, ct);

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
                    BloodType=model.HealthRecordViewModel.BloodType
                }
            };

            var result = await _memberRepository.AddAsync(member);
            return result > 0;
        }



        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAysnc(CancellationToken ct = default)
        {
            var members = await  _memberRepository.GetAllAsync(ct:ct);

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
    }
}
