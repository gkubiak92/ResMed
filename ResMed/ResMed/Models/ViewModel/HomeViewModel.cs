using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResMed.Models.ViewModel
{
    public class HomeViewModel
    {
        public string selectedSpec { get; set; }
        public SelectList SpecList { get; set; }
    }
}
