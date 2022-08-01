using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubMenu : ContentPage
    {
        public SubMenu()
        {
            InitializeComponent();
            inicializarVista();

        }

        private void inicializarVista()
        {
            bool keys = Preferences.ContainsKey("data");
            if (keys)
            {
                //Deserializar es convertir un archivo Json a un objeto
                var data = Preferences.Get("data", "");
                var deserializer = Newtonsoft.Json.JsonConvert.DeserializeObject<MLogin>(data);
                var user = deserializer?.usu_nombre;

                if (user == "SUPERVISOR")
                {
                    btnConf.IsVisible = true;
                }
            }
        }

        private async void btnUbicacion_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new NavigationPage(new UbicacionStock()));
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private async void btnEtiquetas_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new NavigationPage(new Etiquetas()));
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
        public static string ip = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ipServidor.txt");
        public static string ipServ = Convert.ToString(File.ReadAllText(ip));
        public static string nombreBD = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "nombreBD.txt");
        public static string nombrebd = Convert.ToString(File.ReadAllText(nombreBD));
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
        public static int nameTerminal = Convert.ToInt32(File.ReadAllText(ruta));
        private async void btnConf_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new PedidoIP2(ipServ, nombrebd, nameTerminal));
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private async void btnFaltantes_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new NavigationPage(new MercaderiaFaltante()));
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private async void btnPedidos_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new NavigationPage(new Pedidos()));
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
    }
}