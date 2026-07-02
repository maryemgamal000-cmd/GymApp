using AutoMapper;
using GymSystem.BLL.ViewModels.MemberViewModels;
using GymSystem.BLL.ViewModels.PlanViewModels;
using GymSystem.BLL.ViewModels.SessionViewModels;
using GymSystem.BLL.ViewModels.TrainerViewModels;
using GymSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {

            MapMember();
            MapSession();
            MapPlan();
            MapTrainer();

        }



        private void MapMember()
        {

            CreateMap<Member, MemberViewModel>()
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"));
            //Dest = MemberviewModel
            //Src = Member


            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City));
            //Dest = MemberToUpdateviewModel
            //Src = Member


            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                });



            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
                {
                    BuildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));

        }


        private void MapSession() {

            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<Category , CategorySelectViewModel>();

            CreateMap<Trainer, TrainerSelectViewModel>();

            CreateMap<Session, SessionViewModel>()
            .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<UpdateSessionViewModel, Session>().ReverseMap();

       
                

        }

        private void MapPlan()
        {
            CreateMap<Plan, PlanViewModel>();

            CreateMap<Plan, PlanToUpdateViewModel>()
               .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name));


            CreateMap<PlanToUpdateViewModel, Plan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.PlanMember, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        }


        private void MapTrainer()
        {
            CreateMap<Trainer, TrainerViewModel>()
               .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Speciality.ToString()))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")));



            CreateMap<CreateTrainerViewModel, Trainer>()
                  .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                  {
                      BuildingNumber = src.BuildingNumber,
                      Street = src.Street,
                      City = src.City
                  }));



            CreateMap<Trainer, UpdateTrainerViewModel>()
               .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber))
               .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City));



            CreateMap<UpdateTrainerViewModel, Trainer>()
              .ForMember(dest => dest.Name, opt => opt.Ignore())
              .AfterMap((src, dest) =>
              {
                  dest.Address.BuildingNumber = src.BuildingNumber;
                  dest.Address.Street = src.Street;
                  dest.Address.City = src.City;
              });
        }

    }


}
