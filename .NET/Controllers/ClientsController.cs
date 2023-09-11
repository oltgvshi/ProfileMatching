using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;

namespace ProfileMatching.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {

        private readonly DataContext _context;


        public ClientsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg=1)
        {
            ViewData["Categories"] = _context.Categories.ToList();
            //^we only needed this per at butonin me select n'baze t'kategorise per me filtru freelancers

            List<ClientDetail> clients = _context.ClientDetails.Include(f => f.ApplicationUser).Include(f => f.City).ToList();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = clients.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = clients.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewData["Pager"] = pager;

            return View(data);
        }



       
        public async Task<IActionResult> SearchClients(string teksti)
        {
            
            ViewData["SearchInput"] = teksti;

            //tekstin qe e ka shkru perdoruesi n'search input, kqyrim per kolona t ndryshme t tabeles se a e permbajne

            var dataContext = _context.ClientDetails.Include(f => f.ApplicationUser).Include(f => f.City);
            var result = dataContext.Where(f => f.ApplicationUser.UserName.ToLower().Contains(teksti.ToLower())
                || f.ApplicationUser.LastName.ToLower().Contains(teksti.ToLower())
                || f.City.CityName.ToLower().Contains(teksti.ToLower())
                || f.Position.ToLower().Contains(teksti.ToLower())
                || f.Description.ToLower().Contains(teksti.ToLower())
                || f.CompanyName.ToLower().Contains(teksti.ToLower())); 
                


            return View(await result.ToListAsync());
        }
    }
}
