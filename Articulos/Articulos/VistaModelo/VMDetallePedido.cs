using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;

namespace Ubicacion_Articulos.VistaModelo
{
    public class VMDetallePedido : BaseViewModel
    {
        public ObservableCollection<MDetallePedido> LlenarDetalles(int codigo)
        {
            ObservableCollection<MDetallePedido> detalle = new ObservableCollection<MDetallePedido>();
            //SqlCommand cmd = new SqlCommand("SELECT his_articulo, his_cantidad, his_codnum, his_codtex, ISNULL(his_adicional,'') as adicional FROM MovArticPed WHERE his_codigo=" + codigo.ToString(), ConexionMaestra.con);
            //string variable = "SELECT his_articulo, his_cantidad, his_codnum, ISNULL(his_adicional,'') as adicional FROM MovArticPed WHERE his_codigo=" + codigo.ToString();

            string hostIMG = "http://190.123.89.13:70/";
            string variable = "SELECT his_articulo, his_cantidad, his_codnum, his_codtex, ISNULL(his_adicional,'') as adicional, " +
            "(CASE WHEN Adicional.adi_descri IS NULL THEN '' ELSE Adicional.adi_descri END) AS adi_descri, " +
            "CASE WHEN ISNULL(ada_codnum, 0) = 0 Then REPLACE(art_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') " +
            "Else REPLACE(ada_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') END AS imagen, " +
            "IsNUll((ubi_ubica1 + '-' + ubi_ubica2 +'-'+ ubi_ubica3 +'-'+ ubi_ubica4),'') as ubicacion, his_cpreparada " +
            "FROM MovArticPrep Left Join Articulo on his_codtex = art_codtex And his_codnum = art_codnum " +
            "LEFT JOIN AdicionalxArtic ON(art_codtex = AdicionalxArtic.ada_codtex AND art_codnum = AdicionalxArtic.ada_codnum AND ISNULL(his_adicional,'') = ISNULL(ada_adicional,'') AND AdicionalxArtic.ada_vigencia = 1) " +
            "LEFT JOIN Adicional ON(AdicionalxArtic.ada_adicional = Adicional.adi_codigo) " +
            "Left Join Ubicacion on ubi_codtex = art_codtex And ubi_codnum = art_codnum And IsNUll(ubi_adicional ,'') = IsNUll(ada_adicional, '') And IsNUll(ubi_predef,'0') = 1 " +
            "WHERE his_cantidad>0 and his_codigo = " + codigo.ToString();

            SqlCommand cmd = new SqlCommand(variable, ConexionMaestra.con);

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var cantidad = Convert.ToDouble(dr["his_cpreparada"].ToString());
                bool preparada;
                if (cantidad > 0)
                {
                    preparada = true;
                }
                else
                {
                    preparada = false;
                }
                MDetallePedido est = new MDetallePedido()
                {
                    Articulo = dr["his_articulo"].ToString(),
                    Cantidad = Convert.ToDouble(dr["his_cantidad"].ToString()),
                    IdArticulos = Convert.ToInt32(dr["his_codnum"].ToString()),
                    IdPedido = codigo,
                    Adicional = dr["adicional"].ToString(),
                    CodTex = dr["his_codtex"].ToString(),
                    DescAdicional = dr["adi_descri"].ToString(),
                    Imagen = dr["imagen"].ToString(),
                    Ubicacion = dr["ubicacion"].ToString(),
                    CantidadPrep = preparada,
                    numPrep = Convert.ToDouble(dr["his_cpreparada"].ToString())
                };
                detalle.Add(est);
            }
            return detalle;
        }
    }
}
