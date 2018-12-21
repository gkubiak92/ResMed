using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ResMed.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordRejection : PageModel
    {
        public void OnGet()
        {
        }
    }
}
