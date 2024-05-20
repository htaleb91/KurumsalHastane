using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalHastane.Entities
{
    public class Appointment
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public User Patient { get; set; }


        public int SpecialistId { get; set; }
        [ForeignKey("SpecialistId")]
        public User Specialist { get; set; }


        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }


        public DateTime AppointnentDate { get; set; }

       
    }
}
