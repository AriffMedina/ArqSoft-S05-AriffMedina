using CitasApp.Controllers;
using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CitasApp.Tests.Controllers
{
    public class CitaRepositoryFake : ICitaRepository
    {
        private readonly List<Cita> _citas;
        public CitaRepositoryFake(List<Cita> citas) => _citas = citas;
        public List<Cita> ObtenerTodos() => _citas;
        public List<Cita> ObtenerPorPaciente(int pacienteId) =>
            _citas.Where(c => c.PacienteId == pacienteId).ToList();
    }

    public class PacienteRepositoryFake : IPacienteRepository
    {
        private readonly List<Paciente> _pacientes;
        public PacienteRepositoryFake(List<Paciente> pacientes) => _pacientes = pacientes;
        public List<Paciente> ObtenerTodos() => _pacientes;
        public Paciente? ObtenerPorId(int id) => _pacientes.FirstOrDefault(p => p.Id == id);
    }

    public class MedicoRepositoryFake : IMedicoRepository
    {
        private readonly List<Medico> _medicos;
        public MedicoRepositoryFake(List<Medico> medicos) => _medicos = medicos;
        public List<Medico> ObtenerTodos() => _medicos;
        public Medico? ObtenerPorId(int id) => _medicos.FirstOrDefault(m => m.Id == id);
    }

    public class CitaControllerTests
    {
        [Fact]
        public void Index_RegresaTodasLasCitasSinFiltrar()
        {
            var citas = new List<Cita>
            {
                new() { Id = 1, PacienteId = 10, Estado = "Pendiente" },
                new() { Id = 2, PacienteId = 20, Estado = "Confirmada" }
            };
            var controller = new CitaController(
                new CitaRepositoryFake(citas),
                new PacienteRepositoryFake(new List<Paciente>()),
                new MedicoRepositoryFake(new List<Medico>()));

            var resultado = controller.Index() as ViewResult;
            var modelo = resultado?.Model as List<Cita>;

            Assert.NotNull(modelo);
            Assert.Equal(2, modelo!.Count);
        }
    }
}