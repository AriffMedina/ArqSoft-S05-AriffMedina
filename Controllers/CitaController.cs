using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers
{
    public class CitaController : Controller
    {
        private static List<Paciente> _pacientes = new()
        {
            new Paciente { Id = 1, Nombre = "Ana",   Apellido = "García"   },
            new Paciente { Id = 2, Nombre = "Luis",  Apellido = "Martínez" },
            new Paciente { Id = 3, Nombre = "María", Apellido = "López"    },
        };

        private static List<Medico> _medicos = new()
        {
            new Medico { Id = 1, Nombre = "Carlos",   Apellido = "Reyes"   },
            new Medico { Id = 2, Nombre = "Patricia",  Apellido = "Vega"   },
            new Medico { Id = 3, Nombre = "Roberto",  Apellido = "Sánchez" },
        };
        private static List<Cita> _citas = new()
                    {
            new Cita { Id = 1, PacienteId = 1, MedicoId = 1, Fecha = new DateOnly(2026, 6, 1), Hora = new TimeOnly(9,  0), Motivo = "Consulta general",      Estado = "Confirmada" },
            new Cita { Id = 2, PacienteId = 2, MedicoId = 2, Fecha = new DateOnly(2026, 6, 1), Hora = new TimeOnly(10, 0), Motivo = "Revisión de resultados", Estado = "Pendiente"  },
            new Cita { Id = 3, PacienteId = 3, MedicoId = 1, Fecha = new DateOnly(2026, 6, 3), Hora = new TimeOnly(11, 0), Motivo = "Primera consulta",       Estado = "Pendiente"  },
        };
        public IActionResult Index()
        {
            ViewBag.Pacientes = _pacientes;
            ViewBag.Medicos = _medicos;
            return View(_citas);
        }

        public IActionResult PorPaciente(int pacienteId)
        {
            var citas = _citas.Where(c => c.PacienteId == pacienteId).ToList();
            ViewBag.Pacientes = _pacientes;
            ViewBag.Medicos = _medicos;
            return View(citas);
        }
    }
}