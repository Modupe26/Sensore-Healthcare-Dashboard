using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Data;
using PatientPortal.ViewModels;
using PatientPortal.Models;

namespace PatientPortal.Controllers.Clinician
{
    public class ClinicianDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public ClinicianDashboardController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles="Clinician")]
        public async Task<IActionResult> Index()
        {
            var vm = new ClinicianDashboardViewModel();

            // ---------------- SUMMARY COUNTS ----------------
            vm.PatientCount = await _context.Patients.CountAsync();
            vm.ActiveAlertCount = await _context.Alerts.CountAsync();
            vm.UnreadReportCount = await _context.Reports.CountAsync();

            // ---------------- RECENT ALERTS ----------------
            vm.RecentAlerts = await _context.Alerts
                .Join(_context.Patients,
                    a => a.PatientId,
                    p => p.Id,
                    (a, p) => new DashboardAlertItem
                    {
                        Id = a.Id,
                        PatientName = p.FullName,
                        Message = a.Message,
                        Severity = a.Severity.ToString(),
                        CreatedAt = a.CreatedAt
                    })
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            // ---------------- UPCOMING APPOINTMENTS ----------------
            vm.UpcomingAppointments = await _context.ClinicianAppointments
                .Join(_context.Patients,
                    appt => appt.PatientId,
                    p => p.Id,
                    (appt, p) => new DashboardAppointmentItem
                    {
                        Id = appt.Id,
                        PatientName = p.FullName,
                        StartsAt = appt.StartsAt,
                        Location = appt.Location
                    })
                .OrderBy(appt => appt.StartsAt)
                .Take(5)
                .ToListAsync();

            // ---------------- RECENT SENSOR DATA ----------------
            vm.RecentSensorData = await _context.SensorData
                .Join(_context.Patients,
                    s => s.PatientId,
                    p => p.Id,
                    (s, p) => new DashboardSensorItem
                    {
                        Id = s.Id,
                        PatientName = p.FullName,
                        Pressure = s.Pressure,
                        Timestamp = s.Timestamp
                    })
                .OrderByDescending(s => s.Timestamp)
                .Take(5)
                .ToListAsync();

            // ---------------- RECENT COMMENTS ----------------
            vm.RecentComments = await _context.Comments
                .Join(_context.Patients,
                    c => c.PatientId,
                    p => p.Id,
                    (c, p) => new DashboardCommentItem
                    {
                        Id = c.Id,
                        PatientName = p.FullName,
                        BodyText = c.BodyText,
                        CreatedAtUtc = c.CreatedAtUtc
                    })
                .OrderByDescending(c => c.CreatedAtUtc)
                .Take(5)
                .ToListAsync();

            return View(vm);
        }
    }
}