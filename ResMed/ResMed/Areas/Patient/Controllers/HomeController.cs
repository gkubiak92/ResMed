using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResMed.Data;
using ResMed.Models;

namespace ResMed.Controllers
{
    [Area("Patient")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;


        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(string searchString)
        {
            return RedirectToAction(nameof(Search), new { id = searchString });
        }


        [HttpGet]
        public async Task<IActionResult> Search(string id)
        {
            var doctorList = await _db.Doctors.Include(m => m.Specializations).Where(m => m.Address == id).ToListAsync();
            
            return View(doctorList);
        }

        [HttpGet]
        public async Task<IActionResult> BookDoc(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _db.Doctors.Include(m => m.Specializations).Where(m => m.Id == id).FirstOrDefaultAsync();


            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> BookDoc(int id)
        {
            var doctor = await _db.Doctors.Include(m => m.Specializations).Where(m => m.Id == id).FirstOrDefaultAsync();

            var user = await _userManager.GetUserAsync(User);
            var userId = await _userManager.GetUserIdAsync(user);

            var patient = GetActualLoggedPatientFromDb(user);

            var visit = new Visits
            {
                Date = DateTime.Today,
                DoctorId = doctor.Id,
                PatientId = patient.Id
            };

            _db.Visits.Add(visit);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(x => x.UserId == user.Id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
