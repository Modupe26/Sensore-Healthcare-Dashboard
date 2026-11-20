using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Data;
using PatientPortal.ViewModels;

namespace PatientPortal.Controllers.Clinician
{
    public class ClinicianReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ClinicianReportsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles="Clinician")]
        public async Task<IActionResult> Index()
        {
            var vm = new ClinicianReportsViewModel();

            vm.Reports = await _context.Reports
                .Join(
                    _context.Patients,
                    report => report.PatientProfileId,
                    patient => patient.Id,
                    (report, patient) => new ClinicianReportItem
                    {
                        Id = report.Id,
                        PatientName = patient.FullName,
                        Title = "Health Report", // or map from your model if you add a property
                        CreatedAt = report.PeriodEnd // use PeriodEnd as "Created"
                    }
                )
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(vm);
        }
    }
}