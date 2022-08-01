using System;
using Ubicacion_Articulos.Vistas;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Articulos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Introduccion();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
