using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ResMed.Data;

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
    }
}