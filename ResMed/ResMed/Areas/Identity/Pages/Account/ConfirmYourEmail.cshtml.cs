using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ResMed.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmYourEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmYourEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            return Page();
        }
    }
}
