using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Data;
using PatientPortal.ViewModels;
using PatientPortal.Models;                // Gives access to Alert, Patient, etc.
using PatientPortal.Models.Clinician;      // In case any Clinician models are needed

namespace PatientPortal.Controllers.Clinician
{
    public class ClinicianAlertsController : Controller
    {
        private readonly AppDbContext _context;

        public ClinicianAlertsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles="Clinician")]
        public async Task<IActionResult> Index()
        {
            var vm = new ClinicianAlertsViewModel
            {
                Alerts = await _context.Alerts
                    .Join(
                        _context.Patients,
                        alert => alert.PatientId,
                        patient => patient.Id,
                        (alert, patient) => new ClinicianAlertItem
                        {
                            Id = alert.Id,
                            PatientName = patient.FullName,
                            Message = alert.Message,
                            Severity = alert.Severity.ToString(),
                            CreatedAt = alert.CreatedAt,
                            Acknowledged = alert.Acknowledged
                        }
                    )
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}