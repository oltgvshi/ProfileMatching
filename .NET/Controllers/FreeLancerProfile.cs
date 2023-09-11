using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ProfileMatching.Controllers
{
    [Authorize]
    public class FreeLancerProfile : Controller
    {

        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FreeLancerProfile(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin, Freelancer")]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            var freeLancerInfo = _context.FreelancerDetails.Include(f => f.ApplicationUser)
           .Include(f => f.City)
           .Include(f => f.Category)
           .FirstOrDefault(f => f.UserId == userId);

            /*nese ndatabaz freelancerDetails jon empty, i bjen qe hala freelancer ska shti detials per veten,
            kshtuqe t'shtin niher me shku te Create form per freelancerDetails para sa me shku tani te profili*/
            if (freeLancerInfo == null)
            {
                return RedirectToAction("Create", "FreelancerDetails");
            }

            //t dhanat prej tabelave tjera meqe smujm mi bo return, veq i bartim me viewData
            var projekti = _context.Projects.Include(f => f.Category).Where(f => f.UserId == userId).ToList();
            ViewData["Projects"] = projekti;

            var freelancerexperience = _context.FreelancerExperiences.Include(f => f.EmploymentType).Where(f => f.UserId == userId).ToList();
            ViewData["FreeLancerExperiences"] = freelancerexperience;

            var freelancereducation = _context.FreelancerEducations.Where(f => f.UserId == userId).ToList();
            ViewData["FreeLancerEducations"] = freelancereducation;


            return View(freeLancerInfo);
            

        }

        public IActionResult View(string? id)
        {

            var freeLancerInfo = _context.FreelancerDetails.Include(f => f.ApplicationUser)
           .Include(f => f.City)
           .Include(f => f.Category)
           .FirstOrDefault(f => f.UserId == id);

            /*nese ndatabaz freelancerDetails jon empty, i bjen qe hala freelancer ska shti detials per veten,
            kshtuqe t'shtin niher me shku te Create form per freelancerDetails para sa me shku tani te profili*/
            if (freeLancerInfo == null)
            {
                return RedirectToAction("Create", "FreelancerDetails");
            }

            //t dhanat prej tabelave tjera meqe smujm mi bo return, veq i bartim me viewData
            var projekti = _context.Projects.Include(f => f.Category).Where(f => f.UserId == id).ToList();
            ViewData["Projects"] = projekti;

            var freelancerexperience = _context.FreelancerExperiences.Include(f => f.EmploymentType).Where(f => f.UserId == id).ToList();
            ViewData["FreeLancerExperiences"] = freelancerexperience;

            var freelancereducation = _context.FreelancerEducations.Where(f => f.UserId == id).ToList();
            ViewData["FreeLancerEducations"] = freelancereducation;


            return View(freeLancerInfo);


        }
    }
}
