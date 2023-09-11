using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Authorize]
    public class SavedJobs : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SavedJobs(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg=1)
        {
            List<SavedJob> savedJobs = _context.SavedJobs.ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = savedJobs.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = savedJobs.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);
        }



        [HttpPost]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> SaveJob(int id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            /*krijohet ni instance e re e klases SavedJob me id t userit edhe
              job id qe po pranohet n metode*/
            var itemToAdd = new SavedJob
            {
                UserId = userId,
                JobPostId = id
            };

            //check nese ekziston ndatabaz ni record/rresht ku veq useri veq e ka bo save qat job
            if ((_context.SavedJobs.Any(p => p.JobPostId == id && p.UserId == userId)))
            {
                TempData["Message2"] = "You already saved this job";
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }

            TempData["SuccessfulSave"] = "Successful Save!";
            _context.SavedJobs.Add(itemToAdd);
            await _context.SaveChangesAsync();
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        //kjo e merr id te logged in user edhe jobpostId, aplikohet veq kur freelancer e bon vet unsave 
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Unsave(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var job = _context.SavedJobs.Where(x => x.JobPostId == id && x.UserId == userId).First();

            if (job != null)
            {
                _context.SavedJobs.Remove(job);
            }

            await _context.SaveChangesAsync();
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }


        //kto e perdor veq admini, e merr id savedjobID
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSave(int? id)
        {
            if (_context.SavedJobs == null)
            {
                return Problem("Entity set 'DataContext.Savedjobs'  is null.");
            }
            var job = await _context.SavedJobs.FindAsync(id);

            if (job != null)
            {
                _context.SavedJobs.Remove(job);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> MySavedJobs()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            //e merr id t userit qe osht logged in, qe me ja shfaq saved jobs t tij

            /*po i marrim edhe applied jobs, per arsye se kur e sheh useri listen e saved jobs,
              aty me mujt me ja shfaq edhe butonin apply apo unapply */
            var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["AppliedJobs"] = appliedJobs;

            //return saved jobs
            var jobs = _context.SavedJobs
              .Where(m => m.UserId == userId)
              .Include(m => m.JobPost)
              .Include(m => m.JobPost.Category)
              .Include(m => m.ApplicationUser)
              .ToListAsync();
            
            return View(await jobs);
        }



    }
}
