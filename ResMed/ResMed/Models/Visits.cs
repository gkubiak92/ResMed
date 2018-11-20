using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models
{
    public class Visits
    {
        public int Id { get; set; }

        [Display(Name = "Data wizyty")]
        public DateTime Date { get; set; }

        [NotMapped]
        [Display(Name = "Godzina wizyty")]
        public DateTime Time { get; set; }

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
