using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.ViewModels;

namespace PatientPortal.Controllers.Clinician
{
    public class ClinicianNotesController : Controller
    {
        [Authorize(Roles="Clinician")]
        public IActionResult Index()
        {
            // Empty for now; no DB table shown in your screenshots
            var vm = new ClinicianNotesViewModel();
            return View(vm);
        }
    }
}