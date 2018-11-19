using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models
{
    public class Visits
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [NotMapped]
        public DateTime Time { get; set; }

        public int DoctorId { get; set; } 

        public int PatientId { get; set; }

    }
}
