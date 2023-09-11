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
    public class FreelancerDetailsController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FreelancerDetailsController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FreelancerDetails
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg=1)
        {
            List<FreelancerDetails> freelancerDetails = _context.FreelancerDetails.Include(f => f.ApplicationUser).Include(f => f.Category).Include(f => f.City).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = freelancerDetails.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = freelancerDetails.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);

        }

        // GET: FreelancerDetails/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FreelancerDetails == null)
            {
                return NotFound();
            }

            var freelancerDetails = await _context.FreelancerDetails
                .Include(f => f.ApplicationUser)
                .Include(f => f.Category)
                .Include(f => f.City)
                .FirstOrDefaultAsync(m => m.FreelancerDetailsId == id);
            if (freelancerDetails == null)
            {
                return NotFound();
            }

            return View(freelancerDetails);
        }

        // GET: FreelancerDetails/Create
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            //nihere po e kqyrim se freelancer qe eshte logged in a ka freelancerDetails n'databaz
            var freeLancerInfo = _context.FreelancerDetails.Include(f => f.ApplicationUser)
           .Include(f => f.City)
           .Include(f => f.Category)
           .FirstOrDefault(f => f.UserId == userId);

            /*nese freelancer already ka shti freelancerDetails n'databaz, e kthen te profili,
             dmth spo dojm me leju me shku te create form prap, se veq ni here ka t drejte
             me shti freelancerDetails per veten n'databaz */
            if (freeLancerInfo != null)
            {
                return RedirectToAction("Index", "FreelancerProfile");
            }

            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "FirstName");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: FreelancerDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Create([Bind("FreelancerDetailsId,CategoryId,CityId,HourlyRate,Languages,Overview,UserId")] FreelancerDetails freelancerDetails)
        {

            _context.Add(freelancerDetails);

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"Freelancer Details '{freelancerDetails.FreelancerDetailsId}' was created",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];

            /*po kqyrim a osht roli i userit qe pe perdor create form freelancer,
              qe dmth nese freelancer i ka shti per veten t dhanat,
              me qu masi qe e bon formen submit nprofil t vetin me i pa t dhanat */
            if (roli == "Freelancer")
            {
                return RedirectToAction("Index", "FreelancerProfile");
            }

            //perndryshe nese se ka rolin freelancer, i bjen qe osht Admin edhe shkon te indexi
            return RedirectToAction(nameof(Index));
          
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerDetails.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", freelancerDetails.CategoryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", freelancerDetails.CityId);
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        // GET: FreelancerDetails/Edit/5
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            if (id == null || _context.FreelancerDetails == null)
            {
                return NotFound();
            }

            var freelancerDetails = await _context.FreelancerDetails.FindAsync(id);
            if (freelancerDetails == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "FirstName", freelancerDetails.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", freelancerDetails.CategoryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", freelancerDetails.CityId);
            return View(freelancerDetails);
        }

        // POST: FreelancerDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Freelancer")]
        public async Task<IActionResult> Edit(int id, [Bind("FreelancerDetailsId,CategoryId,CityId,HourlyRate,Languages,Overview,UserId")] FreelancerDetails freelancerDetails)
        {
            if (id != freelancerDetails.FreelancerDetailsId)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(freelancerDetails);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Freelancer Details '{freelancerDetails.FreelancerDetailsId}' was edited",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FreelancerDetailsExists(freelancerDetails.FreelancerDetailsId))
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

            /*po kqyrim a osht roli i userit qe pe perdor edit form freelancer,
             qe dmth nese freelancer i ka edit per veten t dhanat,
             me qu masi qe e bon formen submit nprofil t vetin me i pa t dhanat */
            if (roli == "Freelancer")
            {
                return RedirectToAction("Index", "FreelancerProfile");
            }

            //perndryshe nese se ka rolin freelancer, i bjen qe osht Admin edhe shkon te indexi
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", freelancerDetails.UserId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", freelancerDetails.CategoryId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", freelancerDetails.CityId);
            
        }

        // GET: FreelancerDetails/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FreelancerDetails == null)
            {
                return NotFound();
            }

            var freelancerDetails = await _context.FreelancerDetails
                .Include(f => f.ApplicationUser)
                .Include(f => f.Category)
                .Include(f => f.City)
                .FirstOrDefaultAsync(m => m.FreelancerDetailsId == id);
            if (freelancerDetails == null)
            {
                return NotFound();
            }

            return View(freelancerDetails);
        }

        // POST: FreelancerDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FreelancerDetails == null)
            {
                return Problem("Entity set 'DataContext.FreelancerDetails'  is null.");
            }
            var freelancerDetails = await _context.FreelancerDetails.FindAsync(id);
            if (freelancerDetails != null)
            {
                _context.FreelancerDetails.Remove(freelancerDetails);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Freelancer Details '{freelancerDetails.FreelancerDetailsId}' was deleted",
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

        private bool FreelancerDetailsExists(int id)
        {
          return _context.FreelancerDetails.Any(e => e.FreelancerDetailsId == id);
        }
    }
}
