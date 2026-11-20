using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Data;
using PatientPortal.ViewModels;

namespace PatientPortal.Controllers.Clinician
{
    public class ClinicianPatientController : Controller
    {
        private readonly AppDbContext _context;

        public ClinicianPatientController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles="Clinician")]
        public async Task<IActionResult> Index()
        {
            var vm = new ClinicianPatientsViewModel
            {
                Patients = await _context.Patients
                    .OrderBy(p => p.FullName)
                    .Select(p => new ClinicianPatientItem
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        LoginId = p.LoginId
                    })
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}