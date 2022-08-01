using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;

namespace Ubicacion_Articulos.VistaModelo
{
    public class VMPedidos : BaseViewModel
    {
        public ObservableCollection<MPedidos> LlenarPedidos()
        {
            ObservableCollection<MPedidos> Pedidos = new ObservableCollection<MPedidos>();
            SqlCommand cmd = new SqlCommand("SELECT * FROM MovVtaPed LEFT JOIN CtrlModif ON vta_codigo = ctr_codigo WHERE vta_estado = 'EN PREPARACION' AND ctr_codigo IS NULL ORDER BY vta_fecemi, VTA_CPBTE", ConexionMaestra.con);
            //SqlCommand cmd = new SqlCommand("SELECT vta_codigo,vta_fecpro,vta_ctacli,vta_cliente,vta_estado, vta_cpbte FROM MovVtaPed WHERE vta_estado='EN PREPARACION' ORDER BY vta_codigo", ConexionMaestra.con);
            SqlDataReader dr = cmd.ExecuteReader();
            int index = 0;
            while (dr.Read())
            {
                MPedidos est = new MPedidos()
                {
                    IdPedido = Convert.ToInt32(dr["vta_codigo"].ToString()),
                    FechaPedido = Convert.ToDateTime(dr["vta_fecpro"].ToString()),
                    IdCliente = Convert.ToInt32(dr["vta_ctacli"].ToString()),
                    Cliente = dr["vta_cliente"].ToString(),
                    EstadoPedido = dr["vta_estado"].ToString(),
                    NumeroComprobante = dr["vta_cpbte"].ToString(),
                    TipMov = Convert.ToInt32(dr["vta_tipmov"].ToString()),
                    NIndex = index
                };
                index++;
                Pedidos.Add(est);
            }
            return Pedidos;
        }
        //SELECT * FROM MovVtaPed LEFT JOIN CtrlModif ON vta_codigo = ctr_codigo WHERE vta_estado = 'EN PREPARACION' AND ctr_codigo IS NOT NULL ORDER BY vta_fecemi, VTA_CPBTE
        public ObservableCollection<MPedidos> LlenarPedidosEnCurso()
        {
            ObservableCollection<MPedidos> Pedidos = new ObservableCollection<MPedidos>();
            //SqlCommand cmd = new SqlCommand("SELECT * FROM MovVtaPed LEFT JOIN CtrlModif ON vta_codigo = ctr_codigo WHERE vta_estado = 'EN PREPARACION' AND ctr_codigo IS NOT NULL ORDER BY vta_fecemi, VTA_CPBTE", ConexionMaestra.con);
            SqlCommand cmd = new SqlCommand("SELECT * FROM MovVtaPed LEFT JOIN CtrlModif ON vta_codigo = ctr_codigo WHERE vta_estado = 'EN PREPARACION' AND vta_fin = 0 AND ctr_codigo IS NOT NULL ORDER BY vta_fecemi, VTA_CPBTE", ConexionMaestra.con);
            SqlDataReader dr = cmd.ExecuteReader();
            int index = 0;
            while (dr.Read())
            {
                MPedidos est = new MPedidos()
                {
                    IdPedido = Convert.ToInt32(dr["vta_codigo"].ToString()),
                    FechaPedido = Convert.ToDateTime(dr["vta_fecpro"].ToString()),
                    IdCliente = Convert.ToInt32(dr["vta_ctacli"].ToString()),
                    Cliente = dr["vta_cliente"].ToString(),
                    EstadoPedido = dr["vta_estado"].ToString(),
                    NumeroComprobante = dr["vta_cpbte"].ToString(),
                    TipMov = Convert.ToInt32(dr["vta_tipmov"].ToString()),
                    NIndex = index
                };
                index++;
                Pedidos.Add(est);
            }
            return Pedidos;
        }
    }
}
