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
    public class FreelancerEducationsController : Controller
    {
        private readonly DataContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public FreelancerEducationsController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FreelancerEducations
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg=1)
        {
            List<FreelancerEducation> freelancerEducations = _context.FreelancerEducations.Include(f => f.ApplicationUser).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = freelancerEducations.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = freelancerEducations.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);

        }

        // GET: FreelancerEducations/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FreelancerEducations == null)
            {
                return NotFound();
            }

            var freelancerEducation = await _context.FreelancerEducations
                .Include(f => f.ApplicationUser)
                .FirstOrDefaultAsync(m => m.FreelancerEducationid == id);
            if (freelancerEducation == null)
            {
                return NotFound();
            }

            return View(freelancerEducation);
        }

        // GET: FreelancerEducations/Create
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
            return View();
        }

        // POST: FreelancerEducations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Create([Bind("FreelancerEducationid,InstituteName,Degree,FieldOfStudy,StartDate,EndDate,UserId")] FreelancerEducation freelancerEducation)
        {

            _context.Add(freelancerEducation);

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"Freelancer Education '{freelancerEducation.FreelancerEducationid}' was created",
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

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerEducation.UserId);
            return View(freelancerEducation);
        }

        // GET: FreelancerEducations/Edit/5
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int? id)
        {
            //kodi qe po na qon rolin e loggedin user te viewbag n view
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            if (id == null || _context.FreelancerEducations == null)
            {
                return NotFound();
            }

            var freelancerEducation = await _context.FreelancerEducations.FindAsync(id);
            if (freelancerEducation == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerEducation.UserId);
            return View(freelancerEducation);
        }

        // POST: FreelancerEducations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int id, [Bind("FreelancerEducationid,InstituteName,Degree,FieldOfStudy,StartDate,EndDate,UserId")] FreelancerEducation freelancerEducation)
        {
            if (id != freelancerEducation.FreelancerEducationid)
            {
                return NotFound();
            }

            //  if (ModelState.IsValid)
            // {
            try
            {
                    _context.Update(freelancerEducation);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Freelancer Education '{freelancerEducation.FreelancerEducationid}' was edited",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerEducationExists(freelancerEducation.FreelancerEducationid))
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
            // }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerEducation.UserId);
            return View(freelancerEducation);
        }

        // GET: FreelancerEducations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FreelancerEducations == null)
            {
                return NotFound();
            }

            var freelancerEducation = await _context.FreelancerEducations
                .Include(f => f.ApplicationUser)
                .FirstOrDefaultAsync(m => m.FreelancerEducationid == id);
            if (freelancerEducation == null)
            {
                return NotFound();
            }

            return View(freelancerEducation);
        }

        // POST: FreelancerEducations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FreelancerEducations == null)
            {
                return Problem("Entity set 'DataContext.FreelancerEducations'  is null.");
            }
            var freelancerEducation = await _context.FreelancerEducations.FindAsync(id);
            if (freelancerEducation != null)
            {
                _context.FreelancerEducations.Remove(freelancerEducation);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Freelancer Education '{freelancerEducation.FreelancerEducationid}' was deleted",
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

        private bool FreelancerEducationExists(int id)
        {
          return _context.FreelancerEducations.Any(e => e.FreelancerEducationid == id);
        }
    }
}
