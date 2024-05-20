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
using KurumsalHastane.Models.SpecialistModels;

namespace KurumsalHastane.Controllers
{
    public class SpecialistController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;

        public SpecialistController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }

        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<SpecialistModel>();
            var specialists = await _context.Users.Where(a=>a.RoleId == (int) UserRole.Specialist).ToListAsync();
            foreach (var specialist in specialists)
            {
                models.Add(_mapper.Map<SpecialistModel>(specialist));
            }

            // Render a partial view with the retrieved data
            return PartialView("_SpecialistListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(SpecialistSearchModel searchModel)
        {
            if (searchModel is null)
            {

            }
            var models = new List<SpecialistModel>();
            var specialists = await _context.Users.Where(a => a.RoleId == (int)UserRole.Specialist).ToListAsync();
            if (searchModel.Name is not null)
            {
                specialists = specialists.Where(a => a.FirstName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant()) || a.LastName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant())).ToList();
            }
            if (searchModel.Email is not null)
            {
                specialists = specialists.Where(a => a.Email.ToLowerInvariant().Contains(searchModel.Email.ToLowerInvariant())).ToList();
            }
            foreach (var specialist in specialists)
            {

                models.Add(_mapper.Map<SpecialistModel>(specialist));
            }

            // Render a partial view with the retrieved data
            return PartialView("_SpecialistListPartial", models);
        }
        // GET: Specialist
        public async Task<IActionResult> Index()
        {
            var models = new List<SpecialistModel>();
            var specialists = await _context.Users.Where(a => a.RoleId == (int)UserRole.Specialist).ToListAsync();
            foreach (var specialist in specialists)
            {
                models.Add(_mapper.Map<SpecialistModel>(specialist));
            }
            return View(models);
        }
        public async Task<IActionResult> SpecialistList()
        {
            var models = new List<SpecialistModel>();
            var specialists = await _context.Users.Where(a => a.RoleId == (int)UserRole.Specialist).ToListAsync();
            foreach (var specialist in specialists)
            {
                models.Add(_mapper.Map<SpecialistModel>(specialist));
            }
            return Json(models.ToArray());
        }
        // GET: Specialist/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (specialist == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<SpecialistModel>(specialist);
            model.DepartmentId = _context.SpecialistDepartmentMappings.FirstOrDefault(x => x.SpecialistId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: Specialist/Create
        public async Task<IActionResult> Create()
        {
            var model = new SpecialistModel();
            //model.DepartmentId = _context.SpecialistDepartmentMappings.FirstOrDefault(x => x.SpecialistId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // POST: Specialist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialistModel model)
        {

            if (SpecialistExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var filePath = await FileUploadAsync(model.File);
                model.PicturePath = filePath;
                var passwordHash = _passwordHasherService.HashPassword(model.PasswordHash);

                var specialistEntity = _mapper.Map<User>(model);
                specialistEntity.PasswordHash = passwordHash;
                specialistEntity.RoleId = (int)UserRole.Specialist;
                _context.Add(specialistEntity);
                await _context.SaveChangesAsync();

                var specialitDepartment = new SpecialistDepartmentMapping
                {
                    SpecialistId = specialistEntity.Id,
                    DepartmentId = model.DepartmentId
                };
                _context.Add(specialitDepartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //model.DepartmentId = _context.SpecialistDepartmentMappings.FirstOrDefault(x => x.SpecialistId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: Specialist/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Users.FindAsync(id);
            if (specialist == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<SpecialistModel>(specialist);
            var departmentMapping = _context.SpecialistDepartmentMappings.FirstOrDefault(x => x.SpecialistId == id);
            model.DepartmentId = departmentMapping is not null ? departmentMapping.DepartmentId: 0;
            model.Departments = GetDepartmentList();
           
            return View(model);
        }

        // POST: Specialist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecialistModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var specialist = await _context.Users.FindAsync(model.Id);
            if (specialist is not null && ModelState.IsValid)
            {
                var specialistEntity = _mapper.Map<User>(model);
                _context.Entry(specialist).State = EntityState.Detached;
                try
                {
                    //var newHashedpass = _passwordHasherService.HashPassword(model.PasswordHash);
                    if (specialistEntity.PasswordHash.Equals(specialist.PasswordHash))
                    {
                        specialistEntity.PasswordHash = specialist.PasswordHash;
                    }
                    else
                    {
                        specialistEntity.PasswordHash = _passwordHasherService.HashPassword(model.PasswordHash);
                    }
                    specialistEntity.RoleId = (int)UserRole.Specialist;
                    if(model.File is not null)
                    {
                        var filePath = await FileUploadAsync(model.File);
                        specialistEntity.PicturePath = filePath;
                    }
                    else
                    {
                        specialistEntity.PicturePath = specialist.PicturePath;
                    }
                    _context.Update(specialistEntity);
                    
                    
                        var specialitDepartment = _context.SpecialistDepartmentMappings.FirstOrDefault(a => a.SpecialistId == specialistEntity.Id);
                    if (specialitDepartment is not null) {
                    if(model.DepartmentId != specialitDepartment.DepartmentId)
                    {
                        specialitDepartment.DepartmentId = model.DepartmentId;
                        _context.Update(specialitDepartment);
                    }
                        
                    }
                   else
                        {
                            var _specialitDepartment = new SpecialistDepartmentMapping
                            {
                                SpecialistId = specialistEntity.Id,
                                DepartmentId = model.DepartmentId
                            };
                            _context.SpecialistDepartmentMappings.Add(_specialitDepartment);
                        }
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialistExists(model.Id))
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
            model.DepartmentId = _context.SpecialistDepartmentMappings.FirstOrDefault(x => x.SpecialistId == model.Id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: Specialist/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialist = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (specialist == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<SpecialistModel>(specialist);
            model.DepartmentId = _context.SpecialistDepartmentMappings.FirstOrDefault(x => x.SpecialistId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // POST: Specialist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialist = await _context.Users.FindAsync(id);
            var specilalistDepartment = _context.SpecialistDepartmentMappings.FirstOrDefault(x => x.SpecialistId == id);
            if (specialist != null)
            {
                _context.Users.Remove(specialist);
            }
            if (specilalistDepartment != null)
            {
                _context.SpecialistDepartmentMappings.Remove(specilalistDepartment);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialistExists(int id)
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
        private List<SelectListItem> GetDepartmentList()
        {
            var departmentList = new List<SelectListItem>();
            departmentList.Add(new SelectListItem
            {
                Text = "Select",
                Value = 0.ToString(),
                Selected = true
            });
            var departments = _context.Departments.ToList();
            foreach (var department in departments)
            {
                departmentList.Add(new SelectListItem
                {
                    Text = department.Name,
                    Value = department.Id.ToString()
                });
            }
            return departmentList;
        }
    }
}
