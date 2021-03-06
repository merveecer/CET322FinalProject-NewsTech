﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NewsTech.Data;

namespace NewsTech.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<NewsTechUser> _signInManager;
        private readonly UserManager<NewsTechUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
	
		public IEnumerable<SelectListItem> GenderList { get; set; }
		public RegisterModel(
            UserManager<NewsTechUser> userManager,
            SignInManager<NewsTechUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
			

		}

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
			public InputModel() {
				GenderList = new List<SelectListItem>();
			}
            [Required]
            [EmailAddress]
            [Display(Name = "E-posta")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Şifre Onay")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
			//Merve 12.05.2019
			[Required]
			[StringLength(100)]
			[Display(Name = "Ad")]

			public string FirstName { get; set; }

			[Required]
			[StringLength(100)]
			[Display(Name = "Soyad")]

			public string LastName { get; set; }

			//[Required]
			//[StringLength(100)]
			//public string City { get; set; }

			[Required]
			[Display(Name = "Doğum Tarihi")]

			public DateTime BirthDate { get; set; }

			[Display(Name = "Cinsiyet")]
			public int SelectedGenderId { get; set; }
			public IEnumerable<SelectListItem> GenderList { get; set; }

			//MerveEnd
		}
		public enum GenderType
		{
			Erkek = 1,
			Kadın = 2
		}
		public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
			IEnumerable<GenderType> GenderType = Enum.GetValues(typeof(GenderType)).Cast<GenderType>();
			ViewData["Gender"] = from gender in GenderType
									   select new SelectListItem {
										   Text = gender.ToString(),
										   Value = ((int)gender).ToString()
									   };
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
			{
			var gender = Input.SelectedGenderId;

			returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
				var user = new NewsTechUser { UserName = Input.Email, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName, BirthDate = Input.BirthDate , isActive=true,isDeleted=false,CreatedDateTime=DateTime.Now,Gender=gender};
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
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
