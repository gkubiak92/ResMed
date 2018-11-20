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
        public Visits Visit { get; set; }

        public MyVisitsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = GetActualLoggedPatientFromDb(user);

            var visits = await _db.Visits.Include(v => v.Doctor).Where(v => v.PatientId == patient.Id).OrderBy(d => d.Date).ToListAsync();


            return View(visits);
        }

        private Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}