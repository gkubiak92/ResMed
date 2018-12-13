using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResMed.Data;
using ResMed.Models;

namespace ResMed.Areas.Doctor.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="AdminRole")]
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

        public IActionResult Create()
        {
            return View();
        }


        
        //GET - EDIT specjalization
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _db.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }


        //POST - EDIT specjalization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctors doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(doctor);
        }

        //GET - Details specjalization
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _db.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
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

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _db.Doctors.FindAsync(id);
            _db.Doctors.Remove(doctor);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}