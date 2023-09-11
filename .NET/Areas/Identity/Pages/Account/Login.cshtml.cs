// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProfileMatching.Controllers;
using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using ProfileMatching.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ProfileMatching.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DataContext _context;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    //uncomment this kur don ni user me bo admin, veq qite te userId Id e userit
                    /*
                    var userId = "025c5af0-daa3-46ba-a808-da23a5ce9faf";
                    var user = _context.Users.Where(x => x.Id == userId).First();
                    await _userManager.RemoveFromRoleAsync(user, "Freelancer");
                    await _userManager.AddToRoleAsync(user, "Admin");*/

                    
                    //get user id and freelancer category
                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                    var userId = user.Id;
                    string jobCount = "";
                    string hiredJobCount = "";
                    string applicantsCount = "";

                    var userRole = await _userManager.GetRolesAsync(user);

                    // Create a new UserCredentials object
                    var userCredentials = new UserCredentials
                    {
                        UserId = userId,
                        UserRole = userRole.FirstOrDefault() // Assuming the user has only one role
                    };

                    // Add the UserCredentials object to the database
                    _context.UserCredentials.Add(userCredentials);
                    await _context.SaveChangesAsync();

                    //check for last login
                    var existingLogin = await _context.RecentLogins.FirstOrDefaultAsync(l => l.UserId == userId);

                   if (await _userManager.IsInRoleAsync(user, "Freelancer"))
                    {
                        var category = (await _context.FreelancerDetails.FirstOrDefaultAsync(c => c.UserId == userId)).CategoryId;
                        //para se me rujt new recentLogin, check for new jobPosts with the same category
                        var newJobPosts = await _context.JobPosts
                            .Where(j => j.CreationDate > existingLogin.LoginDate && j.CategoryId == category)
                            .ToListAsync();

                        jobCount = newJobPosts.Count.ToString();


                        var hiredJobs = await _context.ApplicantsPerJobs.Where(j => j.UserId == userId && j.HiredStatus && j.HiredDate > existingLogin.LoginDate).ToListAsync();
                        hiredJobCount = hiredJobs.Count.ToString();
                    }

                   if (await _userManager.IsInRoleAsync(user, "Client"))
                    {
                        var clientJobs = await _context.JobPosts.Where(j => j.UserId == userId).Select(j => j.JobPostId).ToListAsync();
                        var newApplicants = await _context.ApplicantsPerJobs
                            .Where(a => clientJobs.Contains((int)a.JobPostId) && a.ApplicationDate > existingLogin.LoginDate)
                            .ToListAsync();

                        applicantsCount = newApplicants.Count.ToString();
                    }
                    


                    // Add the custom variables to the user's claims

                    var claims = new List<Claim>
                    {
                        new Claim("NewJobPosts", jobCount),
                        new Claim("HiredJobsCount", hiredJobCount),
                        new Claim("ApplicantsCount", applicantsCount)
                    };


                    await _signInManager.UserManager.AddClaimsAsync(user, claims);
                    await _signInManager.RefreshSignInAsync(user);
                    

                    if (existingLogin != null)
                    {
                        existingLogin.LoginDate = DateTime.Now;
                        _context.Update(existingLogin);
                    }
                    else
                    {
                        var recentLogin = new RecentLogin
                        {
                            UserId = userId,
                            LoginDate = DateTime.Now
                        };
                        _context.RecentLogins.Add(recentLogin);
                    }
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User logged in.");
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
