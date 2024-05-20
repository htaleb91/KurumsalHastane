using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KurumsalHastane.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string LastName { get; set; }

        public int RoleId { get; set; }

        public string Email { get; set; }

        [Column(TypeName = "nvarchar(11)")]
        public string Telefon { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string PasswordHash { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string PicturePath { get; set; }
        public bool SubscribeToOurMonthlyNewsletter { get; set; }
        public bool EnableCustomNotificationsViaEmail { get; set; }


    }
}
