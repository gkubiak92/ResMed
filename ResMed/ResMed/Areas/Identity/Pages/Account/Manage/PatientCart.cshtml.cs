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
    public partial class PatientCart : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public PatientCart(
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
            
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);


            var patient = GetActualLoggedPatientFromDb(user); // pobiera obiekt aktualnego doktora z bazy danych
            
            Input = new InputModel
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                PhoneNumber = phoneNumber,
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

            var patient = GetActualLoggedPatientFromDb(user); //pobieranie obiektu zalogowanego doktora z bazy danych

            if (Input.FirstName != patient.FirstName)
            {
                patient.FirstName = Input.FirstName;
                _db.Patients.Update(patient);
                await _db.SaveChangesAsync();
            }

            if (Input.LastName != patient.LastName)
            {
                patient.LastName = Input.LastName;
                _db.Patients.Update(patient);
                await _db.SaveChangesAsync();
            }
            
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Twój profil został zaktualizowany";
            return RedirectToPage();

        }
        

        /////////Metody///////////////

        private Patients GetActualLoggedPatientFromDb(IdentityUser user)
        {
            return _db.Patients.FirstOrDefault(x => x.UserId == user.Id);
        }
    }
}
