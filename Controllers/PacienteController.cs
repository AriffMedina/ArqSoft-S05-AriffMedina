using CitasApp.Interfaces;

namespace CitasApp.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IPacienteRepository _repo;
        public PacienteController(IPacienteRepository repo) { _repo = repo; }

        public IActionResult Index() => View(_repo.ObtenerTodos());

        public IActionResult Detalle(int id)
        {
            var paciente = _repo.ObtenerPorId(id);
            return paciente == null ? NotFound() : View(paciente);
        }
    }
}