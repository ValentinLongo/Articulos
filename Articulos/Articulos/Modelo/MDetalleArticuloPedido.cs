using System;
using System.Collections.Generic;
using System.Text;

namespace Ubicacion_Articulos.Modelo
{
    public class MDetalleArticuloPedido
    {
        public int IdPedido { get; set; } //his_codigo
        public int IdArticulo { get; set; } //his_codnum
        public string DescArticulo { get; set; } //Descripcion his_articulo
        public double Cantidad { get; set; } //Cantidad articulo his_cantidad
        public string Adicional { get; set; } // his_adicional
    }
}
