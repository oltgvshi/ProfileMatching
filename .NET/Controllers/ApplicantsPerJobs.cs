using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Areas.Admin.Controllers;
using ProfileMatching.Migrations;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Authorize]
    public class ApplicantsPerJobs : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicantsPerJobs(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<ApplicantsPerJob> applicantsPerJobs = _context.ApplicantsPerJobs.Include(j => j.JobPost).Include(j => j.ApplicationUser).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = applicantsPerJobs.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = applicantsPerJobs.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);
        }



        [HttpPost]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> ApplyForJob(int id)
        {
            //merr id te logged in user
            var userId = _userManager.GetUserId(HttpContext.User);

            /*krijohet ni instance e re e klases ApplicantsPerJob me id t userit edhe
              job id qe po pranohet n metode*/
            var itemToAdd = new ApplicantsPerJob
            {
                UserId = userId,
                JobPostId = id,
                ApplicationDate = DateTime.Now
            };

            //check nese ekziston ndatabaz ni record/rresht ku veq useri veq ka apliku per qat job
            if ((_context.ApplicantsPerJobs.Any(p => p.JobPostId == id && p.UserId == userId)))
            {
                TempData["Message"] = "You already applied for this job";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }

            TempData["SuccessfulApplication"] = "Successful Application!";
            _context.ApplicantsPerJobs.Add(itemToAdd);
            await _context.SaveChangesAsync();
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> UnApply(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var job = _context.ApplicantsPerJobs.Where(x => x.JobPostId == id && x.UserId == userId).First();

            if (job != null)
            {
                _context.ApplicantsPerJobs.Remove(job);
            }

            await _context.SaveChangesAsync();
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }


        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> ShowJobApplicants(int? id)
        {
            if (id == null || _context.ApplicantsPerJobs == null)
            {
                return NotFound();
            }

            var job = _context.JobPosts.Where(x => x.JobPostId == id).First();
            ViewData["JobName"] = job.JobPostName;
            //^qitu jobName e kena marr veq per me "bajt" n view tani me mujt me ja thirr

            var applicants = await _context.ApplicantsPerJobs.Include(j => j.JobPost).Include(j => j.ApplicationUser).Where(m => m.JobPostId == id).ToListAsync();
            if (applicants == null)
            {
                return NotFound();
            }

            return View(applicants);
        }


        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> HireApplicant(int? id, bool hired)
        {
            var hiredperson = _context.ApplicantsPerJobs.Where(x => x.ApplicantPerJobId == id).First();
            
            if (hiredperson == null)
                return NotFound();

            hiredperson.HiredStatus = !hired;
            hiredperson.HiredDate = DateTime.Now;

            _context.Update(hiredperson);

            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [Authorize(Roles = "Admin, Client")]
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


        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> MyJobApplications()
        {
            //kodi qe po na qon rolin e loggedin user te viewbag n view
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First(); //nbaz t ids e marrin prej databaze krejt userin
            var RolesForUser = await _userManager.GetRolesAsync(user); //ja kqyrim rolet
            var roli = RolesForUser[0]; //e marrim veq rolin e pare dmth npoziten 0
            ViewData["Role"] = roli;

            //i bartim id te puneve qe useri has already applied for me ni viewdata
            var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["AppliedJobs"] = appliedJobs;

            var jobs = _context.ApplicantsPerJobs
              .Where(m => m.UserId == userId)
              .Include(m => m.JobPost)
              .Include(m => m.JobPost.Category)
              .Include(m => m.ApplicationUser)
              .OrderByDescending(m => m.HiredStatus)
              .ToListAsync();

            return View(await jobs);
        }



    }
}
