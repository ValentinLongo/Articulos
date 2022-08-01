using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        VMLogin log = new VMLogin(); //Instancio un nuevo objeto porque sino no podria llamar a la funcion 'ValidarUsuario'
        public Login()
        {
            InitializeComponent();
            BindingContext = new VMLogin(); // Hago la conexion con mi ViewModel correspondiente
        }

        private void btnIngresar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var user = usuario.Text;
                var pas = pass.Text;
                if (user == null || pas == null) //Valido q ambos campos no sean nulos
                {
                    DisplayAlert("Error", "Ingrese Usuario y/o Contraseña", "OK");
                }
                else //Si ambos campos son ingresados llamo a mi funcion para hacer las validaciones necesarias
                {
                    log.ValidarUsuario(usuario.Text, pass.Text);

                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
    }
}