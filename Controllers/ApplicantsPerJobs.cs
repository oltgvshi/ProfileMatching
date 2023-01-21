﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    public class ApplicantsPerJobs : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicantsPerJobs(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicantsPerJobs.ToListAsync());
        }



        [HttpPost]
        public async Task<IActionResult> ApplyForJob(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            var itemToAdd = new ApplicantsPerJob
            {
                UserId = userId,
                JobPostId = id
            };

            if((_context.ApplicantsPerJobs.Any(p => p.JobPostId == id && p.UserId == userId)))
            {
                return Problem("You already applied for this job");
            }

            _context.ApplicantsPerJobs.Add(itemToAdd);
            await _context.SaveChangesAsync();
            return View(itemToAdd); 
        }



        public async Task<IActionResult> ShowJobApplicants(int? id)
        {
            if (id == null || _context.ApplicantsPerJobs == null)
            {
                return NotFound();
            }

            var applicants = await _context.ApplicantsPerJobs.Where(m => m.JobPostId == id).ToListAsync();
            if (applicants == null)
            {
                return NotFound();
            }

            return View(applicants);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (_context.ApplicantsPerJobs == null)
            {
                return Problem("Entity set 'DataContext.ApplicantsPerJob'  is null.");
            }
            var application = await _context.ApplicantsPerJobs.FindAsync(id);

            var jobId = application.JobPostId;

            if (application != null)
            {
                _context.ApplicantsPerJobs.Remove(application);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowJobApplicants", new { id = jobId });
        }



        public async Task<IActionResult> FreeLancerJobApplications()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            return View(await _context.ApplicantsPerJobs.Where(m => m.UserId == userId).ToListAsync());
        }



    }
}