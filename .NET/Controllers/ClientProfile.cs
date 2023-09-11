using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ProfileMatching.Controllers
{

    [Authorize]
    public class ClientProfile : Controller
    {

        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientProfile(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin, Client")]
        public IActionResult Index()
        {
            //id t userit qe osht logged in
            var userId = _userManager.GetUserId(HttpContext.User);

            var clientInfo = _context.ClientDetails.Include(f => f.ApplicationUser)
           .Include(f => f.City)
           .FirstOrDefault(f => f.UserId == userId);

            /*nese ndatabaz clientDetails jon empty, i bjen qe hala klienti ska shti detials per veten,
              kshtuqe t'shtin niher me shku te Create form per clientDetails para sa me shku tani te profili*/
            if (clientInfo == null)
            {
                return RedirectToAction("Create", "ClientDetails");
            }

            //t dhanat prej tabelave tjera meqe smujm mi bo return, veq i bartim me viewData
            var jobpost = _context.JobPosts.Include(f => f.Category).Where(f => f.UserId == userId).ToList();
            ViewData["JobPosts"] = jobpost;

            return View(clientInfo);
        }
        public IActionResult View(string? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users.Where(x => x.Id == userId).First();

            //i bartim id te puneve qe useri has already applied for me ni viewdata
            var appliedJobs = _context.ApplicantsPerJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["AppliedJobs"] = appliedJobs;

            //i bartim id te puneve qe useri has already saves for me ni viewdata
            var savedJobs = _context.SavedJobs.Where(x => x.UserId == userId).Select(x => x.JobPostId).ToList();
            ViewData["SavedJobs"] = savedJobs;

            var clientInfo = _context.ClientDetails.Include(f => f.ApplicationUser)
           .Include(f => f.City)
           .FirstOrDefault(f => f.UserId == id);

            /*nese ndatabaz clientDetails jon empty, i bjen qe hala klienti ska shti detials per veten,
              kshtuqe t'shtin niher me shku te Create form per clientDetails para sa me shku tani te profili*/
            if (clientInfo == null)
            {
                return RedirectToAction("Create", "ClientDetails");
            }

            //t dhanat prej tabelave tjera meqe smujm mi bo return, veq i bartim me viewData
            var jobpost = _context.JobPosts.Include(f => f.Category).Where(f => f.UserId == id).ToList();
            ViewData["JobPosts"] = jobpost;

            return View(clientInfo);
        }

        public IActionResult HiredApplicants()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var clientJobPosts = _context.JobPosts.Include(f => f.Category).Where(f => f.UserId == userId).ToList();

            if (clientJobPosts == null)
            {
                // Handle the case where no job posts were found for the client
                // You can return an appropriate view or redirect to another page
                return View("NoJobPosts");
            }

            var hiredApplicants = _context.ApplicantsPerJobs
                .Include(apj => apj.ApplicationUser)
                .ToList() // Fetch all the ApplicantsPerJobs records from the database
                .Where(apj => clientJobPosts.Any(jp => jp.JobPostId == apj.JobPostId) && apj.HiredStatus)
                .ToList();

            return View(hiredApplicants);
        }



    }
}
