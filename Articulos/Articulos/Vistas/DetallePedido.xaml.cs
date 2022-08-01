using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetallePedido : ContentPage
    {
        VMDetallePedido obj = new VMDetallePedido();
        public DetallePedido()
        {
            InitializeComponent();
            InicializarVista();
        }

        private void InicializarVista()
        {
            try
            {
                //Traigo datos del pedido
                var codigo = Preferences.Get("pedido", "");
                var deserialize = JsonConvert.DeserializeObject<MPedidos>(codigo);
                Codigo.Text = Convert.ToString(deserialize?.IdPedido);
                int entero = Convert.ToInt32(Codigo.Text);

                //Verifico si ya existe en MovArticPrep
                SqlCommand verificacion = new SqlCommand($"Select * from MovArticPrep where his_codigo = {entero}", ConexionMaestra.con);
                SqlDataReader dr1 = verificacion.ExecuteReader();

                if (dr1.Read())
                {

                }
                else
                {
                    SqlCommand movarticprep = new SqlCommand("INSERT INTO MovArticPrep (his_codigo, his_codtex, his_codnum, his_tipmov, his_cpbte, his_fecemi, his_ctacli, his_articulo, his_aliva, his_punitario, his_cantidad, his_pend, his_dtoa, his_dtob, his_dtoc, his_dtod, " +
                    "his_dtoe, his_dtof, his_total, his_preccosto, his_cn, his_referencia, his_dtoCondA, his_dtoCondB, his_dtoCondC, his_dtoCondD, his_dtoctdo, his_plista, his_observa, his_orden, his_ctrlprec, " +
                    "his_varpor, his_plistareal, his_usuario, his_adicional, his_concepto, his_tiprecalc, his_oferta, his_ctacont, his_categoria, his_condiva, his_conceptocont, his_medida, his_refmedida, " +
                    "his_refprepara, his_codtexRe, his_codnumRe, his_adicionalRe, his_cpreparada) " +
                    "SELECT his_codigo, his_codtex, his_codnum, his_tipmov, his_cpbte, his_fecemi, his_ctacli, his_articulo, his_aliva, his_punitario, his_cantidad, his_pend, his_dtoa, his_dtob, his_dtoc, his_dtod, " +
                    "his_dtoe, his_dtof, his_total, his_preccosto, his_cn, his_referencia, his_dtoCondA, his_dtoCondB, his_dtoCondC, his_dtoCondD, his_dtoctdo, his_plista, his_observa, his_orden, his_ctrlprec, " +
                    "his_varpor, his_plistareal, his_usuario, his_adicional, his_concepto, his_tiprecalc, his_oferta, his_ctacont, his_categoria, his_condiva, his_conceptocont, his_medida, his_refmedida, " +
                   $"his_prepara, 'NULL',0,'NULL',0 FROM MovArticPed Where his_codigo = {entero}", ConexionMaestra.con);
                    SqlDataReader dr = movarticprep.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
            //Completo tabla MovArticPrep con los detalles del pedido
        }

        protected override void OnAppearing()
        {
            try
            {
                var codigo = Preferences.Get("pedido", "");
                var deserialize = JsonConvert.DeserializeObject<MPedidos>(codigo);
                Codigo.Text = Convert.ToString(deserialize?.IdPedido);
                int entero = Convert.ToInt32(Codigo.Text);
                ListaProducto.ItemsSource = obj.LlenarDetalles(entero);
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
        private void ListaProducto_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                //envio datos del item
                var item = e.Item as MDetallePedido;
                var serializer = JsonConvert.SerializeObject(item);
                if (item != null)
                {
                    Preferences.Set("articulo", serializer);
                }

                Preferences.Set("datoUbic", "");
                Preferences.Set("bandera", "");
                Navigation.PushAsync(new DetalleArticuloPedido());
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private async void btnFinalizar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var acc = await DisplayActionSheet("¿Desea finalizar el pedido?", "Cancelar", null, "SI", "NO");
                //Traigo datos del pedido
                var codigo = Preferences.Get("pedido", "");
                var deserialize = JsonConvert.DeserializeObject<MPedidos>(codigo);
                Codigo.Text = Convert.ToString(deserialize?.IdPedido);
                int entero = Convert.ToInt32(Codigo.Text);

                //Datos del usuario
                var login = Preferences.Get("data", "");
                var deserializee = JsonConvert.DeserializeObject<MLogin>(login);
                numUsu.Text = (deserializee?.usu_codigo).ToString();

                if (acc == "SI")
                {
                    SqlCommand fin = new SqlCommand($"UPDATE MovVtaPed SET vta_fin = 1 WHERE vta_codigo = {entero}", ConexionMaestra.con);
                    fin.ExecuteNonQuery();
                    SqlCommand fin1 = new SqlCommand($"UPDATE PrepPed SET ped_estado = 'FIN', ped_fecfin = '{DateTime.Now.ToString("d")}', ped_horafin = '{DateTime.Now.ToString("T")}', ped_usuario1 = '{numUsu.Text}' WHERE ped_codigo = {entero}", ConexionMaestra.con);
                    fin.ExecuteNonQuery();

                    await DisplayAlert("Mensaje", "El pedido ha sido FINALIZADO", "Aceptar");
                }
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
    }
}