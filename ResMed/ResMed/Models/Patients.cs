using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models
{
    public class Patients
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(Name ="Data urodzenia")]
        public DateTime Birthday { get; set; }

        [Required]
        public string Gender { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }
    }
}
