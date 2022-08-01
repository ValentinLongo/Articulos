using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Ubicacion_Articulos.Conexion;

namespace Ubicacion_Articulos.VistaModelo
{
    public class VMusuario
    {
        public void ComprobarConexion(ref int Id)
        {
            try
            {
                ConexionMaestra.abrir();
                SqlCommand cc = new SqlCommand("Select Top 1 usu_codigo from Usuario", ConexionMaestra.con);
                Id = Convert.ToInt32(cc.ExecuteScalar());
            }
            catch (Exception)
            {
                Id = 0;
            }
        }
    }
}
