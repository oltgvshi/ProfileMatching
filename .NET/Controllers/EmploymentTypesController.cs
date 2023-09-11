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
    public class EmploymentTypesController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmploymentTypesController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmploymentTypes
        public async Task<IActionResult> Index()
        {
              return View(await _context.EmploymentTypes.ToListAsync());
        }

        // GET: EmploymentTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EmploymentTypes == null)
            {
                return NotFound();
            }

            var employmentType = await _context.EmploymentTypes
                .FirstOrDefaultAsync(m => m.EmploymentTypeId == id);
            if (employmentType == null)
            {
                return NotFound();
            }

            return View(employmentType);
        }

        // GET: EmploymentTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmploymentTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmploymentTypeId,EmploymentTypeName")] EmploymentType employmentType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employmentType);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Employment Type '{employmentType.EmploymentTypeName}' was created",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employmentType);
        }

        // GET: EmploymentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmploymentTypes == null)
            {
                return NotFound();
            }

            var employmentType = await _context.EmploymentTypes.FindAsync(id);
            if (employmentType == null)
            {
                return NotFound();
            }
            return View(employmentType);
        }

        // POST: EmploymentTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmploymentTypeId,EmploymentTypeName")] EmploymentType employmentType)
        {
            if (id != employmentType.EmploymentTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employmentType);

                    var activityLog = new Activity
                    {
                        UserId = _userManager.GetUserId(HttpContext.User),
                        ActivityDescription = $"Employment Type '{employmentType.EmploymentTypeName}' was edited",
                        ActivityDate = DateTime.Now
                    };
                    _context.Activities.Add(activityLog);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmploymentTypeExists(employmentType.EmploymentTypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employmentType);
        }

        // GET: EmploymentTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EmploymentTypes == null)
            {
                return NotFound();
            }

            var employmentType = await _context.EmploymentTypes
                .FirstOrDefaultAsync(m => m.EmploymentTypeId == id);
            if (employmentType == null)
            {
                return NotFound();
            }

            return View(employmentType);
        }

        // POST: EmploymentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EmploymentTypes == null)
            {
                return Problem("Entity set 'DataContext.EmploymentTypes'  is null.");
            }
            var employmentType = await _context.EmploymentTypes.FindAsync(id);
            if (employmentType != null)
            {
                _context.EmploymentTypes.Remove(employmentType);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Employment Type '{employmentType.EmploymentTypeName}' was deleted",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmploymentTypeExists(int id)
        {
          return _context.EmploymentTypes.Any(e => e.EmploymentTypeId == id);
        }
    }
}
