using KurumsalHastane.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalHastane.Models.AppointmentModels
{
    public class AppointmentModel
    {

        public int Id { get; set; }

        [DisplayName("Patient")]
        public int PatientId { get; set; }

        [ValidateNever]
        public string PatientName { get; set; }

        [ValidateNever]
        public IList<SelectListItem> Patients { get; set; }



        [DisplayName("Specialist")]
        public int SpecialistId { get; set; }

        [ValidateNever]
        public string SpecialistName { get; set; }

        [ValidateNever]
        public IList<SelectListItem> Specialists { get; set; }



        [DisplayName("Department")]
        public int DepartmentId { get; set; }

        [ValidateNever]
        public string DepartmentName { get; set; }

        [ValidateNever]
        public IList<SelectListItem> Departments { get; set; }



        public DateTime AppointnentDate { get; set; }

    }
}
