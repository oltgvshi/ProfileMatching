﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProfileMatching.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ProfileMatching.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var user = _context.Users.Where(x => x.Id == userId).First();


                var allJobPosts = _context.JobPosts.Include(j => j.Category).Include(j => j.ApplicationUser);
                ViewData["RecentJobPosts"] = allJobPosts.OrderByDescending(j => j.JobPostId).Take(3);

                var userCategory = _context.FreelancerDetails.FirstOrDefault(x => x.UserId == userId).CategoryId;
                var matchingJobPosts = allJobPosts.Where(x => x.CategoryId == userCategory).OrderByDescending(j => j.JobPostId).Take(3);
                ViewData["MatchingJobPosts"] = matchingJobPosts;

                //i bartim id te puneve qe useri has already applied for me ni viewdata
                var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
                ViewData["AppliedJobs"] = appliedJobs;

                //i bartim id te puneve qe useri has already saves for me ni viewdata
                var savedJobs = _context.SavedJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
                ViewData["SavedJobs"] = savedJobs;

                ViewData["Freelancers"] = _context.FreelancerDetails.Include(f => f.ApplicationUser).Include(f => f.Category).Include(f => f.City).OrderByDescending(j => j.FreelancerDetailsId).Take(3);

                var slider = _context.Sliders.ToList();
                ViewData["Sliders"] = slider;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}