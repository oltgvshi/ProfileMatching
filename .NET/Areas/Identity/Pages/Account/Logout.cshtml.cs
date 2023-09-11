// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using ProfileMatching.Controllers;

namespace ProfileMatching.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DataContext _context;
        public LogoutModel(DataContext context, SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);

            //remove newjobposts claim
            var jobpostclaim = User.FindFirst("NewJobPosts");
            if (jobpostclaim != null)
            {
                await _signInManager.UserManager.RemoveClaimAsync(user, jobpostclaim);
            }

            //remove hiredjobs claim
            var hiredjobsclaim = User.FindFirst("HiredJobsCount");
            if (hiredjobsclaim != null)
            {
                await _signInManager.UserManager.RemoveClaimAsync(user, hiredjobsclaim);
            }

            //remove ApplicantsCount claim
            var applicantscount = User.FindFirst("ApplicantsCount");
            if (applicantscount != null)
            {
                await _signInManager.UserManager.RemoveClaimAsync(user, applicantscount);
            }

            // Delete all records from the UserCredentials table
            _context.UserCredentials.RemoveRange(_context.UserCredentials);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
