using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;
using Xamarin.Essentials;

namespace Ubicacion_Articulos.VistaModelo
{
    public class VMDetalleArticuloPedido : BaseViewModel
    {
        public ObservableCollection<MEditarUbicacion> traerUbicacion(string campo)
        {
            ObservableCollection<MEditarUbicacion> ubi = new ObservableCollection<MEditarUbicacion>();
            string query = "select (ubi_ubica1 + '-' + ubi_ubica2 +'-'+ ubi_ubica3 +'-'+ ubi_ubica4) as ubicacion, * from Ubicacion left join Deposito on ubi_deposito = dep_codigo where " + campo + "";
            SqlCommand sqlUbica = new SqlCommand(query, ConexionMaestra.con);
            SqlDataReader dr = sqlUbica.ExecuteReader();
            while (dr.Read())
            {
                MEditarUbicacion x = new MEditarUbicacion()
                {
                    ubicacion = dr["ubicacion"].ToString(),
                    deposito = dr["dep_descri"].ToString(),
                    ubi_codtex = dr["ubi_codtex"].ToString(),
                    ubi_adicional = dr["ubi_adicional"].ToString(),
                    ubi_codnum = Convert.ToInt32(dr["ubi_codnum"].ToString()),
                    ubi_codigo = Convert.ToInt32(dr["ubi_codigo"].ToString()),
                    ubi_deposito = Convert.ToInt32(dr["ubi_deposito"].ToString()),
                    ubi_predef = Convert.ToInt32(dr["ubi_predef"].ToString()),
                    ubi_ubica1 = dr["ubi_ubica1"].ToString(),
                    ubi_ubica2 = dr["ubi_ubica2"].ToString(),
                    ubi_ubica3 = dr["ubi_ubica3"].ToString(),
                    ubi_ubica4 = dr["ubi_ubica4"].ToString()

                };
                ubi.Add(x);
                var serializer = JsonConvert.SerializeObject(ubi[0]);
                if (ubi != null && ubi.Count > 0)
                {
                    Preferences.Set("dato", serializer);
                }
            }
            return ubi;
        }
    }
}
