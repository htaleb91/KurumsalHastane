using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KurumsalHastane.Data;
using KurumsalHastane.Entities;
using KurumsalHastane.Models.NurseModels;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using KurumsalHastane.Infrastructure.PasswordHasher;

namespace KurumsalHastane.Controllers
{
    public class NurseController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;

        public NurseController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }

        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<NurseModel>();
            var nurses = await _context.Users.Where(a => a.RoleId == (int)UserRole.Nurse).ToListAsync();
            foreach (var nurse in nurses)
            {
                models.Add(_mapper.Map<NurseModel>(nurse));
            }

            // Render a partial view with the retrieved data
            return PartialView("_NurseListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(NurseSearchModel searchModel)
        {
            if (searchModel is null)
            {

            }
            var models = new List<NurseModel>();
            var nurses = await _context.Users.Where(a => a.RoleId == (int)UserRole.Nurse).ToListAsync();
            if (searchModel.Name is not null)
            {
                nurses = nurses.Where(a => a.FirstName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant()) || a.LastName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant())).ToList();
            }
            if (searchModel.Email is not null)
            {
                nurses = nurses.Where(a => a.Email.ToLowerInvariant().Contains(searchModel.Email.ToLowerInvariant())).ToList();
            }
            foreach (var nurse in nurses)
            {

                models.Add(_mapper.Map<NurseModel>(nurse));
            }

            // Render a partial view with the retrieved data
            return PartialView("_NurseListPartial", models);
        }
        // GET: Nurse
        public async Task<IActionResult> Index()
        {
            var models = new List<NurseModel>();
            var nurses = await _context.Users.Where(a => a.RoleId == (int)UserRole.Nurse).ToListAsync();
            foreach (var nurse in nurses)
            {
                models.Add(_mapper.Map<NurseModel>(nurse));
            }
            return View(models);
        }
        public async Task<IActionResult> NurseList()
        {
            var models = new List<NurseModel>();
            var nurses = await _context.Users.Where(a => a.RoleId == (int)UserRole.Nurse).ToListAsync();
            foreach (var nurse in nurses)
            {
                models.Add(_mapper.Map<NurseModel>(nurse));
            }
            return Json(models.ToArray());
        }
        // GET: Nurse/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (nurse == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<NurseModel>(nurse);

            model.DepartmentId = _context.NurseDepartmentMappings.FirstOrDefault(x => x.NurseId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: Nurse/Create
        public async Task<IActionResult> Create()
        {
            var model = new NurseModel();
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // POST: Nurse/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NurseModel model)
        {

            if (NurseExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var filePath = await FileUploadAsync(model.File);
                model.PicturePath = filePath;
                var passwordHash = _passwordHasherService.HashPassword(model.PasswordHash);

                var nurseEntity = _mapper.Map<User>(model);
                nurseEntity.PasswordHash = passwordHash;
                nurseEntity.RoleId = (int)UserRole.Nurse;
                _context.Add(nurseEntity);
                await _context.SaveChangesAsync();

                var nurseDepartment = new NurseDepartmentMapping
                {
                    NurseId = nurseEntity.Id,
                    DepartmentId = model.DepartmentId
                };
                _context.Add(nurseDepartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: Nurse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Users.FindAsync(id);
            if (nurse == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<NurseModel>(nurse);

            var departmentMapping = _context.NurseDepartmentMappings.FirstOrDefault(x => x.NurseId == id);
            model.DepartmentId = departmentMapping is not null ? departmentMapping.DepartmentId : 0;
            model.Departments = GetDepartmentList();

            return View(model);
        }

        // POST: Nurse/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NurseModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var nurse = await _context.Users.FindAsync(model.Id);
            if (nurse is not null && ModelState.IsValid)
            {
                var nurseEntity = _mapper.Map<User>(model);
                _context.Entry(nurse).State = EntityState.Detached;
                try
                {
                    //var newHashedpass = _passwordHasherService.HashPassword(model.PasswordHash);
                    if (nurseEntity.PasswordHash.Equals(nurse.PasswordHash))
                    {
                        nurseEntity.PasswordHash = nurse.PasswordHash;
                    }
                    else
                    {
                        nurseEntity.PasswordHash = _passwordHasherService.HashPassword(model.PasswordHash);
                    }
                    nurseEntity.RoleId = (int)UserRole.Nurse;
                    if (model.File is not null)
                    {
                        var filePath = await FileUploadAsync(model.File);
                        nurseEntity.PicturePath = filePath;
                    }
                    else
                    {
                        nurseEntity.PicturePath = nurse.PicturePath;
                    }
                    _context.Update(nurseEntity);
                    var nurseDepartment = _context.NurseDepartmentMappings.FirstOrDefault(a => a.NurseId == nurseEntity.Id);
                    if (nurseDepartment is not null)
                    {
                        if (model.DepartmentId != nurseDepartment.DepartmentId)
                        {
                            nurseDepartment.DepartmentId = model.DepartmentId;
                            _context.Update(nurseDepartment);
                        }

                    }
                    else
                    {
                        var _nurseDepartment = new NurseDepartmentMapping
                        {
                            NurseId = nurseEntity.Id,
                            DepartmentId = model.DepartmentId
                        };
                        _context.NurseDepartmentMappings.Add(_nurseDepartment);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NurseExists(model.Id))
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
            model.DepartmentId = _context.NurseDepartmentMappings.FirstOrDefault(x => x.NurseId == model.Id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: Nurse/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (nurse == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<NurseModel>(nurse);
            model.DepartmentId = _context.NurseDepartmentMappings.FirstOrDefault(x => x.NurseId == model.Id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // POST: Nurse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nurse = await _context.Users.FindAsync(id);
            if (nurse != null)
            {
                _context.Users.Remove(nurse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NurseExists(int id)
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
