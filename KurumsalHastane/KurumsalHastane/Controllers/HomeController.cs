using KurumsalHastane.Data;
using KurumsalHastane.Entities;
using KurumsalHastane.Infrastructure.PasswordHasher;
using KurumsalHastane.Models.DashboardModels;
using KurumsalHastane.Models.HomeModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KurumsalHastane.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IPasswordHasherService _passwordHasherService;

        public HomeController(ApplicationDBContext context, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _passwordHasherService = passwordHasherService;
        }
        public IActionResult IndexDashboard()
        {
            var model = new HomeModel();
            return View(model);
        }
        public IActionResult Index()
        {
            var model = new HomeModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult SignIn(SignInModel model)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email.Equals(model.Email));
            if(user is not null)
            {
                var hashedPasswordVerified = _passwordHasherService.VerifyPassword(user.PasswordHash, model.Password);
                if (hashedPasswordVerified)
                {
                    // Store user information in session
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserName", user.FirstName+ " " + user.LastName);
                    HttpContext.Session.SetString("UserImageSrc", user.PicturePath);
                    return RedirectToAction(nameof(Dashboard));
                }
                
            }
           
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {


            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
           return View();
        }

        public IActionResult Dashboard()
        {
            var model = new DashboardModel();
            model.Departments = _context.Departments.ToList();
            model.DepartmentsCount = _context.Departments.Count();

            model.Appointments = (_context.Appointments.Where(a=>a.AppointnentDate> DateTime.UtcNow).Include(a=>a.Patient).Include(a => a.Department).Include(a => a.Specialist).ToList()).Take(5).ToList();
            model.AppointmentsCount = _context.Appointments.Count();

            model.Patients = (_context.Users.Where(a => a.RoleId == (int)UserRole.Patient)).ToList().TakeLast(5).ToList();
            model.PatientsCount = _context.Users.Where(a => a.RoleId == (int)UserRole.Patient).Count();

            model.Specialists = (_context.Users.Where(a => a.RoleId == (int)UserRole.Specialist)).ToList().TakeLast(5).ToList();
            model.SpecialtsCount = _context.Users.Where(a => a.RoleId == (int)UserRole.Specialist).Count();

            return View(model);
        }
    }
}
