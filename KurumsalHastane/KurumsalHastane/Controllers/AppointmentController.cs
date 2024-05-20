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
using KurumsalHastane.Models.AppointmentModels;

namespace KurumsalHastane.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPasswordHasherService _passwordHasherService;

        public AppointmentController(ApplicationDBContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _passwordHasherService = passwordHasherService;
        }

        [HttpPost]
        public async Task<ActionResult> List()
        {
            var models = new List<AppointmentModel>();
            var appointemnts = await _context.Appointments
                .Include(a=> a.Specialist)
                .Include(a=> a.Patient)
                .Include(a=> a.Department)
                 .ToListAsync();
            foreach (var appointment in appointemnts)
            {
                var appointmentModel = _mapper.Map<AppointmentModel>(appointment);
                appointmentModel.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.FirstName;
                appointmentModel.SpecialistName = appointment.Specialist.FirstName + " " + appointment.Specialist.FirstName;
                appointmentModel.DepartmentName = appointment.Department.Name;
                models.Add(appointmentModel);
            }

            // Render a partial view with the retrieved data
            return PartialView("_AppointmentListPartial", models);
        }
        [HttpPost]
        public async Task<ActionResult> SearchList(AppointmentSearchModel searchModel)
        {
            if (searchModel is null)
            {

            }
            var models = new List<AppointmentModel>();
            var appointments = await _context.Appointments
                .Include(a => a.Specialist)
                .Include(a => a.Patient)
                .Include(a => a.Department).ToListAsync();
            if (searchModel.PatientName is not null)
            {
                appointments = appointments.Where(a => 
                (a.Patient.FirstName.ToLowerInvariant() + " "+ a.Patient.FirstName.ToLowerInvariant())
                .Contains(searchModel.PatientName.ToLowerInvariant())).ToList();
            }
            if (searchModel.SpecialistName is not null)
            {
                appointments = appointments.Where(a =>
                                (a.Specialist.FirstName.ToLowerInvariant() + " " + a.Specialist.FirstName.ToLowerInvariant())
                                .Contains(searchModel.SpecialistName.ToLowerInvariant())).ToList();
            }
            if (searchModel.DepartmentName is not null)
            {
                appointments = appointments.Where(a =>
                                (a.Department.Name.ToLowerInvariant())
                                .Contains(searchModel.DepartmentName.ToLowerInvariant())).ToList();
            }
            foreach (var appointment in appointments)
            {

                var appointmentModel = _mapper.Map<AppointmentModel>(appointment);
                appointmentModel.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.FirstName;
                appointmentModel.SpecialistName = appointment.Specialist.FirstName + " " + appointment.Specialist.FirstName;
                appointmentModel.DepartmentName = appointment.Department.Name;
                models.Add(appointmentModel);
            }

            // Render a partial view with the retrieved data
            return PartialView("_AppointmentListPartial", models);
        }
        // GET: appointemnt
        public async Task<IActionResult> Index()
        {
            var models = new List<AppointmentModel>();
            var appointments = await _context.Appointments
                .Include(a => a.Specialist)
                .Include(a => a.Patient)
                .Include(a => a.Department).ToListAsync();
            foreach (var appointment in appointments)
            {
                models.Add(_mapper.Map<AppointmentModel>(appointment));
            }
            return View(models);
        }
        public async Task<IActionResult> UserList()
        {
            var models = new List<AppointmentModel>();
            var appointments = await _context.Appointments
                 .Include(a => a.Specialist)
                 .Include(a => a.Patient)
                 .Include(a => a.Department).ToListAsync();
            foreach (var appointment in appointments)
            {
                var appointmentModel = _mapper.Map<AppointmentModel>(appointment);
                appointmentModel.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.FirstName;
                appointmentModel.SpecialistName = appointment.Specialist.FirstName + " " + appointment.Specialist.FirstName;
                appointmentModel.DepartmentName = appointment.Department.Name;
                models.Add(appointmentModel);
            }
            return Json(models.ToArray());
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Department)
                .Include(a => a.Patient)
                .Include(a => a.Specialist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<AppointmentModel>(appointment);
            model.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.FirstName;
            model.SpecialistName = appointment.Specialist.FirstName + " " + appointment.Specialist.FirstName;
            model.DepartmentName = appointment.Department.Name;
            return View(model);
        }

        // GET: Appointment/Create
        public IActionResult Create()
        {
            var model = new AppointmentModel();
            model.Departments = GetDepartmentList();
            model.Patients = GetPatientList();
            model.Specialists = GetSpecialistList();
            model.AppointnentDate = DateTime.UtcNow;
            return View(model);
            
        }

        // POST: Appointment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( AppointmentModel model)
        {
            if (AppointmentExists(model.Id))
            {
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                var appointment = _mapper.Map<Appointment>(model);
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.Departments = GetDepartmentList();
            model.Patients = GetPatientList();
            model.Specialists = GetSpecialistList();
            model.AppointnentDate = DateTime.UtcNow;

            return View(model);
        }

        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<AppointmentModel>(appointment);
            model.Departments = GetDepartmentList();
            model.Patients = GetPatientList();
            model.Specialists = GetSpecialistList();
            model.AppointnentDate = DateTime.UtcNow;

            return View(model);
        }

        // POST: Appointment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentModel model)
        {
            if (model is null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointments.FindAsync(model.Id);
            if (appointment is not null && ModelState.IsValid)
            {
                try
                {
                    var appointmentEntity = _mapper.Map<Appointment>(model);

                    _context.Entry(appointment).State = EntityState.Detached;

                    _context.Update(appointmentEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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

            model.Departments = GetDepartmentList();
            model.Patients = GetPatientList();
            model.Specialists = GetSpecialistList();
            model.AppointnentDate = DateTime.UtcNow;

            return View(model);

        }
        // GET: Appointment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Department)
                .Include(a => a.Patient)
                .Include(a => a.Specialist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
                var model = _mapper.Map<AppointmentModel>(appointment);
            model.PatientName = appointment.Patient.FirstName + " " + appointment.Patient.FirstName;
            model.SpecialistName = appointment.Specialist.FirstName + " " + appointment.Specialist.FirstName;
            model.DepartmentName = appointment.Department.Name;
            model.AppointnentDate = DateTime.UtcNow;

            return View(model);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
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

        private List<SelectListItem> GetSpecialistList()
        {
            var specialistList = new List<SelectListItem>();
            specialistList.Add(new SelectListItem
            {
                Text = "Select",
                Value = 0.ToString(),
                Selected = true
            });
            var specialists = _context.Users.Where(a => a.RoleId == (int)UserRole.Specialist);
            foreach (var specialist in specialists)
            {
                specialistList.Add(new SelectListItem
                {
                    Text = specialist.FirstName + " " + specialist.LastName,
                    Value = specialist.Id.ToString()
                });
            }
            return specialistList;
        }
    }
}
