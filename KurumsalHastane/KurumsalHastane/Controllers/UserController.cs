using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KurumsalHastane.Data;
using KurumsalHastane.Models.User;
using AutoMapper;
using KurumsalHastane.Entities;
using Microsoft.AspNetCore.Hosting;
using KurumsalHastane.Infrastructure.PasswordHasher;

namespace KurumsalHastane.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;
        public UserController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }
        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<UserModel>();
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                models.Add(_mapper.Map<UserModel>(user));
            }

            // Render a partial view with the retrieved data
            return PartialView("_UserListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(UserSearchModel searchModel)
        {
            if(searchModel is null)
            {

            }
            var models = new List<UserModel>();
            var users = await _context.Users.ToListAsync();
            if (searchModel.Name is not null)
            {
                users = users.Where(a => a.FirstName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant()) || a.LastName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant())).ToList();
            }
            if (searchModel.Email is not null)
            {
                users = users.Where(a => a.Email.ToLowerInvariant().Contains(searchModel.Email.ToLowerInvariant()) ).ToList();
            }
            foreach (var user in users)
            {
                
                models.Add(_mapper.Map<UserModel>(user));
            }

            // Render a partial view with the retrieved data
            return PartialView("_UserListPartial", models);
        }
        // GET: User
        public async Task<IActionResult> Index()
        {
            var models = new List<UserModel>();
            var users = await _context.Users.ToListAsync();
            foreach(var user in users)
            {
                models.Add(_mapper.Map<UserModel>(user));
            }
            return View(models);
        }
        public async Task<IActionResult> UserList()
        {
            var models = new List<UserModel>();
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                models.Add(_mapper.Map<UserModel>(user));
            }
            return Json(models.ToArray());
        }
        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<UserModel>(user);

            model.DepartmentId = _context.PersonelDepartmentMappings.FirstOrDefault(x => x.PersonelId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            var model = new UserModel();
            //model.DepartmentId = _context.PersonelDepartmentMappings.FirstOrDefault(x => x.PersonelId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return  View(model);
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model)
        {
            
            if(UserExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid) 
            {
                var filePath = await FileUploadAsync(model.File);
                model.PicturePath = filePath;
                var passwordHash = _passwordHasherService.HashPassword(model.PasswordHash);

                var userEntity= _mapper.Map<User>(model);
                userEntity.PasswordHash = passwordHash;
                _context.Add(userEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //model.DepartmentId = _context.PersonelDepartmentMappings.FirstOrDefault(x => x.PersonelId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<UserModel>(user);
            var departmentMapping = _context.PersonelDepartmentMappings.FirstOrDefault(x => x.PersonelId == id);
            model.DepartmentId = departmentMapping is not null ? departmentMapping.DepartmentId : 0;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(  UserModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(model.Id);
            if (user is not null && ModelState.IsValid)
            {
                _context.Entry(user).State = EntityState.Detached;
                var userEntity = _mapper.Map<User>(model);
                try
                {
                    //var newHashedpass = _passwordHasherService.HashPassword(model.PasswordHash);
                    if (user.PasswordHash.Equals(userEntity.PasswordHash))
                    {
                        userEntity.PasswordHash = user.PasswordHash;
                    }
                    else
                    {
                        userEntity.PasswordHash = _passwordHasherService.HashPassword(model.PasswordHash);
                    }

                    if (model.File is not null)
                    {
                        var filePath = await FileUploadAsync(model.File);
                        userEntity.PicturePath = filePath;
                    }
                    else
                    {
                        userEntity.PicturePath = user.PicturePath;
                    }
                    _context.Update(userEntity);

                    var personelDepartment = _context.PersonelDepartmentMappings.FirstOrDefault(a => a.PersonelId == userEntity.Id);
                    if (personelDepartment is not null)
                    {
                        if (model.DepartmentId != personelDepartment.DepartmentId)
                        {
                            personelDepartment.DepartmentId = model.DepartmentId;
                            _context.Update(personelDepartment);
                        }

                    }
                    else
                    {
                        var _personelDepartment = new PersonelDepartmentMapping
                        {
                            PersonelId = userEntity.Id,
                            DepartmentId = model.DepartmentId
                        };
                        _context.PersonelDepartmentMappings.Add(_personelDepartment);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.Id))
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
            model.DepartmentId = _context.PersonelDepartmentMappings.FirstOrDefault(x => x.PersonelId == model.Id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<UserModel>(user);

            model.DepartmentId = _context.PersonelDepartmentMappings.FirstOrDefault(x => x.PersonelId == id).DepartmentId;
            model.Departments = GetDepartmentList();
            return View(model);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
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
