using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
    public partial class Pedidos : ContentPage
    {
        VMPedidos obj = new VMPedidos();
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
        public static int terminal = Convert.ToInt32(File.ReadAllText(ruta));
        string estadoPicker;
        public Pedidos()
        {
            InitializeComponent();
            InicializarVista();
            PickerEstado.Items.Add("PEDIDOS");
            PickerEstado.Items.Add("EN CURSO");
        }

        private void InicializarVista()
        {
            
        }

        protected override void OnAppearing()
        {
            InicializarVista();
            ListaPedidos.ItemsSource = "";
            PickerEstado.Title = "Seleccione Estado";
            estadoPicker = "";
        }
        private void PickerEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                estadoPicker = Convert.ToString(PickerEstado.SelectedItem);
                if (estadoPicker == "PEDIDOS")
                {
                    ListaPedidos.ItemsSource = obj.LlenarPedidos();
                }
                else if (estadoPicker == "EN CURSO")
                {
                    ListaPedidos.ItemsSource = obj.LlenarPedidosEnCurso();
                }
            }
            catch (Exception ex)
            {
                ConexionMaestra.abrir();
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                
            }
        }

        private async void ListaPedidos_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var item = e.Item as MPedidos;
                if (estadoPicker == "PEDIDOS")
                {
                    if (item.NIndex == 0)
                    {
                        var accion = await DisplayActionSheet("¿Desea comenzar un nuevo pedido?", null, null, "SI", "NO");
                        if (accion == "SI")
                        {
                            //Envio informacion sobre el pedido
                            //var item = e.Item as MPedidos;
                            var serializer = JsonConvert.SerializeObject(item);
                            if (item != null)
                            {
                                Preferences.Set("pedido", serializer);
                            }
                            //Informacion sobre usuario y terminal.
                            var login = Preferences.Get("data", "");
                            var deserialize = JsonConvert.DeserializeObject<MLogin>(login);
                            numUsu.Text = (deserialize?.usu_codigo).ToString();

                            //Inserto en tabla CtrlModif
                            SqlCommand cmd = new SqlCommand($"INSERT INTO CtrlModif(ctr_codigo, ctr_tipmov, ctr_cpbte, ctr_usuario, ctr_terminal, ctr_fecha, ctr_hora, ctr_modulo) VALUES ({item.IdPedido},{item.TipMov},'{item.NumeroComprobante}',{numUsu.Text}, {terminal},'{DateTime.Now.ToString("d")}','{DateTime.Now.ToString("T")}','VENTAS')", ConexionMaestra.con);
                            await cmd.ExecuteNonQueryAsync();

                            //Inserto en tabla PrepPed
                            SqlCommand PrepPed = new SqlCommand($"INSERT INTO PrepPed (ped_codigo, ped_cpbte, ped_fecemi, ped_estado, ped_horaini, ped_fecfin, ped_horafin, ped_usuario1, " +
                            "ped_usuario2, ped_usuario3, ped_observa, ped_fa, ped_codigoPrep)" +
                            $"SELECT vta_codigo, vta_cpbte, vta_fecemi, vta_estado, '{DateTime.Now.ToString("T")}', NULL, NULL, {numUsu.Text}, NULL, NULL, '', '', 0 " +
                            $"FROM MovVtaPed Where vta_codigo = {item.IdPedido}", ConexionMaestra.con);
                            await PrepPed.ExecuteNonQueryAsync();

                            await Navigation.PushAsync(new DetallePedido());
                        }
                    }
                    else
                    {
                        await DisplayAlert("Mensaje", "Solo puede comenzar el primer pedido", "Aceptar");
                    }
                }
                else if (estadoPicker == "EN CURSO")
                {

                    var accion = await DisplayActionSheet("¿Desea continuar el pedido?", null, null, "SI", "NO");
                    if (accion == "SI")
                    {
                        //Envio informacion sobre el pedido
                        var serializer = JsonConvert.SerializeObject(item);
                        if (item != null)
                        {
                            Preferences.Set("pedido", serializer);
                        }
                        //Informacion sobre usuario y terminal.
                        var login = Preferences.Get("data", "");
                        var deserialize = JsonConvert.DeserializeObject<MLogin>(login);
                        numUsu.Text = (deserialize?.usu_codigo).ToString();

                        await Navigation.PushAsync(new DetallePedido());
                    }
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
    }
}