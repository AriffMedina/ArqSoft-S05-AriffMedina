using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers
{
    public class MedicoController : Controller
    {
        private static List<Medico> _medicos = new()
        {
            new Medico { Id = 1, Nombre = "Carlos",  Apellido = "Reyes",   Especialidad = "Medicina General", NumeroLicencia = "MG-10421" },
            new Medico { Id = 2, Nombre = "Patricia", Apellido = "Vega",   Especialidad = "Pediatría",        NumeroLicencia = "PD-20835" },
            new Medico { Id = 3, Nombre = "Roberto",  Apellido = "Sánchez", Especialidad = "Cardiología",     NumeroLicencia = "CA-30117" },
        };

        public IActionResult Index() => View(_medicos);

        public IActionResult Detalle(int id)
        {
            var medico = _medicos.FirstOrDefault(m => m.Id == id);

            return medico == null ? NotFound() : View(medico);
        }
    }
}