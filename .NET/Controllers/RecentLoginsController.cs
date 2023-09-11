using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecentLoginsController : ControllerBase
    {
        private readonly DataContext _context;

        public RecentLoginsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/RecentLogins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecentLogin>>> GetRecentLogins()
        {
          if (_context.RecentLogins == null)
          {
              return NotFound();
          }
            return await _context.RecentLogins.ToListAsync();
        }

        // GET: api/RecentLogins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecentLogin>> GetRecentLogin(int id)
        {
          if (_context.RecentLogins == null)
          {
              return NotFound();
          }
            var recentLogin = await _context.RecentLogins.FindAsync(id);

            if (recentLogin == null)
            {
                return NotFound();
            }

            return recentLogin;
        }

        // PUT: api/RecentLogins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecentLogin(int id, RecentLogin recentLogin)
        {
            if (id != recentLogin.RecentLoginID)
            {
                return BadRequest();
            }

            _context.Entry(recentLogin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecentLoginExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RecentLogins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RecentLogin>> PostRecentLogin(RecentLogin recentLogin)
        {
          if (_context.RecentLogins == null)
          {
              return Problem("Entity set 'DataContext.RecentLogins'  is null.");
          }
            _context.RecentLogins.Add(recentLogin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecentLogin", new { id = recentLogin.RecentLoginID }, recentLogin);
        }

        // DELETE: api/RecentLogins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecentLogin(int id)
        {
            if (_context.RecentLogins == null)
            {
                return NotFound();
            }
            var recentLogin = await _context.RecentLogins.FindAsync(id);
            if (recentLogin == null)
            {
                return NotFound();
            }

            _context.RecentLogins.Remove(recentLogin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecentLoginExists(int id)
        {
            return (_context.RecentLogins?.Any(e => e.RecentLoginID == id)).GetValueOrDefault();
        }
    }
}
