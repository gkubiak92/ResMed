using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public VisitsViewModel VisitVM { get; set; }

        public MyVisitsController(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;

            VisitVM = new VisitsViewModel()
            {
                Visit = new Visits(),
                Doctor = new Doctors(),
                Review = new Reviews()
            };
        }


        public async Task<IActionResult> Index(string hidePastVisits)
        {

            ViewBag.HidePastVisitsParm = string.IsNullOrEmpty(hidePastVisits) ? "hide_past_visits" : "";
            
            var user = await _userManager.GetUserAsync(User);

            var patient = GetActualLoggedPatientFromDb(user);


            IQueryable<Visits> visits = (from v in _db.Visits.Include(d => d.Doctor)
                                         where v.PatientId == patient.Id
                                         select v);

            switch(hidePastVisits)
            {
                case "hide_past_visits":
                    visits = visits.Where(v => v.Date >= DateTime.Today);
                    break;
                default:
                    break;
            }

            return View(visits.OrderBy(d => d.Date));
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

            VisitVM.Visit = _db.Visits.Find(id);
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
            var user = await _userManager.GetUserAsync(User);
            var patient = GetActualLoggedPatientFromDb(user);


            var visit = await _db.Visits.FindAsync(id);

            var docUser = GetDoctorUserFromDb(visit.DoctorId);
            string docEmail = docUser.Email;


            _db.Visits.Remove(visit);
            await _db.SaveChangesAsync();

            //mail do lekarza
            await _emailSender.SendEmailAsync(docEmail, $"Anulowanie wizyty dnia: {visit.Date.ToShortDateString()}",
                        $"Pacjent {patient.FirstName + " " + patient.LastName} anulował wizytę o godzinie: {visit.Date.TimeOfDay}");

            //mail do pacjenta z potwierdzeniem anulowania wizyty
            await _emailSender.SendEmailAsync(user.Email, $"Anulowanie wizyty",
                $"Pomyślnie anulowałeś wizytę dnia: {visit.Date.ToShortDateString()}");

            return RedirectToAction(nameof(Index));
        }

        //GET - Recenzja
        [HttpGet]
        public async Task<ActionResult> Review(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var patient = GetActualLoggedPatientFromDb(user);

            VisitVM.Visit = _db.Visits.Find(id);
            VisitVM.Doctor = await _db.Doctors.Where(d => d.Id == VisitVM.Visit.DoctorId).FirstOrDefaultAsync();
            VisitVM.Review.DoctorId = VisitVM.Doctor.Id;
            VisitVM.Review.PatientId = patient.Id;


            if (VisitVM == null)
            {
                return NotFound();
            }

            return View(VisitVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Review(int id)
        {

            if (!ModelState.IsValid)
            {
                return View(VisitVM);
            }

            var review = VisitVM.Review;

            var vis = _db.Visits.Find(id);
            vis.IsReviewed = true;

            _db.Visits.Update(vis);
            _db.Reviews.Add(review);

            var doc = _db.Doctors.Find(VisitVM.Review.DoctorId);

            doc.RatingCount++;

            int sum = (from d in _db.Reviews
                      where d.DoctorId == doc.Id
                      select d.Rating).ToList().Sum();


            doc.AverageRating = (double)(sum + review.Rating) / doc.RatingCount;
            _db.Doctors.Update(doc);

            await _db.SaveChangesAsync();
            return (RedirectToAction(nameof(Index)));
        }


        [HttpGet]
        public async Task<ActionResult> TermChange(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var patient = GetActualLoggedPatientFromDb(user);

            VisitVM.Visit = _db.Visits.Find(id);
            VisitVM.Doctor = await _db.Doctors.Where(d => d.Id == VisitVM.Visit.DoctorId).FirstOrDefaultAsync();
            VisitVM.Review.DoctorId = VisitVM.Doctor.Id;
            VisitVM.Review.PatientId = patient.Id;

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

            if (VisitVM == null)
            {
                return NotFound();
            }

            return View(VisitVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TermChange(int id)
        {
            if (!ModelState.IsValid)
            {
                return View(VisitVM);
            }


            var user = await _userManager.GetUserAsync(User);
            var patient = GetActualLoggedPatientFromDb(user);

            var visit = await _db.Visits.FindAsync(id);

            var docUser = GetDoctorUserFromDb(visit.DoctorId);
            string docEmail = docUser.Email;
            
            DateTime oldDate = visit.Date;
            
            visit.Date = VisitVM.Visit.Date
                            .AddHours(VisitVM.Visit.Time.Hour)
                            .AddMinutes(VisitVM.Visit.Time.Minute);

            _db.Visits.Update(visit);
            await _db.SaveChangesAsync();

            //mail do lekarza
            await _emailSender.SendEmailAsync(docEmail, $"Zmiana terminu wizyty z dnia: {oldDate.ToShortDateString()}",
                        $"Pacjent {patient.FirstName + " " + patient.LastName} zmienił termin wizyty z dnia {oldDate.ToShortDateString()} o godzinie: {oldDate.TimeOfDay}" +
                        $"na termin: {visit.Date.ToShortDateString()} godz: {visit.Date.TimeOfDay}");

            //mail do pacjenta z potwierdzeniem zmiany terminu wizyty
            await _emailSender.SendEmailAsync(user.Email, $"Potwierdzenie zmiany terminu wizyty",
                $"Pomyślnie przełożyłeś wizytę z dnia: {oldDate.ToShortDateString()} godz: {oldDate.TimeOfDay}" + "\n" +
                $"na termin: {visit.Date.ToShortDateString()} o godz: {visit.Date.TimeOfDay}");

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<ActionResult> Calendar(int id, string Date)
        {
            var model = await _db.Doctors.Include(m => m.Specializations).Where(m => m.Id == id).FirstOrDefaultAsync();

            //model.SelectedDate = DateTime.Parse(Date);
            model.SelectedDate = DateTime.ParseExact(Date, "dd-MM-yyyy", null); //parsowanie z konkretnego formatu, bez określenia formatu były problemy na innych stacjach roboczych


            var takenHoursList = (from v in _db.Visits
                                  where (v.DoctorId == model.Id
                                  && v.Date.Day == model.SelectedDate.Day
                                  && v.Date.Month == model.SelectedDate.Month
                                  && v.Date.Year == model.SelectedDate.Year)
                                  select v.Date.TimeOfDay.ToString(@"hh\:mm")).ToList();

            model.TakenHoursInDay = takenHoursList;

            //Pętla generująca listę stringów odnośnie dostępnych godzin dla danego lekarza
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




        private IdentityUser GetDoctorUserFromDb(int id)
        {
            var doc = _db.Doctors.Find(id);
            return _db.Users.Find(doc.UserId);
        }

        private Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}