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
    public class ClientDetailsController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientDetailsController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ClientDetails
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg=1)
        {
            List<ClientDetail> clientDetails = _context.ClientDetails.Include(c => c.ApplicationUser).Include(c => c.City).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = clientDetails.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = clientDetails.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);
        }

        // GET: ClientDetails/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ClientDetails == null)
            {
                return NotFound();
            }

            //marrim veq details t client me id qe e pranon metoda
            var clientDetail = await _context.ClientDetails
                .Include(c => c.ApplicationUser)
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.ClientDetailsId == id);

            if (clientDetail == null)
            {
                return NotFound();
            }

            return View(clientDetail);
        }

        // GET: ClientDetails/Create
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Create()
        {
            //e marrim id t klientit logged in
            var userId = _userManager.GetUserId(HttpContext.User);

            //nihere po e kqyrim se klienti qe eshte logged in a ka clientDetails n'databaz
            var clientInfo = _context.ClientDetails.Include(f => f.ApplicationUser)
           .Include(f => f.City)
           .FirstOrDefault(f => f.UserId == userId);

            /*nese klienti already ka shti clientDetails n'databaz, e kthen te profili,
             dmth spo dojm me leju me shku te create form prap, se veq ni here ka t drejte
             me shti clientDetails per veten n'databaz */
            if (clientInfo != null)
            {
                return RedirectToAction("Index", "ClientProfile");
            }

            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: ClientDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Create([Bind("ClientDetailsId,UserId,Position,CompanyName,Description,CityId")] ClientDetail clientDetail)
        {
            _context.Add(clientDetail);

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"Client Details '{clientDetail.ClientDetailsId}' was created",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];

            /*po kqyrim a osht roli i userit qe pe perdor create form klient,
              qe dmth nese klienti i ka shti per veten t dhanat,
              me qu masi qe e bon formen submit nprofil t vetin me i pa t dhanat */
            if (roli == "Client")
            {
                return RedirectToAction("Index", "ClientProfile");
            }

            //perndryshe nese se ka rolin client, i bjen qe osht Admin edhe shkon te indexi
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", clientDetail.UserId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", clientDetail.CityId);
            
        }

        // GET: ClientDetails/Edit/5
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();
            var RolesForUser = await _userManager.GetRolesAsync(user);
            var roli = RolesForUser[0];
            ViewData["Role"] = roli;

            if (id == null || _context.ClientDetails == null)
            {
                return NotFound();
            }

            var clientDetail = await _context.ClientDetails.FindAsync(id);
            if (clientDetail == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", clientDetail.UserId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityName", clientDetail.CityId);
            return View(clientDetail);
        }

        // POST: ClientDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Edit(int id, [Bind("ClientDetailsId,UserId,Position,CompanyName,Description,CityId")] ClientDetail clientDetail)
        {
            if (id != clientDetail.ClientDetailsId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(clientDetail);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Client Details '{clientDetail.ClientDetailsId}' was edited",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientDetailExists(clientDetail.ClientDetailsId))
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

            /*po kqyrim a osht roli i userit qe pe perdor edit form klient,
              qe dmth nese klienti i ka edit per veten t dhanat,
              me qu masi qe e bon formen submit nprofil t vetin me i pa t dhanat */
            if (roli == "Client")
            {
                return RedirectToAction("Index", "ClientProfile");
            }

            //perndryshe nese se ka rolin client, i bjen qe osht Admin edhe shkon te indexi
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", clientDetail.UserId);
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", clientDetail.CityId);
        }

        // GET: ClientDetails/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ClientDetails == null)
            {
                return NotFound();
            }

            var clientDetail = await _context.ClientDetails
                .Include(c => c.ApplicationUser)
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.ClientDetailsId == id);
            if (clientDetail == null)
            {
                return NotFound();
            }

            return View(clientDetail);
        }

        // POST: ClientDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ClientDetails == null)
            {
                return Problem("Entity set 'DataContext.ClientDetails'  is null.");
            }
            var clientDetail = await _context.ClientDetails.FindAsync(id);
            if (clientDetail != null)
            {
                _context.ClientDetails.Remove(clientDetail);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Client Details '{clientDetail.ClientDetailsId}' was deleted",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientDetailExists(int id)
        {
          return _context.ClientDetails.Any(e => e.ClientDetailsId == id);
        }
    }
}
