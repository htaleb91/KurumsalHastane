using KurumsalHastane.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace KurumsalHastane.Models.PatientModels
{
    public class PatientModel
    {
        public int Id { get; set; }

        [DisplayName("First Name")]
        [Required(ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is Required")]
        public string LastName { get; set; }

        [DisplayName("Role")]
        //[Required(ErrorMessage = " Choosing a Role is Required")]
       // [Range(1, int.MaxValue, ErrorMessage = "you Must Choose One Role")]
        public int RoleId { get; set; }
        [ValidateNever]
        public string RoleName { get => ((UserRole)RoleId).ToString(); }
        //public IList<SelectListItem> Roles { get; set; }

        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Telefon")]
        [Required(ErrorMessage = "Telefon is Required")]
        [Phone]
        public string Telefon { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password Field is Required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string PasswordHash { get; set; }

        [DisplayName("Re-Type-Password")]
        [Required(ErrorMessage = "Password Field is Required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [Compare(nameof(PasswordHash), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Picture")]
        public IFormFile File { get; set; }

        [ValidateNever]
        [DisplayName("Picture")]
        public string PicturePath { get; set; }
        public bool SubscribeToOurMonthlyNewsletter { get; set; }
        public bool EnableCustomNotificationsViaEmail { get; set; }
    }
}
