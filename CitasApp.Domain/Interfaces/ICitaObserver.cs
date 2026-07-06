using CitasApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitasApp.Domain.Interfaces
{
    public interface ICitaObserver
    {
        void OnCitaConfirmada(Cita cita);
    }

}
