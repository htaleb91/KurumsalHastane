using entities=KurumsalHastane.Entities;

namespace KurumsalHastane.Models.DashboardModels
{
    public class DashboardModel
    {
        
        public int SpecialtsCount { get; set; }
        public IList<entities.User> Specialists { get; set; }


        public int DepartmentsCount { get; set; }
        public IList<entities.Department> Departments { get; set; }


        public int PatientsCount { get; set; }
        public IList<entities.User> Patients { get; set; }

        public int AppointmentsCount { get; set; }
        public IList<entities.Appointment> Appointments { get; set; }
    }
}
