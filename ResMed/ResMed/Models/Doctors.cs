using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models
{
    public class Doctors
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Aktywny")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Numer licencji")]
        public string LicenseNr { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        public string Image { get; set; }

        [Display(Name = "Id Specjalizacji")]
        public int? SpecializationId { get; set; }

        [ForeignKey("SpecializationId")]
        public virtual Specializations Specializations { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }
    }
}
