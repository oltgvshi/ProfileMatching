using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProjectsController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectsController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg=1)
        {
            List<Project> projects = _context.Projects.Include(p => p.ApplicationUser).Include(p => p.Category).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = projects.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = projects.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            return View(data);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ApplicationUser)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Freelancer")]
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

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Create([Bind("ProjectId,ProjectName,FileName,Description,ProjectWebsite,CategoryId,UserId")] Project project)
        {

            _context.Add(project);
            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"Project '{project.ProjectName}' was created",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];

            /*po kqyrim a osht roli i userit qe po e perdor create project freelancer,
              qe dmth nese o freelancer ka me shtu projekt t'vetin,
              edhe me qu masi qe e bon formen submit nprofil t vetin me i pa t dhanat */
            if (roli == "Freelancer")
            {
                return RedirectToAction("Index", "FreelancerProfile");
            }

            //qe sosht freelancer i bjen osht admin, edhe e kthen te index
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", project.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", project.CategoryId);

        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", project.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", project.CategoryId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,ProjectName,FileName,Description,ProjectWebsite,CategoryId,UserId")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(project);
                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Project '{project.ProjectName}' was edited",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.ProjectId))
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

            if (roli == "Freelancer")
            {
                return RedirectToAction("Index", "FreelancerProfile");
            }

            //perndryshe, nese sosht freelancer i bjen osht admin edhe veq e kthen te index i projektev
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", project.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", project.CategoryId);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ApplicationUser)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'DataContext.Projects'  is null.");
            }
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Project '{project.ProjectName}' was deleted",
                    ActivityDate = DateTime.Now
                };

                _context.Activities.Add(activityLog);
            }

    

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];

            if (roli == "Freelancer")
            {
                return RedirectToAction("Index", "FreelancerProfile");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
          return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
