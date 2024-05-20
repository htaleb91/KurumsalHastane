using entities= KurumsalHastane.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using KurumsalHastane.Entities;

namespace KurumsalHastane.Models.PrescriptionModels
{
    public class PrescriptionModel
    {


        public int Id { get; set; }

        [DisplayName("Prescription Guid")]
        public Guid PrescriptionGuid { get; set; }
        [DisplayName("Patient")]
        [Required(ErrorMessage = "Patient is Required")]
        public int PatientId { get; set; }


        [ForeignKey("PatientId")]
        [ValidateNever]
        public string PatientName { get; set; }

        [ValidateNever]
        public IList<SelectListItem> Patients { get; set; }

        [DisplayName("Medicines")]
        [Required(ErrorMessage = " At Least one Medicine is Required")]
        public IList<int> MedicineIds { get; set; }
        //public IList<entities.Medicine> Medicines { get; set; } 
        [ValidateNever]
        public IList<SelectListItem> Medicines { get; set; }
        [ValidateNever]
        public MultiSelectList MedicineList { get; set; }
    }
}
