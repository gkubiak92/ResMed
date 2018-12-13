using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        [BindProperty]
        public Visits Visit { get; set; }

        public MyPatientsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(string sortOrder, string patientSearch)
        {

            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            var user = await _userManager.GetUserAsync(User);

            var doctor = GetActualLoggedDoctorFromDb(user);

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
        public async Task<ActionResult> Delete(int id)
        {
            var visit = await _db.Visits.FindAsync(id);

            _db.Visits.Remove(visit);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private Doctors GetActualLoggedDoctorFromDb(IdentityUser user)
        {
            return _db.Doctors.FirstOrDefault(x => x.UserId == user.Id);
        }


    }
}