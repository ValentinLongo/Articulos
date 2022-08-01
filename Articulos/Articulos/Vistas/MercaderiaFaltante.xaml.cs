using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
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
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MercaderiaFaltante : ContentPage
    {
        VMFaltantes fun = new VMFaltantes();

        public MercaderiaFaltante()
        {
            InitializeComponent();
            inicializarVista();
            pickerEstado.Items.Add("REGISTRADOS");
            pickerEstado.Items.Add("FINALIZADOS");
            pickerEstado.Items.Add("EN CURSO");
        }

        private void inicializarVista()
        {

        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                var data = Preferences.Get("data", "");
                var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
                var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
                SqlCommand curso = new SqlCommand("select * from FaltantesApp where fal_idusu = " + usu_codigo + " and fal_estado = 'EN CURSO' ", ConexionMaestra.con);
                SqlDataReader enCurso = curso.ExecuteReader();
                if (enCurso.Read())
                {
                    await DisplayAlert("Advertencia", "Usted tiene un faltante en curso", "OK");
                }
                else
                {
                    await Navigation.PushAsync(new ListaFaltantes());
                }
            }
            catch (Exception)
            {
                ConexionMaestra.abrir();
            }

        }
        public void pickerEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToString(pickerEstado.SelectedItem) == "REGISTRADOS")
                {
                    var serializer = JsonConvert.SerializeObject(pickerEstado.SelectedItem);
                    if (pickerEstado.SelectedItem != null)
                    {
                        Preferences.Set("EstadoPicker", serializer);
                    }
                    var str = "fal_estado = 'REGISTRADO'";
                    list.ItemsSource = fun.Faltantes(str);
                }
                else if (Convert.ToString(pickerEstado.SelectedItem) == "FINALIZADOS")
                {
                    var serializer = JsonConvert.SerializeObject(pickerEstado.SelectedItem);
                    if (pickerEstado.SelectedItem != null)
                    {
                        Preferences.Set("EstadoPicker", serializer);
                    }
                    var str = "fal_estado = 'FINALIZADO' and fal_fecini = '"+DateTime.Now.ToString("d")+"'";
                    list.ItemsSource = fun.Faltantes(str);
                }
                else if (Convert.ToString(pickerEstado.SelectedItem) == "EN CURSO")
                {
                    var serializer = JsonConvert.SerializeObject(pickerEstado.SelectedItem);
                    if (pickerEstado.SelectedItem != null)
                    {
                        Preferences.Set("EstadoPicker", serializer);
                    }
                    var str = "fal_estado = 'EN CURSO'";
                    list.ItemsSource = fun.Faltantes(str);
                }
            }
            catch (Exception ex)
            {
                ConexionMaestra.abrir();
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
            }
        }
        private void list_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as MEstadoFaltante;
            var serializer = JsonConvert.SerializeObject(item);
            if (item != null)
            {
                Preferences.Set("DetFaltantes", serializer);
            }
            Navigation.PushAsync(new DetFaltante());
        }
        private void frameLista_SizeChanged(object sender, EventArgs e)
        {
            Frame frame = (Frame)sender;
            if (Convert.ToString(pickerEstado.SelectedItem) == "REGISTRADOS")
            {
                frame.BackgroundColor = Color.White;
            }
            else if (Convert.ToString(pickerEstado.SelectedItem) == "FINALIZADOS")
            {
                frame.BackgroundColor = Color.LightGreen;
            }
        }
        protected override void OnAppearing()
        {
            try
            {
                refresh.IsRefreshing = false;
                if (Convert.ToString(pickerEstado.SelectedItem) == "REGISTRADOS")
                {
                    var str = "fal_estado = 'REGISTRADO'";
                    list.ItemsSource = fun.Faltantes(str);
                }
                else if (Convert.ToString(pickerEstado.SelectedItem) == "FINALIZADOS")
                {
                    var str = "fal_estado = 'FINALIZADO' and fal_fecini = '" + DateTime.Now.ToString("d") + "'";

                    list.ItemsSource = fun.Faltantes(str);
                }
                else if (Convert.ToString(pickerEstado.SelectedItem) == "EN CURSO")
                {
                    var str = "fal_estado = 'EN CURSO'";
                    list.ItemsSource = fun.Faltantes(str);
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
        private void refresh_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }
    }
}