using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Data;
using PatientPortal.ViewModels;

namespace PatientPortal.Controllers.Clinician
{
    public class ClinicianVitalController : Controller
    {
        private readonly AppDbContext _context;

        public ClinicianVitalController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles="Clinician")]
        public async Task<IActionResult> Index()
        {
            var vm = new ClinicianVitalViewModel();

            vm.Vitals = await _context.ClinicianVital
                .Join(
                    _context.Patients,
                    vital => vital.PatientId,
                    patient => patient.Id,
                    (vital, patient) => new ClinicianVitalItem
                    {
                        Id = vital.Id,
                        PatientName = patient.FullName,
                        Pressure = vital.Pressure,
                        RecordedAt = vital.RecordedAt
                    }
                )
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();

            return View(vm);
        }
    }
}