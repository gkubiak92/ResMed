using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResMed.Data;
using ResMed.Models;
using ResMed.Models.ViewModel;

namespace ResMed.Areas.Patient.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "DoctorRole")]
    public class MyPatientsController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Visits Visit { get; set; }

        public MyPatientsController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
        }


        public async Task<IActionResult> Index(string sortOrder, string patientSearch)
        {

            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            var user = await _userManager.GetUserAsync(User);

            var doctor = GetActualLoggedDoctorUserFromDb(user);

            IQueryable<Visits> visits = (from v in _db.Visits.Include(p => p.Patient)
                                         where v.DoctorId == doctor.Id
                                         select v);

            switch (sortOrder)
            {
                case "name_desc":
                    visits = visits.OrderByDescending(s => s.Patient.LastName);
                    break;
                case "Date":
                    visits = visits.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    visits = visits.OrderByDescending(s => s.Date);
                    break;
                default:
                    visits = visits.OrderBy(s => s.Patient.LastName);
                    break;
            }


            if (!string.IsNullOrEmpty(patientSearch))
            {
                visits = visits.Where(p => p.Patient.FirstName.Contains(patientSearch) ||
                                        p.Patient.LastName.Contains(patientSearch));
            }

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

            VisitVM.Visit = _db.Visits.Include(p => p.Patient).FirstOrDefault(i => id == i.Id);
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
        public async Task<ActionResult> Delete(int id, string reason)
        {
            var visit = await _db.Visits.FindAsync(id);


            var doc = GetActualLoggedDoctorFromDb(visit.DoctorId);

            var user = GetPatientUserFromDb(visit.PatientId);
            string patientMail = user.Email;

            await _emailSender.SendEmailAsync(patientMail, $"Anulowanie wizyty dnia: {visit.Date.ToShortDateString()}",
                        $"Lekarz {doc.FirstName + " " + doc.LastName} anulował wizytę o godzinie: {visit.Date.TimeOfDay} \n" +
                        $"Powód: {reason}");

            _db.Visits.Remove(visit);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private Doctors GetActualLoggedDoctorFromDb(int doctorId)
        { 
            return _db.Doctors.Find(doctorId);
        }

        private Doctors GetActualLoggedDoctorUserFromDb(IdentityUser user)
        {
            return _db.Doctors.FirstOrDefault(d => d.UserId == user.Id);
        }

        private IdentityUser GetPatientUserFromDb(int patientId)
        {
            var pat = _db.Patients.Find(patientId);
            return _db.Users.Find(pat.UserId);
        }

    }
}