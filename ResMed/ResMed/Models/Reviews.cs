using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models
{
    public class Reviews
    {
        public int Id { get; set; }

        [Display(Name = "Treść")]
        public string Text { get; set; }

        [Required]
        [Display(Name = "Ocena")]
        [Range(1,5, ErrorMessage = "Ocena musi być w przedziale od 1 do 5")]
        public int Rating { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [Display(Name = "Lekarz")]
        public virtual Doctors Doctor { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [Display(Name = "Pacjent")]
        public virtual Patients Patient { get; set; }
    }
}
