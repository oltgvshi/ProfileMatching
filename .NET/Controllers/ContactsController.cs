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
    public class ContactsController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ContactsController(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Contacts
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int pg=1)
        {


            List<Contact> contacts = _context.Contacts.Include(c => c.ApplicationUser).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = contacts.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = contacts.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();

            return View(data);
        }

        // GET: Contacts/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();

            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,UserId,Message")] Contact contact)
        {
        
                _context.Add(contact);

            var activityLog = new Activity
            {
                UserId = _userManager.GetUserId(HttpContext.User),
                ActivityDescription = $"Contact message '{contact.Message}' was created",
                ActivityDate = DateTime.Now
            };
            _context.Activities.Add(activityLog);

            await _context.SaveChangesAsync();

                var userId = _userManager.GetUserId(HttpContext.User);
                var user = _context.Users.Where(x => x.Id == userId).First();
                var RolesForUser = await _userManager.GetRolesAsync(user);
                var roli = RolesForUser[0];
                ViewData["Role"] = roli;

           
                if (roli == "Freelancer")
                {
                return RedirectToAction("Index", "freelancerprofile");
                }

                if (roli == "Client")
                {
                return RedirectToAction("Index", "clientprofile");
                }


            return RedirectToAction(nameof(Index));

                ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", contact.UserId);
                return View(contact);
        }

        // GET: Contacts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", contact.UserId);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,UserId,Message")] Contact contact)
        {
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
          //  {
                try
                {
                    _context.Update(contact);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Contact message '{contact.Message}' was edited",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);

                await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ContactId))
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
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", contact.UserId);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'DataContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);

                var activityLog = new Activity
                {
                    UserId = _userManager.GetUserId(HttpContext.User),
                    ActivityDescription = $"Contact message '{contact.Message}' was deleted",
                    ActivityDate = DateTime.Now
                };
                _context.Activities.Add(activityLog);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
          return _context.Contacts.Any(e => e.ContactId == id);
        }
    }
}
