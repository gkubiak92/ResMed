using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public VisitsViewModel VisitVM { get; set; }

        public MyVisitsController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;

            VisitVM = new VisitsViewModel()
            {
                Visit = new Visits(),
                Doctor = new Doctors(),
                Review = new Reviews()
            };
        }


        public async Task<IActionResult> Index(string hidePastVisits)
        {

            ViewBag.HidePastVisitsParm = string.IsNullOrEmpty(hidePastVisits) ? "hide_past_visits" : "";
            
            var user = await _userManager.GetUserAsync(User);

            var patient = GetActualLoggedPatientFromDb(user);


            IQueryable<Visits> visits = (from v in _db.Visits.Include(d => d.Doctor)
                                         where v.PatientId == patient.Id
                                         select v);

            switch(hidePastVisits)
            {
                case "hide_past_visits":
                    visits = visits.Where(v => v.Date >= DateTime.Today);
                    break;
                default:
                    break;
            }

            return View(visits.OrderByDescending(d => d.Date));
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
            var user = await _userManager.GetUserAsync(User);
            var patient = GetActualLoggedPatientFromDb(user);


            var visit = await _db.Visits.FindAsync(id);

            var docUser = GetDoctorUserFromDb(visit.DoctorId);
            string docEmail = docUser.Email;


            _db.Visits.Remove(visit);
            await _db.SaveChangesAsync();

            //mail do lekarza
            await _emailSender.SendEmailAsync(docEmail, $"Anulowanie wizyty dnia: {visit.Date.ToShortDateString()}",
                        $"Pacjent {patient.FirstName + " " + patient.LastName} anulował wizytę o godzinie: {visit.Date.TimeOfDay}");

            //mail do pacjenta z potwierdzeniem anulowania wizyty
            await _emailSender.SendEmailAsync(user.Email, $"Anulowanie wizyty",
                $"Pomyślnie anulowałeś wizytę dnia: {visit.Date.ToShortDateString()}");

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

            var vis = _db.Visits.Find(id);
            vis.IsReviewed = true;

            _db.Visits.Update(vis);
            _db.Reviews.Add(review);

            var doc = _db.Doctors.Find(VisitVM.Review.DoctorId);

            doc.RatingCount++;

            int sum = (from d in _db.Reviews
                      where d.DoctorId == doc.Id
                      select d.Rating).ToList().Sum();


            doc.AverageRating = (double)(sum + review.Rating) / doc.RatingCount;
            _db.Doctors.Update(doc);

            await _db.SaveChangesAsync();
            return (RedirectToAction(nameof(Index)));
        }

        private IdentityUser GetDoctorUserFromDb(int id)
        {
            var doc = _db.Doctors.Find(id);
            return _db.Users.FirstOrDefault(x => x.Id == doc.UserId);
        }

        private Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}