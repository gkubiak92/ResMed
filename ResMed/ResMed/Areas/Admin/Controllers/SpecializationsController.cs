using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResMed.Data;

namespace ResMed.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecializationsController : Controller
    {

        private readonly ApplicationDbContext _db;

        public SpecializationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var SpecializationsList = _db.Specializations.ToList();
            return View(SpecializationsList);
        }
    }
}