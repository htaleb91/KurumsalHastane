using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalHastane.Entities
{
    public class Department
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Desciption { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string PicturePath { get; set; }
    }
}
