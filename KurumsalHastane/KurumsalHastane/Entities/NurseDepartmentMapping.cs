using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalHastane.Entities
{
    public class NurseDepartmentMapping
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int NurseId { get; set; }
        [ForeignKey("NurseId")]
        public User Nurse { get; set; }


        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}
