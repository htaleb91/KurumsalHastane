using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KurumsalHastane.Models.DepartmentModels
{
    public class DepartmentModel
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Department Name is Required")]
        public string Name { get; set; }

        [DisplayName("Desciption")]
        [Required(ErrorMessage = "Desciption is Required")]
        public string Desciption { get; set; }



        [DisplayName("Picture")]
        public IFormFile File { get; set; }

        [ValidateNever]
        [DisplayName("Picture")]
        public string PicturePath { get; set; }
    }
}
