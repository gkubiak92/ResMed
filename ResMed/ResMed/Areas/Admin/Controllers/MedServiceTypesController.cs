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
    public class MedServiceTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MedServiceTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.MedServiceTypes.ToList());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedServiceTypes medServiceTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Add(medServiceTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(medServiceTypes);
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medServiceType = await _db.MedServiceTypes.FindAsync(id);
            if (medServiceType == null)
            {
                return NotFound();
            }

            return View(medServiceType);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedServiceTypes medServiceTypes)
        {
            if (id != medServiceTypes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(medServiceTypes);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(medServiceTypes);
        }


        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medServiceType = await _db.MedServiceTypes.FindAsync(id);
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

            var medServiceType = await _db.MedServiceTypes.FindAsync(id);
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
            var medServiceTypes = await _db.MedServiceTypes.FindAsync(id);
            _db.MedServiceTypes.Remove(medServiceTypes);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}