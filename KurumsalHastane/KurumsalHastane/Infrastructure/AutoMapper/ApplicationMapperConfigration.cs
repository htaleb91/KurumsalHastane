using AutoMapper;
using KurumsalHastane.Entities;
using KurumsalHastane.Models.AppointmentModels;
using KurumsalHastane.Models.DepartmentModels;
using KurumsalHastane.Models.MedicineModels;
using KurumsalHastane.Models.NurseModels;
using KurumsalHastane.Models.PatientModels;
using KurumsalHastane.Models.PrescriptionModels;
using KurumsalHastane.Models.SpecialistModels;
using KurumsalHastane.Models.User;

namespace KurumsalHastane.Infrastructure.AutoMapper
{
    public class ApplicationMapperConfigration : Profile
    {
       
        public ApplicationMapperConfigration()
        {
            AddMappers();
        }
        public void AddMappers()
        {
            //services.AddAutoMapper(typeof(UserModel), typeof(User));
            //services.AddAutoMapper(typeof(User), typeof(UserModel));
            CreateMap<User, UserModel>();
            CreateMap<UserModel, User>();

            CreateMap<User, PatientModel>();
            CreateMap<PatientModel, User>();
            

            CreateMap<User, SpecialistModel>();
            CreateMap<SpecialistModel, User>();

            CreateMap<User, NurseModel>();
            CreateMap<NurseModel, User>();

            CreateMap<Department, DepartmentModel>();
            CreateMap<DepartmentModel, Department>();

            CreateMap<Medicine, MedicineModel>()
                .ForMember(a=>a.Name , opt => opt.MapFrom(src=>src.FirstName))
                .ForMember(a => a.Desciption, opt => opt.MapFrom(src => src.Descriotion));
            CreateMap<MedicineModel, Medicine>()
                .ForMember(a => a.FirstName, opt => opt.MapFrom(src => src.Name))
                .ForMember(a => a.Descriotion, opt => opt.MapFrom(src => src.Desciption));

            CreateMap<Prescription, PrescriptionModel>();
            CreateMap<PrescriptionModel, Prescription>();

            CreateMap<Appointment, AppointmentModel>();
            CreateMap<AppointmentModel, Appointment>();

        }
        
    }
}
