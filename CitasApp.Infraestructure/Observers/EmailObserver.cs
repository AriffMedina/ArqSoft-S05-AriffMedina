using CitasApp.Domain.Interfaces;
using CitasApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitasApp.Infraestructure.Observers
{
    public class EmailObserver : ICitaObserver
    {
        public void OnCitaConfirmada(Cita cita)
        {
            Console.WriteLine($"[Email] Confirmación enviada al paciente {cita.PacienteId} - motivo: {cita.Motivo} - estado: {cita.Estado}");
        }
    }
}
