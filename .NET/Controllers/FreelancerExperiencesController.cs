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
using ProfileMatching.Migrations;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Authorize]
    public class FreelancerExperiencesController : Controller
    {
        private readonly DataContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public FreelancerExperiencesController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FreelancerExperiences
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg=1)
        {
            List<Models.FreelancerExperience> freelancerExperiences = _context.FreelancerExperiences.Include(f => f.ApplicationUser).Include(f => f.EmploymentType).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = freelancerExperiences.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = freelancerExperiences.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);
        }

        // GET: FreelancerExperiences/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FreelancerExperiences == null)
            {
                return NotFound();
            }

            var freelancerExperience = await _context.FreelancerExperiences
                .Include(f => f.ApplicationUser)
                .Include(f => f.EmploymentType)
                .FirstOrDefaultAsync(m => m.FreelancerExperienceID == id);
            if (freelancerExperience == null)
            {
                return NotFound();
            }

            return View(freelancerExperience);
        }

        // GET: FreelancerExperiences/Create
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> CreateAsync()
        {
            //kodi qe po na qon rolin e loggedin user te viewbag n view
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "EmploymentTypeId", "EmploymentTypeName");
            return View();
        }

        // POST: FreelancerExperiences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Create([Bind("FreelancerExperienceID,UserId,EmploymentTypeId,CompanyName,StartDate,EndDate")] Models.FreelancerExperience freelancerExperience)
        {

            _context.Add(freelancerExperience);

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"Freelancer Experience '{freelancerExperience.FreelancerExperienceID}' was created",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

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

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerExperience.UserId);
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "EmploymentTypeId", "EmploymentTypeId", freelancerExperience.EmploymentTypeId);
            return View(freelancerExperience);
        }

        // GET: FreelancerExperiences/Edit/5
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int? id)

        {
            //kodi qe po na qon rolin e loggedin user te viewbag n view
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            if (id == null || _context.FreelancerExperiences == null)
            {
                return NotFound();
            }

            var freelancerExperience = await _context.FreelancerExperiences.FindAsync(id);
            if (freelancerExperience == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerExperience.UserId);
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "EmploymentTypeId", "EmploymentTypeName", freelancerExperience.EmploymentTypeId);
            return View(freelancerExperience);
        }

        // POST: FreelancerExperiences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int id, [Bind("FreelancerExperienceID,UserId,EmploymentTypeId,CompanyName,StartDate,EndDate")] Models.FreelancerExperience freelancerExperience)
        {
            if (id != freelancerExperience.FreelancerExperienceID)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(freelancerExperience);

                    var activityLog = new Activity
                    {
                        UserId = _userManager.GetUserId(HttpContext.User),
                        ActivityDescription = $"Freelancer Experience '{freelancerExperience.FreelancerExperienceID}' was edited",
                        ActivityDate = DateTime.Now
                    };
                    _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerExperienceExists(freelancerExperience.FreelancerExperienceID))
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

            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerExperience.UserId);
            ViewData["EmploymentTypeId"] = new SelectList(_context.EmploymentTypes, "EmploymentTypeId", "EmploymentTypeName", freelancerExperience.EmploymentTypeId);
            return View(freelancerExperience);
        }

        // GET: FreelancerExperiences/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FreelancerExperiences == null)
            {
                return NotFound();
            }

            var freelancerExperience = await _context.FreelancerExperiences
                .Include(f => f.ApplicationUser)
                .Include(f => f.EmploymentType)
                .FirstOrDefaultAsync(m => m.FreelancerExperienceID == id);
            if (freelancerExperience == null)
            {
                return NotFound();
            }

            return View(freelancerExperience);
        }

        // POST: FreelancerExperiences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FreelancerExperiences == null)
            {
                return Problem("Entity set 'DataContext.FreelancerExperiences'  is null.");
            }
            var freelancerExperience = await _context.FreelancerExperiences.FindAsync(id);
            if (freelancerExperience != null)
            {
                _context.FreelancerExperiences.Remove(freelancerExperience);
            }

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"Freelancer Experience '{freelancerExperience.FreelancerExperienceID}' was deleted",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

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

        private bool FreelancerExperienceExists(int id)
        {
          return _context.FreelancerExperiences.Any(e => e.FreelancerExperienceID == id);
        }
    }
}
