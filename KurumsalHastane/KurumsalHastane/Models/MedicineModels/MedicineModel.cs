using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KurumsalHastane.Models.MedicineModels
{
    public class MedicineModel
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Department Name is Required")]
        public string Name { get; set; }

        [DisplayName("Desciption")]
        [Required(ErrorMessage = "Desciption is Required")]
        public string Desciption { get; set; }

        [DisplayName("Side Effects")]
        [Required(ErrorMessage = "Side Effects is Required")]
        public string SideEffects { get; set; }

        [DisplayName("Usage Way")]
        [Required(ErrorMessage = "Usage Way is Required")]
        public string UsageWay { get; set; }
    }
}
