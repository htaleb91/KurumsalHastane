using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalHastane.Entities
{
    public class Medicine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string SideEffects { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Descriotion { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string UsageWay { get; set; }
    }
}
