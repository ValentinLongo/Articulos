using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace Ubicacion_Articulos.Conexion
{
    public class ConexionMaestra
    {
        //public static string cad = "Data Source = SERVERMASER\\MASER_INF;Initial Catalog =Borrar; User ID = sa; Password=1220;MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=true";
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "conexion.txt");
        public static string text = File.ReadAllText(ruta); //Leo el archivo que esta dentro de la variable ruta
        public static string cad = text;
        public static SqlConnection con = new SqlConnection(cad);

        public ConexionMaestra()
        {
            //con.ConnectionString = cad;
        }
        public static void abrir()
        {
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
        }
        public static void cerrar()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }
}
