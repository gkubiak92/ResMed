using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResMed.Data;
using ResMed.Models;

namespace ResMed.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecializationsController : Controller
    {

        private readonly ApplicationDbContext _db;

        public SpecializationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var SpecializationsList = _db.Specializations.ToList().OrderBy(x => x.Name);
            return View(SpecializationsList);
        }


        //GET - CREATE specjalization
        public IActionResult Create()
        {
            return View();
        }


        //POST - CREATE specjalization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specializations specializations)
        {
            if (ModelState.IsValid)
            {
                _db.Specializations.Add(specializations);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specializations);
        }

        //GET - EDIT specjalization
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var specialization = await _db.Specializations.FindAsync(id);

            if (specialization == null)
                return NotFound();

            return View(specialization);
        }


        //POST - EDIT specjalization
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Specializations specializations)
        {
            if(id != specializations.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Specializations.Update(specializations);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specializations);
        }

        //GET - Details specjalization
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var specialization = await _db.Specializations.FindAsync(id);

            if (specialization == null)
                return NotFound();

            return View(specialization);
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialization = await _db.Specializations.FindAsync(id);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var specialization = await _db.Specializations.FindAsync(id);
            _db.Specializations.Remove(specialization);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}