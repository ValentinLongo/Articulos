using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Introduccion : ContentPage
    {
        public Introduccion()
        {
            InitializeComponent();
            Animacion();
        }

        int IdUsuario;
        public async void Animacion()
        {
            imgIntro.Opacity = 0;
            await imgIntro.FadeTo(1, 2000);
            probarConexion();
        }
        private void probarConexion()
        {
            try
            {
                VMusuario funcion = new VMusuario();
                funcion.ComprobarConexion(ref IdUsuario); //Llamo a mi funcion para comprobar si la app tiene conexion a la DB
            }
            catch (Exception)
            {
                IdUsuario = 0;
            }
            if (IdUsuario > 0)// Si es mayor a 0 me dirige a la pantalla login
            {
                Application.Current.MainPage = new Login();
            }
            else // sino me hace ingresar un IP del servidor
            {
                Application.Current.MainPage = new NavigationPage(new PedidoIP());
            }
        }

        
    }
}