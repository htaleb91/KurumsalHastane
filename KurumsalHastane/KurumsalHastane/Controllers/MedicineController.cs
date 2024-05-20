using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KurumsalHastane.Data;
using KurumsalHastane.Entities;
using AutoMapper;
using KurumsalHastane.Infrastructure.PasswordHasher;
using KurumsalHastane.Models.MedicineModels;

namespace KurumsalHastane.Controllers
{
    public class MedicineController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;

        public MedicineController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }


        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<MedicineModel>();
            var departments = await _context.Medicines.ToListAsync();
            foreach (var Medicine in departments)
            {
                models.Add(_mapper.Map<MedicineModel>(Medicine));
            }

            // Render a partial view with the retrieved data
            return PartialView("_MedicineListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(MedicineSearchModel searchModel)
        {
            if (searchModel is null)
            {

            }
            var models = new List<MedicineModel>();
            var departments = await _context.Medicines.ToListAsync();
            if (searchModel.Name is not null)
            {
                departments = departments.Where(a => a.FirstName.ToLowerInvariant().Contains(searchModel.Name.ToLowerInvariant())).ToList();
            }

            foreach (var Medicine in departments)
            {

                models.Add(_mapper.Map<MedicineModel>(Medicine));
            }

            // Render a partial view with the retrieved data
            return PartialView("_MedicineListPartial", models);
        }


        // GET: Medicines
        public async Task<IActionResult> Index()
        {
            var models = new List<MedicineModel>();
            var medicines = await _context.Medicines.ToListAsync();
            foreach (var medicine in medicines)
            {
                models.Add(_mapper.Map<MedicineModel>(medicine));
            }
            return View(models);
        }
        public async Task<IActionResult> MedicineList()
        {
            var models = new List<MedicineModel>();
            var medicines = await _context.Medicines.ToListAsync();
            foreach (var medicine in medicines)
            {
                models.Add(_mapper.Map<MedicineModel>(medicine));
            }
            return Json(models.ToArray());
        }


        // GET: Medicines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<MedicineModel>(medicine);
            return View(model);
        }

        // GET: Medicines/Create
        public IActionResult Create()
        {
            var model = new MedicineModel();
            return View(model);
        }

        // POST: Medicines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( MedicineModel model)
        {
            if (MedicineExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var medicineEntity = _mapper.Map<Medicine>(model);
                _context.Add(medicineEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Medicines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<MedicineModel>(medicine);
            return View(model);
        }

        // POST: Medicines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( MedicineModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var medicine = await _context.Medicines.FindAsync(model.Id);
            if (medicine is not null && ModelState.IsValid)
            {
                try
                {
                    var medicineEntity = _mapper.Map<Medicine>(model);
                    _context.Entry(medicine).State = EntityState.Detached;
                    _context.Update(medicineEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicineExists(medicine.Id))
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

        // GET: Medicines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<MedicineModel>(medicine);
            return View(model);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicineExists(int id)
        {
            return _context.Medicines.Any(e => e.Id == id);
        }
    }
}
