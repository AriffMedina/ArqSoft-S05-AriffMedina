using CitasApp.Domain.Interfaces;
using CitasApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitasApp.Infraestructure.Observers
{
    public class SmsObserver : ICitaObserver
    {
        public void OnCitaConfirmada(Cita cita)
        {
            Console.WriteLine($"[SMS] Confirmación enviada al paciente {cita.PacienteId} - motivo: {cita.Motivo} - estado: {cita.Estado}");
        }
    }
}
