using AutoMapper;
using KurumsalHastane.Data;
using KurumsalHastane.Entities;
using KurumsalHastane.Infrastructure.PasswordHasher;
using KurumsalHastane.Models.DepartmentModels;
using KurumsalHastane.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KurumsalHastane.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;
        public DepartmentController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }


        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<DepartmentModel>();
            var departments = await _context.Departments.ToListAsync();
            foreach (var department in departments)
            {
                models.Add(_mapper.Map<DepartmentModel>(department));
            }

            // Render a partial view with the retrieved data
            return PartialView("_DepartmentListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(DepartmentSearchModel searchModel)
        {
            if (searchModel is null)
            {

            }
            var models = new List<DepartmentModel>();
            var departments = await _context.Departments.ToListAsync();
            if (searchModel.Name is not null)
            {
                departments = departments.Where(a => a.Name.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant()) ).ToList();
            }
           
            foreach (var department in departments)
            {

                models.Add(_mapper.Map<DepartmentModel>(department));
            }

            // Render a partial view with the retrieved data
            return PartialView("_DepartmentListPartial", models);
        }
        // GET: Department
        public async Task<IActionResult> Index()
        {
            var models = new List<DepartmentModel>();
            var departments = await _context.Departments.ToListAsync();
            foreach (var department in departments)
            {
                models.Add(_mapper.Map<DepartmentModel>(department));
            }
            return View(models);
        }
        public async Task<IActionResult> DepartmentList()
        {
            var models = new List<DepartmentModel>();
            var departments = await _context.Departments.ToListAsync();
            foreach (var department in departments)
            {
                models.Add(_mapper.Map<DepartmentModel>(department));
            }
            return Json(models.ToArray());
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<DepartmentModel>(department);
            return View(model);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            var model = new DepartmentModel();
            return View(model);
        }

        // POST: Department/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentModel model)
        {
            if (DepartmentExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var filePath = await FileUploadAsync(model.File);
                model.PicturePath = filePath;

                var departmentEntity = _mapper.Map<Department>(model);

                _context.Add(departmentEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<DepartmentModel>(department);
            return View(model);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( DepartmentModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var department = await _context.Departments.FindAsync(model.Id);
            if (department is not null && ModelState.IsValid)
            {
                try
                {
                    var departmentEntity = _mapper.Map<Department>(model);
                    _context.Entry(department).State = EntityState.Detached;
                    _context.Update(departmentEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<DepartmentModel>(department);
            return View(model);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
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
