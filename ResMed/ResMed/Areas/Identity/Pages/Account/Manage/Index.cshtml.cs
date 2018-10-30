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
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            [Required]
            [EmailAddress]
            [Display(Name = "E-mail")]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Nr telefonu")]
            public string PhoneNumber { get; set; }


            //Pola dodatkowe Input dla Doktora
            [Display(Name = "Nr licencji")]
            public string LicenseNr { get; set; }

            [Display(Name = "Opis")]
            public string Description { get; set; }

            [Display(Name = "Specjalizacja")]
            public int? SpecializationId { get; set; }

            //public SelectList selectListSpec { get; set; }




        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;



            if (User.IsInRole(SD.DoctorRole)) //sprawdza czy zalogowany uzytkownik jest doktorem, jesli tak to rozszerza model Input o dodatkowe pola Doktora
            {
                var doctor = GetActualLoggedDoctorFromDb(user); // pobiera obiekt aktualnego doktora z bazy danych

                Input = new InputModel
                {
                    Email = email,
                    PhoneNumber = phoneNumber,
                    LicenseNr = doctor.LicenseNr,
                    Description = doctor.Description,
                    SpecializationId = doctor.SpecializationId
                };

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

                ViewData["SpecializationsList"] = specs; //Lista typu SelectListItem wrzucana do ViewData aby móc ją przechwycic w stronie Razor Index.cshtml

            }
            else if (User.IsInRole(SD.PatientRole)) //sprawdza czy zalogowany uzytkownik jest pacjentem, jesli tak to rozszerza model Input o dodatkowe pola Pacjenta
            {
                var patient = GetActualLoggedPatientFromDb(user);
                Input = new InputModel
                {
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Description = patient.Description
                };
            }
            else
            {
                Input = new InputModel
                {
                    Email = email,
                    PhoneNumber = phoneNumber
                };
            }

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

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

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
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


            if (User.IsInRole(SD.DoctorRole))
            {
                var doctor = GetActualLoggedDoctorFromDb(user); //pobieranie obiektu zalogowanego doktora z bazy danych

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

            }

            if (User.IsInRole(SD.PatientRole))
            {
                var patient = GetActualLoggedPatientFromDb(user); //pobieranie obiektu zalogowanego doktora z bazy danych

                if (Input.Description != patient.Description)
                {
                    patient.Description = Input.Description;
                    _db.Patients.Update(patient);
                    await _db.SaveChangesAsync();
                }

            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();



        }



        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
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


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }

        /////////Metody///////////////

        private Models.Doctors GetActualLoggedDoctorFromDb(IdentityUser user)
        {
            return _db.Doctors.FirstOrDefault(x => x.UserId == user.Id);
        }


        private Models.Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(p => p.UserId == user.Id);
        }
    }
}
