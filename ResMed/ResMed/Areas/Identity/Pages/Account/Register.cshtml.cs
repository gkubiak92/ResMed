using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ResMed.Data;
using ResMed.Models;
using ResMed.Utility;

namespace ResMed.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Imię")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Nazwisko")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Nr telefonu")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Typ konta")]
            public string SelectedAccountType { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            //Jednorazowy kod na potrzeby dodania konta admina
            //var adminUser = new ApplicationUser
            //{
            //    UserName = "admin@gmail.com",
            //    Email = "admin@gmail.com",
            //    PhoneNumber = "123123123",
            //};
            //var adminResult = await _userManager.CreateAsync(adminUser, "Admin12345.");
            //if (adminResult.Succeeded)
            //    await _userManager.AddToRoleAsync(adminUser, SD.AdminRole);


            returnUrl = returnUrl ?? Url.Content("~/Identity/Account/Manage");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                };
                var result = await _userManager.CreateAsync(user, Input.Password);



                if (result.Succeeded)
                {
                    
                    if (!await _roleManager.RoleExistsAsync(SD.DoctorRole)) // sprawdza czy jest rola Doktor, jeśli nie to tworzy ją
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.DoctorRole));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.PatientRole)) // sprawdza czy jest rola Pacjent, jeśli nie to tworzy ją
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.PatientRole));
                    }

                    //Sprawdzenie jaka rola została wybrana podczas rejestracji i utworzenie odpowiedniego rekordu w tabeli Doctors lub Patients
                    if (Input.SelectedAccountType == SD.DoctorRole)
                    {
                        await _userManager.AddToRoleAsync(user, SD.DoctorRole);

                        var doctor = new Doctors
                        {
                            UserId = user.Id,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            LicenseNr = "",
                            Description = "",
                            Gender = "",
                            AverageRating = 0,
                            RatingCount = 0
                        };

                        _db.Doctors.Add(doctor);
                        await _db.SaveChangesAsync();
                    }

                    if (Input.SelectedAccountType == SD.PatientRole)
                    {
                        await _userManager.AddToRoleAsync(user, SD.PatientRole);

                        var patient = new Patients
                        {
                            UserId = user.Id,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            Gender = "",
                            Description = ""
                        };

                        _db.Patients.Add(patient);
                        await _db.SaveChangesAsync();
                    }
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Potwierdź swój adres e-mail",
                        $"Potwierdź swoje konto <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>klikając tutaj</a>.");

                    //await _signInManager.SignInAsync(user, isPersistent: false); linia ta zakomentowana aby uniknąć automatycznego zalogowania nowego użytkownika bez potwierdzenia maila
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
