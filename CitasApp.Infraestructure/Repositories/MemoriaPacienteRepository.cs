using CitasApp.Interfaces;
using CitasApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitasApp.Infraestructure.Repositories
{
    public class MemoriaPacienteRepository : IPacienteRepository
    {
        List<Paciente> ObtenerTodos()
        {
            return new List<Paciente>();
        }
        Paciente? ObtenerPorId(int id)
        {
            return null;
        }

        List<Paciente> IPacienteRepository.ObtenerTodos()
        {
            return ObtenerTodos();
        }

        Paciente? IPacienteRepository.ObtenerPorId(int id)
        {
            return ObtenerPorId(id);
        }
    }
}
