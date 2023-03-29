using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Lab6_NET.Models
{
    public class Student
    {
        [SwaggerSchema(ReadOnly = true)]
        public Guid ID{get;set;}
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName{get;set;}

        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName{get;set;}

        [Display(Name = "Program")]
        public string Program{get;set;}

    }
}
