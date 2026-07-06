// CitasApp.Infrastructure/Repositories/CsvCitaRepository.cs
// Adapter de salida — implementa ICitaRepository leyendo un archivo CSV
//
// Fecha se guarda como  yyyy-MM-dd  (ej: 2026-06-15)
// Hora  se guarda como  HH:mm       (ej: 09:30)

using CitasApp.Interfaces;
using CitasApp.Domain;
using CitasApp.Models;

namespace CitasApp.Infrastructure.Repositories
{
    public class CsvCitaRepository : ICitaRepository
    {
        private readonly string _filePath;

        public CsvCitaRepository(string filePath)
        {
            _filePath = filePath;

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath,
                    "Id,PacienteId,MedicoId,Fecha,Hora,Motivo,Estado\n");
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        private List<Cita> LeerTodos()
        {
            var lista = new List<Cita>();

            foreach (var linea in File.ReadAllLines(_filePath).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                var p = linea.Split(',');
                if (p.Length < 7) continue;

                lista.Add(new Cita
                {
                    Id         = int.Parse(p[0]),
                    PacienteId = int.Parse(p[1]),
                    MedicoId   = int.Parse(p[2]),
                    Fecha      = DateOnly.ParseExact(p[3], "yyyy-MM-dd"),
                    Hora       = TimeOnly.ParseExact(p[4], "HH:mm"),
                    Motivo     = p[5],
                    Estado     = p[6]
                });
            }

            return lista;
        }

        private void EscribirTodos(List<Cita> citas)
        {
            var lineas = new List<string>
                { "Id,PacienteId,MedicoId,Fecha,Hora,Motivo,Estado" };

            foreach (var c in citas)
            {
                lineas.Add(
                    $"{c.Id}," +
                    $"{c.PacienteId}," +
                    $"{c.MedicoId}," +
                    $"{c.Fecha:yyyy-MM-dd}," +
                    $"{c.Hora:HH:mm}," +
                    $"{Limpiar(c.Motivo)}," +
                    $"{Limpiar(c.Estado)}"
                );
            }

            File.WriteAllLines(_filePath, lineas);
        }

        private static string Limpiar(string texto) =>
            (texto ?? string.Empty).Replace(",", ";");

        // ── Port ────────────────────────────────────────────────────────────────

        public List<Cita> ObtenerTodos() => LeerTodos();

        public Cita? ObtenerPorId(int id) =>
            LeerTodos().FirstOrDefault(c => c.Id == id);

        public List<Cita> ObtenerPorPaciente(int pacienteId) =>
            LeerTodos().Where(c => c.PacienteId == pacienteId).ToList();

        public void Agregar(Cita cita)
        {
            var citas = LeerTodos();
            cita.Id = citas.Count > 0 ? citas.Max(c => c.Id) + 1 : 1;
            citas.Add(cita);
            EscribirTodos(citas);
        }

        public void ConfirmarCita(int id)
        {
            var citas = LeerTodos();
            var cita  = citas.FirstOrDefault(c => c.Id == id);

            if (cita is not null)
            {
                cita.Estado = "Confirmada";
                EscribirTodos(citas);
            }
        }
    }
}
