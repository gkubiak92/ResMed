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
    public class VisitTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public VisitTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.VisitTypes.ToList());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitTypes VisitTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Add(VisitTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(VisitTypes);
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medServiceType = await _db.VisitTypes.FindAsync(id);
            if (medServiceType == null)
            {
                return NotFound();
            }

            return View(medServiceType);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VisitTypes VisitTypes)
        {
            if (id != VisitTypes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(VisitTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(VisitTypes);
        }


        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medServiceType = await _db.VisitTypes.FindAsync(id);
            if (medServiceType == null)
            {
                return NotFound();
            }

            return View(medServiceType);
        }


        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medServiceType = await _db.VisitTypes.FindAsync(id);
            if (medServiceType == null)
            {
                return NotFound();
            }

            return View(medServiceType);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var VisitTypes = await _db.VisitTypes.FindAsync(id);
            _db.VisitTypes.Remove(VisitTypes);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}