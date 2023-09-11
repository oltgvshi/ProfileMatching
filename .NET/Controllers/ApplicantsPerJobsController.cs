using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantsPerJobsController : ControllerBase
    {
        private readonly DataContext _context;

        public ApplicantsPerJobsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/ApplicantsPerJobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicantsPerJob>>> GetApplicantsPerJobs()
        {
            if (_context.ApplicantsPerJobs == null)
            {
                return NotFound();
            }
            return await _context.ApplicantsPerJobs.ToListAsync();
        }

        // GET: api/ApplicantsPerJobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicantsPerJob>> GetApplicantsPerJob(int id)
        {
            if (_context.ApplicantsPerJobs == null)
            {
                return NotFound();
            }
            var applicantsPerJob = await _context.ApplicantsPerJobs.FindAsync(id);

            if (applicantsPerJob == null)
            {
                return NotFound();
            }

            return applicantsPerJob;
        }


        // i kthen jobpostat qe i ka kriju klienti i cili eshte logged in
        // api/ApplicantsPerJobs/GetJobPostsByClientId
        [HttpGet("GetJobpostsByClientId")]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobPostsByClientId()
        {
            // Get the user credentials for the first record
            var userCredentials = await _context.UserCredentials.FirstOrDefaultAsync();

            // Check if userCredentials exist and the user role is "Client"
            if (userCredentials != null && userCredentials.UserRole == "Client")
            {
                var jobPosts = await _context.JobPosts
                    .Where(j => j.UserId == userCredentials.UserId)
                    .ToListAsync();

                if (jobPosts.Count == 0)
                {
                    return NotFound("No job posts found for the client");
                }

                return jobPosts;
            }

            return NotFound("User not logged in!");
        }

        /* metoda qe i kthen punet ku ti je hired at, si freelancer
        */
        // GET: api/ApplicantsPerJobs/GetMyHiredJobs
        [HttpGet("GetMyHiredJobs")]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetMyHiredJobs()
        {
            // Get the user credentials for the first record
            var userCredentials = await _context.UserCredentials.FirstOrDefaultAsync();


            // Check if the logged-in user is a "Freelancer"
            if (userCredentials == null || userCredentials.UserRole != "Freelancer")
            {
                return BadRequest("Only freelancers can access this resource");
            }

            var hiredJobPostIds = await _context.ApplicantsPerJobs
                .Where(a => a.UserId == userCredentials.UserId && a.HiredStatus == true)
                .Select(a => a.JobPostId)
                .ToListAsync();

            var jobPosts = await _context.JobPosts
                .Where(j => hiredJobPostIds.Contains(j.JobPostId))
                .ToListAsync();

            if (jobPosts.Count == 0)
            {
                return NotFound("No hired job posts found for the freelancer");
            }

            return jobPosts;
        }


        /* metoda qe i kthen user data t'aplikanteve qe jane hired ne qat pune
           qikjo mundet me vyjt veq si liste psh nese e bojm ni section t'hired applicants kush osht n'qat pune 
           se perndryshe kur bohesh log in si freelancer, duhet mu kon ni metod tjeter qe ti thirr punt ku je hired
        */
        // GET: api/ApplicantsPerJobs/GetHiredApplicantsForJobPost/5
        [HttpGet("GetHiredApplicantsForJobPost/{jobPostId}")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetHiredApplicantsForJobPost(int jobPostId)
        {
            var hiredApplicants = await _context.ApplicantsPerJobs
                .Where(a => a.JobPostId == jobPostId)
                .Where(a => a.HiredStatus == true)
                .Select(a => a.ApplicationUser)
                .ToListAsync();

            if (hiredApplicants == null)
            {
                return NotFound();
            }

            return hiredApplicants;
        }

        // PUT: api/ApplicantsPerJobs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicantsPerJob(int id, ApplicantsPerJob applicantsPerJob)
        {
            if (id != applicantsPerJob.ApplicantPerJobId)
            {
                return BadRequest();
            }

            _context.Entry(applicantsPerJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicantsPerJobExists(id))
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

        // POST: api/ApplicantsPerJobs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApplicantsPerJob>> PostApplicantsPerJob(ApplicantsPerJob applicantsPerJob)
        {
            if (_context.ApplicantsPerJobs == null)
            {
                return Problem("Entity set 'DataContext.ApplicantsPerJobs'  is null.");
            }
            _context.ApplicantsPerJobs.Add(applicantsPerJob);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplicantsPerJob", new { id = applicantsPerJob.ApplicantPerJobId }, applicantsPerJob);
        }

        // DELETE: api/ApplicantsPerJobs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicantsPerJob(int id)
        {
            if (_context.ApplicantsPerJobs == null)
            {
                return NotFound();
            }
            var applicantsPerJob = await _context.ApplicantsPerJobs.FindAsync(id);
            if (applicantsPerJob == null)
            {
                return NotFound();
            }

            _context.ApplicantsPerJobs.Remove(applicantsPerJob);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicantsPerJobExists(int id)
        {
            return (_context.ApplicantsPerJobs?.Any(e => e.ApplicantPerJobId == id)).GetValueOrDefault();
        }
    }
}