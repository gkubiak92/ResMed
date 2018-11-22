using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ResMed.Data;
using ResMed.Models;
using ResMed.Utility;
using ResMed.Extensions;

namespace ResMed.Areas.Identity.Pages.Account.Manage
{
    public partial class DoctorCart : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public DoctorCart(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [Display(Name = "Nazwa użytkownika")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Display(Name = "Imię")]
            public string FirstName { get; set; }


            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Nr telefonu")]
            public string PhoneNumber { get; set; }


            //Pola dodatkowe Input dla Doktora
            [Required(ErrorMessage = "Pole wymagane")]
            [Display(Name = "Nr licencji")]
            public string LicenseNr { get; set; }

            [Display(Name = "Opis")]
            public string Description { get; set; }

            [Display(Name = "Specjalizacja")]
            public int? SpecializationId { get; set; }

            [Display(Name = "Adres")]
            public string Address { get; set; }

            [Display(Name = "Średni czas wizyty [min]")]
            [Range(0, 360, ErrorMessage = "Wprowadź właściwą liczbę")]
            public int AverageVisitTime { get; set; }

            [Display(Name = "Start pracy")]
            public DateTime StartWorkHours { get; set; }

            [Display(Name = "Koniec pracy")]
            public DateTime EndWorkHours { get; set; }





            [Display(Name = "Poniedziałek")]
            public bool WorkingMonday { get; set; }

            [Display(Name = "Wtorek")]
            public bool WorkingTuesday { get; set; }

            [Display(Name = "Środa")]
            public bool WorkingWednesday { get; set; }

            [Display(Name = "Czwartek")]
            public bool WorkingThursday { get; set; }

            [Display(Name = "Piątek")]
            public bool WorkingFriday { get; set; }

            [Display(Name = "Sobota")]
            public bool WorkingSaturday { get; set; }

            [Display(Name = "Niedziela")]
            public bool WorkingSunday { get; set; }

        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);


            var doctor = GetActualLoggedDoctorFromDb(user); // pobiera obiekt aktualnego doktora z bazy danych

            var specList = _db.Specializations.ToList().OrderBy(s => s.Name); //zaciąga listę specjalizacji z bazy danych
            var specs = new List<SelectListItem>();  //tworzy nową listę typu selectlistitem - itemy do wybierania
            foreach (var item in specList) //pętla, która leci po wszystkich itemach listy specjlizacji i przypisuje je kolejno do itemów listy selectlist
            {
                specs.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            };

            ViewData["SpecsList"] = specs; //Lista typu SelectListItem wrzucana do ViewData aby móc ją przechwycic w stronie Razor Index.cshtml


            Input = new InputModel
            {
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = phoneNumber,
                LicenseNr = doctor.LicenseNr,
                Description = doctor.Description,
                SpecializationId = doctor.SpecializationId,
                Address = doctor.Address,
                AverageVisitTime = doctor.AverageVisitTime,
                StartWorkHours = doctor.StartWorkHours,
                EndWorkHours = doctor.EndWorkHours,

                WorkingMonday = doctor.WorkingMonday,
                WorkingTuesday = doctor.WorkingTuesday,
                WorkingWednesday = doctor.WorkingWednesday,
                WorkingThursday = doctor.WorkingThursday,
                WorkingFriday = doctor.WorkingFriday,
                WorkingSaturday = doctor.WorkingSaturday,
                WorkingSunday = doctor.WorkingSunday
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            var doctor = GetActualLoggedDoctorFromDb(user); //pobieranie obiektu zalogowanego doktora z bazy danych

            if (Input.FirstName != doctor.FirstName)
            {
                doctor.FirstName = Input.FirstName;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }

            if (Input.LastName != doctor.LastName)
            {
                doctor.LastName = Input.LastName;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }

            if (Input.LicenseNr != doctor.LicenseNr)
            {
                doctor.LicenseNr = Input.LicenseNr;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }

            if (Input.Description != doctor.Description)
            {
                doctor.Description = Input.Description;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }

            if (Input.SpecializationId != doctor.SpecializationId)
            {
                doctor.SpecializationId = Input.SpecializationId;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }
            if (Input.Address != doctor.Address)
            {
                doctor.Address = Input.Address;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }
            if (Input.AverageVisitTime != doctor.AverageVisitTime)
            {
                doctor.AverageVisitTime = Input.AverageVisitTime;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }
            if (Input.StartWorkHours != doctor.StartWorkHours)
            {
                doctor.StartWorkHours = Input.StartWorkHours;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }
            if (Input.EndWorkHours != doctor.EndWorkHours)
            {
                doctor.EndWorkHours = Input.EndWorkHours;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }


            //bool[] workingDayArr = { Input.WorkingMonday,
            //                            Input.WorkingTuesday,
            //                            Input.WorkingWednesday,
            //                            Input.WorkingThursday,
            //                            Input.WorkingFriday,
            //                            Input.WorkingSaturday,
            //                            Input.WorkingSunday};

            //int[] finalArr = new int[7];


            //for (int i = 0; i < workingDayArr.Length; i++)
            //{
            //    if (workingDayArr[i] == true)
            //        finalArr[i] = i;
            //    else
            //        finalArr[i] = 50; //50 tak o, bo tydzien ma tylko 7 dni - chwilowe rozwiązanie
            //}

            //doctor.WorkDaysArr = new int[7];

            //finalArr.CopyTo(doctor.WorkDaysArr, 0);

            doctor.WorkingMonday = Input.WorkingMonday;
            doctor.WorkingTuesday = Input.WorkingTuesday;
            doctor.WorkingWednesday = Input.WorkingWednesday;
            doctor.WorkingThursday = Input.WorkingThursday;
            doctor.WorkingFriday = Input.WorkingFriday;
            doctor.WorkingSaturday = Input.WorkingSaturday;
            doctor.WorkingSunday = Input.WorkingSunday;

            _db.Doctors.Update(doctor);
            await _db.SaveChangesAsync();


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Twój profil został zaktualizowany";/* + doctor.WorkDaysArr[0] + ' ' + doctor.WorkDaysArr[1] + ' ' + doctor.WorkDaysArr[2]*/
            return RedirectToPage();

        }


        /////////Metody///////////////

        private Models.Doctors GetActualLoggedDoctorFromDb(IdentityUser user)
        {
            return _db.Doctors.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}
