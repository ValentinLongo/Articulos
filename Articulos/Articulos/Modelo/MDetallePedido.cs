using System;
using System.Collections.Generic;
using System.Text;

namespace Ubicacion_Articulos.Modelo
{
    public class MDetallePedido
    {
        public int IdPedido { get; set; } //his_codigo
        public int IdArticulos { get; set; } // his_codnum
        public string Articulo { get; set; } //his_articulo
        public double Cantidad { get; set; } // his_cantidad
        public string Adicional { get; set; } // his_adicional
        public string CodTex { get; set; } // his_codtex
        public string DescAdicional { get; set; } // adi_descri
        public string Imagen { get; set; }
        public string Ubicacion { get; set; }
        public bool CantidadPrep { get; set; }
        public double numPrep { get; set; } // his_cpreparada
    }
}
