using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models.ViewModel
{
    public class DoctorsViewModel
    {
        public Doctors Doctors { get; set; }
        public IEnumerable<Specializations> Specializations { get; set; }
    }
}
