using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResMed.Data;
using ResMed.Models;
using ResMed.Models.ViewModel;

namespace ResMed.Controllers
{
    [Area("Patient")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public VisitsViewModel VisitVM { get; set; }


        public HomeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            VisitVM = new VisitsViewModel();
        }

        [HttpGet]
        public IActionResult Index()
        {
            /* Przy wczytaniu strony głownej pobieramy listę specjalizacji i zapisujemy ją do selectlisty
             aby użyć jej w liście rozwijanej w razor page */
            var specializationsList = _db.Specializations.OrderBy(s => s.Name)
                           .Select(x => new { x.Id, Value = x.Name });


            var model = new HomeViewModel();
            model.SpecList = new SelectList(specializationsList, "Value", "Value");

            return View(model);
        }


        [HttpPost]
        public IActionResult Index(string searchString, string spec)
        {
            return RedirectToAction(nameof(Search), new { id = searchString, spec });
        }


        [HttpGet]
        public async Task<IActionResult> Search(string id, string spec)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(spec))
            {
                var doctorPlaceSpecList = await _db.Doctors.Include(m => m.Specializations)
                                                    .Where(m => m.Address.Contains(id))
                                                    .Where(s => s.Specializations.Name == spec)
                                                    .ToListAsync();
                return View(doctorPlaceSpecList);
            }

            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(spec))
            {
                var doctorSpecOnlyList = await _db.Doctors.Include(m => m.Specializations)
                                                    .Where(s => s.Specializations.Name == spec)
                                                    .ToListAsync();
                return View(doctorSpecOnlyList);
            }

            if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(spec))
            {
                var doctorPlaceOnlyList = await _db.Doctors.Include(m => m.Specializations)
                                                    .Where(m => m.Address.Contains(id))
                                                    .ToListAsync();
                return View(doctorPlaceOnlyList);
            }


            var doctorList = await _db.Doctors.Include(m => m.Specializations)
                                                    .ToListAsync();
            return View(doctorList);
        }


        [HttpGet]
        public async Task<ActionResult> Calendar(int id, string Date)
        {
            var model = await _db.Doctors.Include(m => m.Specializations).Where(m => m.Id == id).FirstOrDefaultAsync();

            model.SelectedDate = DateTime.Parse(Date);

            var takenHoursList = (from v in _db.Visits
                                  where (v.DoctorId == model.Id
                                  && v.Date.Day == model.SelectedDate.Day
                                  && v.Date.Month == model.SelectedDate.Month
                                  && v.Date.Year == model.SelectedDate.Year)
                                  select v.Date.TimeOfDay.ToString(@"hh\:mm")).ToList();

            model.TakenHoursInDay = takenHoursList;

            //Pętla generująca listę stringów odnośnie dostępnych godzin dla danego doktora
            int IntervalParameter = model.AverageVisitTime;
            TimeSpan SpanHours = model.EndWorkHours.TimeOfDay - model.StartWorkHours.TimeOfDay;

            // .Range(0, 1440 / IntervalParameter) - see Zohar Peled's comment -  
            // is brief, but less readable
            List<string> query = Enumerable
              .Range((int)model.StartWorkHours.TimeOfDay.TotalMinutes / IntervalParameter, (int)(SpanHours.TotalMinutes / IntervalParameter))
              .Select(i => DateTime.Today
                 .AddMinutes(i * (double)IntervalParameter) // AddHours is redundant
                 .ToString("HH:mm"))                        // Let's provide HH:mm format 
              .ToList();

            model.WorkHours = query;
            ///////////////////////////////////////////////////////////////////////////////////////////////
            return PartialView("CalendarPartialView", model);
        }


        [HttpGet]
        [Authorize(Roles = "PatientRole")]
        public async Task<IActionResult> BookDoc(int? id, string selectedDate)
        {

            if (id == null)
                return NotFound();

            var doctor = await _db.Doctors.Include(m => m.Specializations).Where(m => m.Id == id).FirstOrDefaultAsync();

            VisitVM.Doctor = doctor;



            //DateTime dateFromView;
            //if (!string.IsNullOrEmpty(selectedDate))
            //{
            //    dateFromView = DateTime.Parse(selectedDate);
            //}
            //else
            //{
            //    dateFromView = new DateTime(2018,11,30);
            //}

            //var takenHoursList = (from v in _db.Visits
            //                      where (v.DoctorId == doctor.Id
            //                      && v.Date.Day == dateFromView.Day
            //                      && v.Date.Month == dateFromView.Month
            //                      && v.Date.Year == dateFromView.Year)
            //                      select v.Date.TimeOfDay.ToString(@"hh\:mm")).ToList();

            //VisitVM.Doctor.TakenHoursInDay = takenHoursList;




            ////Pętla generująca listę stringów odnośnie dostępnych godzin dla danego doktora
            //int IntervalParameter = doctor.AverageVisitTime;
            //TimeSpan SpanHours = doctor.EndWorkHours.TimeOfDay - doctor.StartWorkHours.TimeOfDay;

            //// .Range(0, 1440 / IntervalParameter) - see Zohar Peled's comment -  
            //// is brief, but less readable
            //List<string> query = Enumerable
            //  .Range((int)doctor.StartWorkHours.TimeOfDay.TotalMinutes / IntervalParameter, (int)(SpanHours.TotalMinutes / IntervalParameter))
            //  .Select(i => DateTime.Today
            //     .AddMinutes(i * (double)IntervalParameter) // AddHours is redundant
            //     .ToString("HH:mm"))                        // Let's provide HH:mm format 
            //  .ToList();

            //VisitVM.Doctor.WorkHours = query;
            /////////////////////////////////////////////////////////////////////////////////////////////////


            //tablica logiczna zawierająca dane true/false czy dany dzień jest pracujący u danego lekarza
            bool[] workingDayArr = {VisitVM.Doctor.WorkingSunday,
                                        VisitVM.Doctor.WorkingMonday,
                                        VisitVM.Doctor.WorkingTuesday,
                                        VisitVM.Doctor.WorkingWednesday,
                                        VisitVM.Doctor.WorkingThursday,
                                        VisitVM.Doctor.WorkingFriday,
                                        VisitVM.Doctor.WorkingSaturday,
                                        };

            //tablica int'ów na potrzeby numerków w javascript
            int[] finalArr = new int[7];


            /*pętla, która sprawdza czy dany dzień jest pracujący, jeśli tak to w danym miejscu w tablicy intów wpisuje JEGO NUMER
            Dla poniedzałku jest to np.1, dla środy 3, dla niedzieli 0 (bo ten kalendarz domyślnie zaczyna się od niedzieli -.-).
            Jeśli fałsz to w miejsce danego dnia wpisujemy -1*/
            for (int i = 0; i < workingDayArr.Length; i++)
            {
                if (workingDayArr[i] == true)
                    finalArr[i] = i;
                else
                    finalArr[i] = -1; //-1 ponieważ w datepickerze nie ma takiego dnia tygodnia. Dni tygodnia to kolejno liczby od 0 do 6
            }

            //inicjalizacja tabeli intów w MODELU Doktora tak aby nie był null
            VisitVM.Doctor.WorkDaysArr = new int[7];

            //kopiowanie wynikowej tablicy do tablicy modelu
            finalArr.CopyTo(VisitVM.Doctor.WorkDaysArr, 0);

            return View(VisitVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "PatientRole")]
        public async Task<IActionResult> BookDoc(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var patient = GetActualLoggedPatientFromDb(user);

            VisitVM.Visit.Date = VisitVM.Visit.Date
                                                .AddHours(VisitVM.Visit.Time.Hour)
                                                .AddMinutes(VisitVM.Visit.Time.Minute);
            VisitVM.Visit.DoctorId = id;
            VisitVM.Visit.PatientId = patient.Id;

            Visits vis = VisitVM.Visit;

            var visDateCheck = (from v in _db.Visits
                                where (v.DoctorId == vis.DoctorId
                                && v.Date == vis.Date)
                                select v);

            if (visDateCheck.Count() > 0)
            {
                TempData["Error"] = "Error";
                return RedirectToAction(nameof(BookDoc));
            }

            vis.IsReviewed = false;

            TempData["Error"] = "";
            _db.Visits.Add(vis);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "MyVisits");
        }

        private Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(x => x.UserId == user.Id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
