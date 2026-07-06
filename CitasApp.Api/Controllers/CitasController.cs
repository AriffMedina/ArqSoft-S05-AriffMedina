using CitasApp.Application.Services;
using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly CitaService _citasService;
        private readonly PacienteService _pacienteService;
        private readonly MedicoService _medicoService;

        public CitasController(CitaService citaService,
                               PacienteService pacienteService,
                               MedicoService medicoService)
        {
            _citasService = citaService;
            _pacienteService = pacienteService;
            _medicoService = medicoService;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_citasService.ObtenerTodos());

        [HttpGet("porpaciente/{pacienteId}")]
        public IActionResult PorPaciente(int pacienteId)
        {
            var citas = _citasService.ObtenerPorPaciente(pacienteId);
            return citas.Count == 0 ? NotFound() : Ok(citas);
        }

        [HttpPost("confirmar/{citaId}")]
        public IActionResult ConfirmarCita(int citaId)
        {
            var cita = _citasService.ObtenerPorId(citaId);
            if (cita == null) return NotFound();

            _citasService.ConfirmarCita(citaId);
            return Ok(new { mensaje = "Cita confirmada", cita });
        }
    }
}