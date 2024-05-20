using KurumsalHastane.Entities;
using Microsoft.EntityFrameworkCore;
using KurumsalHastane.Models.User;

namespace KurumsalHastane.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<NurseDepartmentMapping> NurseDepartmentMappings { get; set; }
        public DbSet<PersonelDepartmentMapping> PersonelDepartmentMappings { get; set; }
        public DbSet<SpecialistDepartmentMapping> SpecialistDepartmentMappings { get; set; }
        
        
    }
}
