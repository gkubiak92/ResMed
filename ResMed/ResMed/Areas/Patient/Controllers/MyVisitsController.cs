using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResMed.Data;
using ResMed.Models;
using ResMed.Models.ViewModel;

namespace ResMed.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class MyVisitsController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public VisitsViewModel VisitVM { get; set; }

        public MyVisitsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            VisitVM = new VisitsViewModel()
            {
                Visit = new Visits(),
                Doctor = new Doctors(),
                Review = new Reviews()
            };
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = GetActualLoggedPatientFromDb(user);

            var visits = await _db.Visits.Include(v => v.Doctor).Where(v => v.PatientId == patient.Id).OrderBy(d => d.Date).ToListAsync();


            return View(visits);
        }


        //GET - Anulowanie wizyty
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var VisitVM = new VisitsViewModel();

            VisitVM.Visit = _db.Visits.Find(id);
            VisitVM.Doctor = await _db.Doctors.Where(d => d.Id == VisitVM.Visit.DoctorId).FirstOrDefaultAsync();

            if (VisitVM == null)
            {
                return NotFound();
            }

            return View(VisitVM);
        }


        //POST - Anulowanie wizyty
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var visit = await _db.Visits.FindAsync(id);

            _db.Visits.Remove(visit);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET - Recenzja
        [HttpGet]
        public async Task<ActionResult> Review(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var patient = GetActualLoggedPatientFromDb(user);

            VisitVM.Visit = _db.Visits.Find(id);
            VisitVM.Doctor = await _db.Doctors.Where(d => d.Id == VisitVM.Visit.DoctorId).FirstOrDefaultAsync();
            VisitVM.Review.DoctorId = VisitVM.Doctor.Id;
            VisitVM.Review.PatientId = patient.Id;


            if (VisitVM == null)
            {
                return NotFound();
            }

            return View(VisitVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Review(int id)
        {

            if (!ModelState.IsValid)
            {
                return View(VisitVM);
            }

            var review = VisitVM.Review;

            //var vis = VisitVM.Visit;
            //vis.IsReviewed = true;

            var vis = _db.Visits.Find(id);
            vis.IsReviewed = true;

            _db.Visits.Update(vis);

            _db.Reviews.Add(review);
            //_db.Visits.Update(vis);

            await _db.SaveChangesAsync();
            return (RedirectToAction(nameof(Index)));
        }



        private Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}