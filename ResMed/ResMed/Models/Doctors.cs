using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
