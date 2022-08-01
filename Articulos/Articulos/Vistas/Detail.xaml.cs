using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.Modelo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Detail : ContentPage
    {
        public Detail()
        {
            InitializeComponent();
            InicializarVista();
        }

        private void InicializarVista()
        {
            bool keys = Xamarin.Essentials.Preferences.ContainsKey("data");
            if (keys)
            {
                //Deseriqalizar es convertir un archivo Json a un objeto
                var data = Xamarin.Essentials.Preferences.Get("data", "");
                var deserializer = Newtonsoft.Json.JsonConvert.DeserializeObject<MLogin>(data);
                user.Text = deserializer?.usu_nombre;
            }

        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}