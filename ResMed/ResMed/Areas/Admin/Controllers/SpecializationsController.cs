using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResMed.Data;
using ResMed.Models;

namespace ResMed.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "AdminRole")]
    public class SpecializationsController : Controller
    {

        private readonly ApplicationDbContext _db;

        public SpecializationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //public IActionResult Index()
        //{
        //    var SpecializationsList = _db.Specializations.ToList().OrderBy(x => x.Name);
        //    return View(SpecializationsList);
        //}




        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IdSortParm"] = sortOrder == "Id" ? "id_desc" : "Id";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var students = from s in _db.Specializations
                           select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name);
                    break;
                case "id":
                    students = students.OrderBy(s => s.Id);
                    break;
                case "id_desc":
                    students = students.OrderByDescending(s => s.Id);
                    break;
                default:
                    students = students.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Specializations>.CreateAsync(students.AsNoTracking(), page ?? 1, pageSize));
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
            if (id != specializations.Id)
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