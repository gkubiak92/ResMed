using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models.ViewModel
{
    public class VisitsViewModel
    {
        public Visits Visit { get; set; }
        public Doctors Doctor { get; set; }
        public Reviews Review { get; set; }
        public IEnumerable<Reviews> ReviewsList { get; set; }
    }
}
