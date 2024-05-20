using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalHastane.Entities
{
    public class Prescription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid PrescriptionGuid { get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public User Patient { get; set; }

        public IList<int> MedicineIds { get; set; }
        public IList<Medicine> Medicines { get; set; }
    }
}
