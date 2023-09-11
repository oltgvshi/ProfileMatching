using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProfileMatching.Controllers;

namespace ProfileMatching.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class Roles : Controller
    {
        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> rolemanager;
        public Roles(DataContext context, RoleManager<IdentityRole> _roleManager)
        {
            _context = context;
            rolemanager = _roleManager;
        }

        public IActionResult Index()
        {
           var roles = rolemanager.Roles.ToList();
           return View(roles);

        }

        //Metoda CreateRole na kthen View ku kemi formen e cila merr emrin e rolit qe deshirojme ta krijojme
        
        public IActionResult CreateRole()
        {
            return View();
        }


        //Metoda CreateRole ne method post na merr si parameter emrin e rolit dhe kerkon qe nese ky rol me kete
        //emer nuk egziston e krijon ne tabele
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {

            if (!await rolemanager.RoleExistsAsync(roleName))
            {

                await rolemanager.CreateAsync(new IdentityRole(roleName));
            }

            return View();
        }

    }


}
