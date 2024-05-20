using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KurumsalHastane.Data;
using KurumsalHastane.Entities;
using AutoMapper;
using KurumsalHastane.Infrastructure.PasswordHasher;
using KurumsalHastane.Models.User;
using KurumsalHastane.Models.PatientModels;

namespace KurumsalHastane.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;
        public PatientController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }
        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<PatientModel>();
            var patients = await _context.Users.Where(a=>a.RoleId == (int) UserRole.Patient).ToListAsync();
            foreach (var patient in patients)
            {
                models.Add(_mapper.Map<PatientModel>(patient));
            }

            // Render a partial view with the retrieved data
            return PartialView("_PatientListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(UserSearchModel searchModel)
        {
            if (searchModel is null)
            {

            }
            var models = new List<PatientModel>();
            var patients = await _context.Users.Where(a => a.RoleId == (int)UserRole.Patient).ToListAsync();
            if (searchModel.Name is not null)
            {
                patients = patients.Where(a => a.FirstName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant()) || a.LastName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant())).ToList();
            }
            if (searchModel.Email is not null)
            {
                patients = patients.Where(a => a.Email.ToLowerInvariant().Contains(searchModel.Email.ToLowerInvariant())).ToList();
            }
            foreach (var patient in patients)
            {

                models.Add(_mapper.Map<PatientModel>(patient));
            }

            // Render a partial view with the retrieved data
            return PartialView("_PatientListPartial", models);
        }
        // GET: Patient
        public async Task<IActionResult> Index()
        {
            var models = new List<PatientModel>();
            var patients = await _context.Users.Where(a => a.RoleId == (int)UserRole.Patient).ToListAsync();
            foreach (var patient in patients)
            {
                models.Add(_mapper.Map<PatientModel>(patient));
            }
            return View(models);
        }

        // GET: Patient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<PatientModel>(patient);
            return View(model);
        }

        // GET: Patient/Create
        public IActionResult Create()
        {
            var model = new PatientModel();

            return View(model);
        }

        // POST: Patient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientModel model)
        {
            if (PatientExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var filePath = await FileUploadAsync(model.File);
                model.PicturePath = filePath;
                var passwordHash = _passwordHasherService.HashPassword(model.PasswordHash);

                var patientEntity = _mapper.Map<User>(model);
                patientEntity.PasswordHash = passwordHash;
                patientEntity.RoleId = (int) UserRole.Patient;
                _context.Add(patientEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Users.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<PatientModel>(patient);
            return View(model);
        }

        // POST: Patient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var patient = await _context.Users.FindAsync(model.Id);
            if (patient is not null && ModelState.IsValid)
            {
                var patientEntity = _mapper.Map<User>(model);
                _context.Entry(patient).State = EntityState.Detached;
                try
                {
                    var newHashedpass= _passwordHasherService.HashPassword(model.PasswordHash);
                    if (newHashedpass.Equals(patient.PasswordHash))
                    {
                        patientEntity.PasswordHash = patient.PasswordHash;
                    }
                    else
                    {
                        patientEntity.PasswordHash = newHashedpass;
                    }
                    patientEntity.RoleId = (int)UserRole.Patient;
                    _context.Update(patientEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Patient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<PatientModel>(patient);
            return View(model);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Users.FindAsync(id);
            if (patient != null)
            {
                _context.Users.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public async Task<string> FileUploadAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save the file to the uploads folder
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Store the relative file path in the database
                string relativeFilePath = Path.Combine("uploads", uniqueFileName);
                return relativeFilePath;
            }
            return null;
        }

        public IActionResult GetFile(string relativeFilePath)
        {
            if (string.IsNullOrEmpty(relativeFilePath))
            {
                return NotFound();
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativeFilePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "application/octet-stream"); // Change MIME type as needed
        }
    }
}
