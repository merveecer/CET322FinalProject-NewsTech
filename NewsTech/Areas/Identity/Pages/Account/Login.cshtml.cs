using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NewsTech.Data;

namespace NewsTech.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
		private readonly NewsTechDbContext _context;
        private readonly SignInManager<NewsTechUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<NewsTechUser> _userManager;

		public LoginModel(SignInManager<NewsTechUser> signInManager, RoleManager<IdentityRole> roleManager, UserManager<NewsTechUser> userManager, ILogger<LoginModel> logger, NewsTechDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
			_context = context;
				_userManager = userManager;
			_roleManager = roleManager;
		}

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
			[Display(Name ="E-posta")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
			[Display(Name = "Parola")]
			public string Password { get; set; }

            [Display(Name = "Beni Hatırla")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
					var user = _context.Users.Where(x => x.Email == Input.Email).FirstOrDefault();
                    _logger.LogInformation("User logged in.");
					var roleName = await _userManager.GetRolesAsync(user);
					var role = await _roleManager.FindByNameAsync(roleName.FirstOrDefault());
					if (role.Name == "admin" || role.Name == "editor")
						return RedirectToAction("Index", "Home");
					return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
