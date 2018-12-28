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
using Microsoft.AspNetCore.Hosting.Internal;
using System.IO;

namespace ResMed.Areas.Identity.Pages.Account.Manage
{
    public partial class DoctorCart : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        public DoctorCart(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db,
            HostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _hostingEnvironment = hostingEnvironment;
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
            public bool IsActive { get; set; }

            [Display(Name = "Płeć")]
            public string Gender { get; set; }

            [Display(Name = "Imię")]
            public string FirstName { get; set; }


            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Nr telefonu (9 cyfr bez +48)")]
            [Range(0, int.MaxValue, ErrorMessage = "Nr telefonu może zawierać tylko cyfry")]
            [StringLength(9, ErrorMessage = "Nr tel musi składać się z 9 cyfr")]
            public string PhoneNumber { get; set; }

            public string Image { get; set; }


            //Pola dodatkowe Input dla Doktora
            [Required(ErrorMessage = "Pole wymagane")]
            [Display(Name = "Nr licencji")]
            [Range(0, int.MaxValue, ErrorMessage = "Nr PWZ może zawierać tylko cyfry")]
            public string LicenseNr { get; set; }

            [Display(Name = "Opis")]
            public string Description { get; set; }

            [Display(Name = "Specjalizacja")]
            public int? SpecializationId { get; set; }

            [Display(Name = "Adres")]
            public string Address { get; set; }

            [Display(Name = "Nazwa gabinetu")]
            public string OfficeName { get; set; }

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
                IsActive = doctor.IsActive,
                Gender = doctor.Gender,
                Image = doctor.Image,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                PhoneNumber = phoneNumber,
                LicenseNr = doctor.LicenseNr,
                Description = doctor.Description,
                SpecializationId = doctor.SpecializationId,
                Address = doctor.Address,
                OfficeName = doctor.OfficeName,
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
                return RedirectToPage();
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
                int[] digits = Input.LicenseNr.ToString().ToCharArray().Select(x => (int)Char.GetNumericValue(x)).ToArray(); //przekształcenie wpisanego nr PWZ na tablicę intów

                if (Input.LicenseNr.Length != 7)
                {
                    StatusMessage = "Błąd. Numer prawa wykonywania zawodu musi posiadać 7 cyfr.";
                    return RedirectToPage();
                }
                else if (digits[0] == 0)
                {
                    StatusMessage = ("Błąd. Numer prawa wykonywania zawodu nie może zaczynać się od cyfry 0.");
                    return RedirectToPage();
                }
                else
                {
                    int j = 1;
                    int sum = 0;
                    for (int i = 1; i < 7; i++)
                    {
                        sum = sum + (digits[i] * j);
                        j = j + 1;
                    }
                    int mod = sum % 11;

                    if (mod != digits[0])
                    {
                        StatusMessage = ("Błąd. Nieprawidłowa cyfra kontrolna. Numer prawa wykonywania zawodu jest niepoprawny.");
                    }

                    else
                    {
                        StatusMessage = ("Pomyślnie ustawiono numer prawa wykonywania zawodu.");
                        doctor.LicenseNr = Input.LicenseNr;
                        doctor.IsActive = true;
                        _db.Doctors.Update(doctor);
                        await _db.SaveChangesAsync();
                    }
                }
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
            if (Input.OfficeName != doctor.OfficeName)
            {
                doctor.OfficeName = Input.OfficeName;
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
            if (Input.Gender != doctor.Gender)
            {
                doctor.Gender = Input.Gender;
                _db.Doctors.Update(doctor);
                await _db.SaveChangesAsync();
            }



            //Zapisanie zdjęcia profilowego

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (string.IsNullOrEmpty(doctor.Image))
            {
                //var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultDoctorImage);
                //System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + doctor.Id + ".png", true);
                string defaultImagePath = @"\" + SD.ImageFolder + @"\" + SD.DefaultDoctorImage;
                doctor.Image = defaultImagePath;
            }

            if (files.Count != 0)
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, doctor.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                doctor.Image = @"\" + SD.ImageFolder + @"\" + doctor.Id + extension;
            }
            //else
            //{
            //    when user does not upload image
            //    var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultDoctorImage);
            //    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + doctor.Id + ".png", true);
            //    doctor.Image = @"\" + SD.ImageFolder + @"\" + doctor.Id + ".png";
            //}
            await _db.SaveChangesAsync();



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
