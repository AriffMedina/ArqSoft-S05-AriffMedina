using CitasApp.Domain.Interfaces;
using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Application.Services
{
    public class CitaService
    {
        private readonly ICitaRepository _repo;
        private readonly IEnumerable<ICitaObserver> _observers;

        public CitaService(ICitaRepository repo, IEnumerable<ICitaObserver> observers)
        {
            _repo = repo;
            _observers = observers;
        }

        public IEnumerable<Cita> ObtenerTodos() => _repo.ObtenerTodos();

        public Cita? ObtenerPorId(int id)
        {
            return _repo.ObtenerTodos().FirstOrDefault(c => c.Id == id);
        }

        public Cita? ConfirmarCita(int citaId)
        {
            var cita = ObtenerPorId(citaId);
            if (cita == null) return null;
            cita.Estado = "Confirmada";
            foreach (var observer in _observers)
                observer.OnCitaConfirmada(cita);
            return cita;
        }

        public List<Cita> ObtenerPorPaciente(int pacienteId) =>
            _repo.ObtenerPorPaciente(pacienteId).ToList();
    }
}