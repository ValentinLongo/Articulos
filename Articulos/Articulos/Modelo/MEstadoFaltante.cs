using System;
using System.Collections.Generic;
using System.Text;

namespace Ubicacion_Articulos.Modelo
{
    public class MEstadoFaltante
    {
        public int idUser { get; set; }
        public int idDeposito { get; set; }
        public string comentario { get; set; }
        public int terminado { get; set; }
        public string Estado { get; set; }
        public DateTime DateTime { get; set; }
        public string horaFin { get; set; }
        public string fechaFin { get; set; }
        public int idFaltantes { get; set; }
        public string colorEstado { get; set; }
    }
}
