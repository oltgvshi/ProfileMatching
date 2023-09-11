using Microsoft.AspNetCore.Mvc;
using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ProfileMatching.Controllers
{
    [Authorize]
    public class JobSuggestions : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobSuggestions(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Index(int pg=1)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
 
            //i marrim freelancerDetails te freelancer qe osht logged in
            var detail = _context.FreelancerDetails.FirstOrDefault(x => x.UserId == user.Id);

            //niher check qaj user a i ka qit freelancer detials nprofil (se na vyn kategoria), nqs jo, e qon te faqja me create freelancer details
            if (detail == null)
            {
                return RedirectToAction("Create", "FreelancerDetails");
            }

            var cat = detail.CategoryId; //prej freelancers details t'qatij useri, po e marrim veq kategorine

            var userId = _userManager.GetUserId(HttpContext.User);
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;


            List<JobPost> jobPosts = _context.JobPosts.Where(x => x.CategoryId == cat).Include(j => j.Category).Include(j => j.ApplicationUser).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = jobPosts.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = jobPosts.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            //i bartim id te puneve qe useri has already applied for me ni viewdata
            var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["AppliedJobs"] = appliedJobs;

            //i bartim id te puneve qe useri has already saved me ni viewdata
            var savedJobs = _context.SavedJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["SavedJobs"] = savedJobs;

            if (jobPosts == null)
            {
                return NotFound();
            }

            return View(data);
        }
    }
    }






