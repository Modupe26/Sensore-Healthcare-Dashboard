using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Data;
using PatientPortal.Models;
using PatientPortal.Models.Clinician;
using PatientPortal.ViewModels;

namespace PatientPortal.Controllers.Clinician
{
    public class ClinicianAppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public ClinicianAppointmentController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles="Clinician")]
        public async Task<IActionResult> Index()
        {
            var vm = new ClinicianAppointmentViewModel
            {
                Appointments = await _context.ClinicianAppointments
                    .Join(
                        _context.Patients,
                        appt => appt.PatientId,
                        patient => patient.Id,
                        (appt, patient) => new ClinicianAppointmentItem
                        {
                            Id = appt.Id,
                            PatientName = patient.FullName,
                            StartsAt = appt.StartsAt,
                            Location = appt.Location,
                            Reason = appt.Reason
                        }
                    )
                    .OrderBy(a => a.StartsAt)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}