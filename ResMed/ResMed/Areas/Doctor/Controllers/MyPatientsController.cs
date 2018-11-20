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


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var doctor = GetActualLoggedDoctorFromDb(user);

            var visits = await _db.Visits.Include(v => v.Patient).Where(v => v.DoctorId == doctor.Id).OrderBy(d => d.Date).ToListAsync();


            return View(visits);
        }

        private Doctors GetActualLoggedDoctorFromDb(IdentityUser user)
        {
            return _db.Doctors.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}