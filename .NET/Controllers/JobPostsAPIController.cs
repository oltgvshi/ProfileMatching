using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostsAPIController : ControllerBase
    {
        private readonly DataContext _context;

        public JobPostsAPIController(DataContext context)
        {
            _context = context;
        }

        // GET: api/JobPostsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobPosts()
        {
            var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            var jobPosts = await _context.JobPosts.ToListAsync();

            // Include only the name claim in the response JSON
            var responseData = new
            {
                JobPosts = jobPosts,
                NameClaim = nameClaim?.Value // Use null conditional operator to avoid null reference exception
            };

            return Ok(responseData);
        }

        [HttpGet("getUserCredentials")]
        public async Task<ActionResult<IEnumerable<UserCredentials>>> GetUserCredentials()
        {
            var userCredentials = await _context.UserCredentials.ToListAsync();

            if (userCredentials == null || userCredentials.Count == 0)
            {
                return NotFound(); // Or any other desired response, such as Ok(null)
            }

            return Ok(userCredentials);
        }

        [HttpGet("GetFullName")]
        public async Task<ActionResult<string>> GetFullName()
        {
            var userCredentials = await _context.UserCredentials.FirstOrDefaultAsync();

            if (userCredentials == null)
            {
                return NotFound(); // Or any other desired response, such as Ok(null)
            }

            ApplicationUser applicationUser = null;
            if (userCredentials.UserRole == "Freelancer")
            {
                var freelancerDetails = await _context.FreelancerDetails
                    .Include(fd => fd.ApplicationUser)
                    .FirstOrDefaultAsync(fd => fd.UserId == userCredentials.UserId);

                if (freelancerDetails != null)
                {
                    applicationUser = freelancerDetails.ApplicationUser;
                }
            }
            else if (userCredentials.UserRole == "Client")
            {
                var clientDetails = await _context.ClientDetails
                    .Include(cd => cd.ApplicationUser)
                    .FirstOrDefaultAsync(cd => cd.UserId == userCredentials.UserId);

                if (clientDetails != null)
                {
                    applicationUser = clientDetails.ApplicationUser;
                }
            }

            if (applicationUser == null)
            {
                return NotFound(); // Or any other desired response, such as Ok(null)
            }

            var fullName = $"{applicationUser.FirstName} {applicationUser.LastName}";

            return Ok(fullName);
        }


        [HttpGet("GetJobClientName/{userId}")]
        public async Task<ActionResult<string>> GetJobClientName (string userId)
        {
            var clientDetails = await _context.ClientDetails
                            .Include(cd => cd.ApplicationUser)
                            .FirstOrDefaultAsync(cd => cd.UserId == userId);

            var applicationUser = clientDetails.ApplicationUser;

            if (clientDetails == null)
            {
                return NotFound(); // Or any other desired response, such as Ok(null)
            }

            var fullName = $"{applicationUser.FirstName} {applicationUser.LastName}";

            return Ok(fullName);
        }





        // GET: api/JobPostsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPost>> GetJobPost(int id)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);

            if (jobPost == null)
            {
                return NotFound();
            }

            return jobPost;
        }

        // PUT: api/JobPostsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobPost(int id, JobPost jobPost)
        {
            if (id != jobPost.JobPostId)
            {
                return BadRequest();
            }

            _context.Entry(jobPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostExists(id))
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

        // POST: api/JobPostsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JobPost>> PostJobPost(JobPost jobPost)
        {
            _context.JobPosts.Add(jobPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobPost", new { id = jobPost.JobPostId }, jobPost);
        }

        // DELETE: api/JobPostsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobPost(int id)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost == null)
            {
                return NotFound();
            }

            _context.JobPosts.Remove(jobPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobPostExists(int id)
        {
            return _context.JobPosts.Any(e => e.JobPostId == id);
        }
    }
}
