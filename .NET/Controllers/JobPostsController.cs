using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Authorize]
    public class JobPostsController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobPostsController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobPosts
        public async Task<IActionResult> Index(int pg = 1)
        {
            ViewData["Categories"] = _context.Categories.ToList();

            var expiredJobPost = await _context.JobPosts.Where(j => j.JobApplicationDeadline < DateTime.UtcNow).ToListAsync();

            foreach (var jobPost in expiredJobPost)
            {
                jobPost.IsArchived = true;
                _context.JobPosts.Update(jobPost);
            }

            await _context.SaveChangesAsync();

            List<JobPost> jobPosts = _context.JobPosts.Include(j => j.Category).Include(j => j.ApplicationUser).ToList();

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

            //kodi qe po na qon rolin e loggedin user te viewbag n view
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            //i bartim id te puneve qe useri has already applied for me ni viewdata
            var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["AppliedJobs"] = appliedJobs;

            //i bartim id te puneve qe useri has already saves for me ni viewdata
            var savedJobs = _context.SavedJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["SavedJobs"] = savedJobs;



            return View(data);
            //return View(await dataContext.OrderByDescending(j => j.JobPostBudget).ToListAsync());
        }


        // GET: JobPosts/Create
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> CreateAsync()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: JobPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Create([Bind("JobPostId,JobPostName,JobPostBudget,JobLength,JobPostDescription,JobApplicationDeadline,CategoryId,UserId")] JobPost jobPost)
        {
            jobPost.CompanyName = _context.ClientDetails.FirstOrDefault(x => x.UserId == jobPost.UserId).CompanyName;
            jobPost.CreationDate = DateTime.Now;
            _context.Add(jobPost);

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"City '{jobPost.JobPostName}' was created",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];

            //nqs po krijon jobPost klienti, e qon mandej te profili i vet me i pa t'dhanat
            if (roli == "Client")
            {
                return RedirectToAction("Index", "ClientProfile");
            }

            //perndryshe, nese sosht klient po admin, e qon te indexi
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", jobPost.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", jobPost.CategoryId);
        }

        // GET: JobPosts/Edit/5
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            if (id == null || _context.JobPosts == null)
            {
                return NotFound();
            }

            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", jobPost.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", jobPost.CategoryId);
            return View(jobPost);
        }

        // POST: JobPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Edit(int id, [Bind("JobPostId,JobPostName,JobPostBudget,JobLength,JobPostDescription,JobApplicationDeadline,CategoryId, UserId")] JobPost jobPost)
        {
            if (id != jobPost.JobPostId)
            {
                return NotFound();
            }

            try
            {
                jobPost.CompanyName = _context.ClientDetails.FirstOrDefault(x => x.UserId == jobPost.UserId).CompanyName;
                _context.Update(jobPost);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"City '{jobPost.JobPostName}' was edited",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostExists(jobPost.JobPostId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];

            if (roli == "Client")
            {
                return RedirectToAction("Index", "ClientProfile");
            }

            return RedirectToAction(nameof(Index));
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", jobPost.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", jobPost.CategoryId);
            
        }

        // GET: JobPosts/Delete/5
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobPosts == null)
            {
                return NotFound();
            }

            var jobPost = await _context.JobPosts
                .Include(j => j.Category)
                .FirstOrDefaultAsync(m => m.JobPostId == id);
            if (jobPost == null)
            {
                return NotFound();
            }

            return View(jobPost);
        }

        // POST: JobPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobPosts == null)
            {
                return Problem("Entity set 'DataContext.JobPosts'  is null.");
            }
            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost != null)
            {
                _context.JobPosts.Remove(jobPost);
            }

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"City '{jobPost.JobPostName}' was deleted",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];

            if (roli == "Client")
            {
                return RedirectToAction("Index", "ClientProfile");
            }

            return RedirectToAction(nameof(Index));

        }
        private bool JobPostExists(int id)
        {
            return _context.JobPosts.Any(e => e.JobPostId == id);
        }


        public async Task<IActionResult> SearchJobPost(string teksti)
        {
            //kodi per me rujt rolin e logged in user me ni viewdata
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            ViewData["Categories"] = _context.Categories.ToList();

            ViewData["SearchInput"] = teksti;

            //i bartim id te puneve qe useri has already applied for me ni viewdata
            var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["AppliedJobs"] = appliedJobs;

            //i bartim id te puneve qe useri has already saves for me ni viewdata
            var savedJobs = _context.SavedJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["SavedJobs"] = savedJobs;

            var allJobs = _context.JobPosts.Include(j => j.Category).Include(j => j.ApplicationUser);
            var result = allJobs.Where(f => f.Category.CategoryName.ToLower().Contains(teksti.ToLower())
                || f.JobPostName.ToLower().Contains(teksti.ToLower())
                || f.ApplicationUser.FirstName.ToLower().Contains(teksti.ToLower())
                || f.ApplicationUser.LastName.ToLower().Contains(teksti.ToLower())
                || f.CompanyName.ToLower().Contains(teksti.ToLower()));

            return View(await result.ToListAsync());
        }

        private async Task ArchivedJobPost()
        {
            var expiredJobPost = await _context.JobPosts.Where(j => j.JobApplicationDeadline < DateTime.UtcNow).ToListAsync();

            foreach (var JobPost in expiredJobPost)
            {
                JobPost.IsArchived = true;
            }

            await _context.SaveChangesAsync();
        }
 

        public async Task<IActionResult> FilterJobPost(int? id, int pg=1)
        {
            //kodi per me rujt rolin e logged in user me ni viewdata
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            ViewData["Categories"] = _context.Categories.ToList();

            //po e marrim kategorine qe e kena select (dmth prej id qe e pranon metoda)
            var category = _context.Categories.Where(x => x.CategoryId == id).First();
            ViewData["FilteredCategoryName"] = category.CategoryName;
            ViewData["FilteredCategoryId"] = category.CategoryId;

            //i bartim id te puneve qe useri has already applied for me ni viewdata
            var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["AppliedJobs"] = appliedJobs;

            //i bartim id te puneve qe useri has already saves for me ni viewdata
            var savedJobs = _context.SavedJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["SavedJobs"] = savedJobs;

            //i marrim punt me qat kategori
            List<JobPost> jobPosts = _context.JobPosts
                .Include(f => f.Category)
                .Where(f => f.Category.CategoryId == id).ToList();

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


            if (jobPosts == null)
            {
                return NotFound();
            }

            return View(data);
        }

    }

}
