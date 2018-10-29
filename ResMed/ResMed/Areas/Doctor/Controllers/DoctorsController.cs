using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResMed.Data;

namespace ResMed.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DoctorsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Doctors.ToList());
        }

        public async Task<IActionResult> MyCart()
        {
            var id = HttpContext.User.Identity.Name;

            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _db.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }
    }
}