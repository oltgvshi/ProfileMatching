using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;


namespace ProfileMatching.Controllers
{
    [Authorize]
    public class FreelancersController : Controller
    {

        private readonly DataContext _context;


        public FreelancersController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg=1)
        {
            ViewData["Categories"] = _context.Categories.ToList();
            //^we only needed this per at butonin me select n'baze t'kategorise per me filtru freelancers

            List<FreelancerDetails> freelancers = _context.FreelancerDetails.Include(f => f.ApplicationUser).Include(f => f.Category).Include(f => f.City).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = freelancers.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = freelancers.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);

        }



        //GET SUGGESTED FREELANCERS FOR A JOB BASED ON THE JOB'S CATEGORY
        [Authorize(Roles = "Admin, Client")]
        public async Task<IActionResult> Suggested(int? id)
        {
            ViewData["Categories"] = _context.Categories.ToList();

            //i marrim freelancers qe e kan categoryId t njejt me kategorine qe e ka puna
            var freelancers = _context.FreelancerDetails
                .Include(f => f.ApplicationUser)
                .Include(f => f.Category).Include(f => f.City)
                .Where(f => f.Category.CategoryId == id);

            if (freelancers == null)
            {
                return NotFound();
            }
            return View(await freelancers.ToListAsync());
        }



        public async Task<IActionResult> Filtered(int? id)
        {
            ViewData["Categories"] = _context.Categories.ToList();

            //qitu veq po e marrim emrin e kategorise te zgjedhne per me filtru, n'baze te id-s se kategorise
            var category = _context.Categories.Where(x => x.CategoryId == id).First();
            ViewData["FilteredCategoryName"] = category.CategoryName;

            //i kthen freelancers me qat kategori
            var freelancers = _context.FreelancerDetails
                .Include(f => f.ApplicationUser)
                .Include(f => f.Category).Include(f => f.City)
                .Where(f => f.Category.CategoryId == id);

            if (freelancers == null)
            {
                return NotFound();
            }
            return View(await freelancers.ToListAsync());
        }

        public async Task<IActionResult> SearchFreelancers(string teksti)
        {
            ViewData["Categories"] = _context.Categories.ToList();

            ViewData["SearchInput"] = teksti;

            //tekstin qe e ka shkru perdoruesi n'search input, kqyrim per kolona t ndryshme t tabeles se a e permbajne
            var dataContext = _context.FreelancerDetails.Include(f => f.ApplicationUser).Include(f => f.Category).Include(f => f.City);
            var result = dataContext.Where(f => f.ApplicationUser.UserName.ToLower().Contains(teksti.ToLower())
                || f.ApplicationUser.LastName.ToLower().Contains(teksti.ToLower())
                || f.City.CityName.ToLower().Contains(teksti.ToLower())
                || f.Category.CategoryName.ToLower().Contains(teksti.ToLower()));

            return View(await result.ToListAsync());
        }
    }
}
