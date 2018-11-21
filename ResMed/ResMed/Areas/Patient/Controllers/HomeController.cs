using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResMed.Data;
using ResMed.Models;
using ResMed.Models.ViewModel;

namespace ResMed.Controllers
{
    [Area("Patient")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public VisitsViewModel VisitVM { get; set; }


        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            VisitVM = new VisitsViewModel();
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
        [Authorize(Roles = "PatientRole")]
        public async Task<IActionResult> BookDoc(int? id)
        {

            if (id == null)
                return NotFound();

            var doctor = await _db.Doctors.Include(m => m.Specializations).Where(m => m.Id == id).FirstOrDefaultAsync();
            
            VisitVM.Doctor = doctor;
            
            return View(VisitVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PatientRole")]
        public async Task<IActionResult> BookDoc(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = GetActualLoggedPatientFromDb(user);

            VisitVM.Visit.Date = VisitVM.Visit.Date
                                                .AddHours(VisitVM.Visit.Time.Hour)
                                                .AddMinutes(VisitVM.Visit.Time.Minute);
            VisitVM.Visit.DoctorId = id;
            VisitVM.Visit.PatientId = patient.Id;

            Visits vis = VisitVM.Visit;

            var visDateCheck = (from v in _db.Visits
                                where (v.DoctorId == vis.DoctorId
                                && v.Date == vis.Date)
                                select v);

            if(visDateCheck.Count() > 0)
            {
                TempData["Error"] = "Error";
                return RedirectToAction(nameof(BookDoc));
            }

            TempData["Error"] = "";
            _db.Visits.Add(vis);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "MyVisits");
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
