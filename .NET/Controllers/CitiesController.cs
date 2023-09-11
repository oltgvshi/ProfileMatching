using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CitiesController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CitiesController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: Cities
        public async Task<IActionResult> Index(int pg=1)
        {
            List<City> cities = _context.Cities.ToList();

            const int pageSize = 8;
            if(pg < 1)
            {
                pg = 1;
            }

            int recsCount = cities.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = cities.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CityId,CityName")] City city)
        {
           // if (ModelState.IsValid)
           // {
                _context.Add(city);

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"City '{city.CityName}' was created",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
           // }
            return View(city);
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CityId,CityName")] City city)
        {
            if (id != city.CityId)
            {
                return NotFound();
            }

          //  if (ModelState.IsValid)
          //  {
                try
                {
                    var activityLog = new Activity
                    {
                        UserId = _userManager.GetUserId(HttpContext.User),
                        ActivityDescription = $"City '{city.CityName}' was edited",
                        ActivityDate = DateTime.Now
                    };
                    _context.Activities.Add(activityLog);
                    _context.Update(city);

                await _context.SaveChangesAsync();
               }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.CityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
           // }
            return View(city);
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cities == null)
            {
                return Problem("Entity set 'DataContext.Cities'  is null.");
            }
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"City '{city.CityName}' was deleted",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);
            }

            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
          return _context.Cities.Any(e => e.CityId == id);
        }
    }
}
