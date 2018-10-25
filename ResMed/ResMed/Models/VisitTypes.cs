using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models
{
    public class VisitTypes
    {
        public int Id { get; set; }
        [Required]
        [Display(Name="Nazwa")]
        public string Name { get; set; }
    }
}
