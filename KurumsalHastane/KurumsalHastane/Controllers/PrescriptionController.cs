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
using KurumsalHastane.Models.MedicineModels;
using KurumsalHastane.Models.PrescriptionModels;

namespace KurumsalHastane.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;

        public PrescriptionController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }
        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<PrescriptionModel>();
            var prescriptions = await _context.Prescriptions.Include(p => p.Patient).ToListAsync();
            foreach (var prescription in prescriptions)
            {
                var prModel = _mapper.Map<PrescriptionModel>(prescription);
                prModel.PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName;
                models.Add(prModel);
            }

            // Render a partial view with the retrieved data
            return PartialView("_PrescriptionListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(PrescriptionSearchModel searchModel)
        {
            if (searchModel is null)
            {

            }
            var models = new List<PrescriptionModel>();
            var prescriptions = await _context.Prescriptions.Include(p => p.Patient).ToListAsync();
            if (searchModel.Name is not null)
            {
                prescriptions = prescriptions.Where(a => a.PrescriptionGuid.ToString().Equals(searchModel.Name)).ToList();
            }

            foreach (var prescription in prescriptions)
            {

                var prModel = _mapper.Map<PrescriptionModel>(prescription);
                prModel.PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName;
                models.Add(prModel);
            }

            // Render a partial view with the retrieved data
            return PartialView("_PrescriptionListPartial", models);
        }


       
        public async Task<IActionResult> MedicineList()
        {
            var models = new List<PrescriptionModel>();
            var prescriptions = await _context.Prescriptions.Include(p=> p.Patient).ToListAsync();
            foreach (var prescription in prescriptions)
            {
                var prModel = _mapper.Map<PrescriptionModel>(prescription);
                prModel.PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName;
                models.Add(prModel);
                
            }
            return Json(models.ToArray());
        }
        // GET: Prescription
        public async Task<IActionResult> Index()
        {
            var models = new List<PrescriptionModel>();
            var prescriptions = await _context.Prescriptions.Include(p => p.Patient).Include(a=>a.Medicines).ToListAsync();
            foreach (var prescription in prescriptions)
            {
                var prModel = _mapper.Map<PrescriptionModel>(prescription);
                prModel.PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName;
                models.Add(prModel);
            }
            return View(models);

           
        }

        // GET: Prescription/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prescription == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<PrescriptionModel>(prescription);
            model.Patients = GetPatientList();
            model.Medicines = GetMedicineList();
            return View(model);
        }

        // GET: Prescription/Create
        public IActionResult Create()
        {
            var model = new PrescriptionModel();
            model.Patients = GetPatientList();
            model.Medicines = GetMedicineList();
            model.MedicineList = new MultiSelectList( GetMedicineList());


            return View(model);
        }

        // POST: Prescription/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( PrescriptionModel model)
        {
            if (PrescriptionExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                var prescriptionEntity = _mapper.Map<Prescription>(model);
                prescriptionEntity.PrescriptionGuid = Guid.NewGuid();
                _context.Add(prescriptionEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            model.Patients = GetPatientList();
            model.Medicines = GetMedicineList();
            model.MedicineList = new MultiSelectList(GetMedicineList());
            return View(model);
        }

        // GET: Prescription/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<PrescriptionModel>(prescription);
            model.Patients = GetPatientList();
            model.MedicineList = new MultiSelectList(GetMedicineList());
            model.Medicines = GetMedicineList();
            return View(model);
        }

        // POST: Prescription/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( PrescriptionModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var prescription = await _context.Prescriptions.FindAsync(model.Id);
            
            if (prescription is not null && ModelState.IsValid)
            {
                try
                {
                    var prescriptionEntity = _mapper.Map<Prescription>(model);
                    _context.Entry(prescription).State = EntityState.Detached;
                    _context.Update(prescriptionEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.Id))
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
            model.Patients = GetPatientList(); model.Medicines = GetMedicineList();
            model.MedicineList = new MultiSelectList(GetMedicineList());
            return View(model);
        }

        // GET: Prescription/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prescription == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<PrescriptionModel>(prescription);
            model.Medicines = GetMedicineList();
            model.Patients = GetPatientList();
            return View(model);
        }

        // POST: Prescription/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                _context.Prescriptions.Remove(prescription);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.Id == id);
        }

        private List<SelectListItem> GetPatientList()
        {
            var patientsList = new List<SelectListItem>();
            patientsList.Add(new SelectListItem
            {
                Text = "Select",
                Value = 0.ToString(),
                Selected = true
            });
            var patients = _context.Users.Where(a => a.RoleId == (int)UserRole.Patient);
            foreach (var patient in patients)
            {
                patientsList.Add(new SelectListItem
                {
                    Text = patient.FirstName + " " + patient.LastName,
                    Value = patient.Id.ToString()
                });
            }
            return patientsList;
        }

        private List<SelectListItem> GetMedicineList()
        {
            var medicinesList = new List<SelectListItem>();
            medicinesList.Add(new SelectListItem
            {
                Text = "Select",
                Value = 0.ToString(),
                Selected = true
            });
            var medicines = _context.Medicines.ToList();
            foreach (var medicine in medicines)
            {
                medicinesList.Add(new SelectListItem
                {
                    Text = medicine.FirstName ,
                    Value = medicine.Id.ToString()
                });
            }
            return medicinesList;
        }
    }
}
