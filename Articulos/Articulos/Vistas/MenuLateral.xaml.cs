using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.Modelo;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS0618 // 'MasterDetailPage' está obsoleto: 'MasterDetailPage is obsolete as of version 5.0.0. Please use FlyoutPage instead.'
    public partial class MenuLateral : MasterDetailPage
#pragma warning restore CS0618 // 'MasterDetailPage' está obsoleto: 'MasterDetailPage is obsolete as of version 5.0.0. Please use FlyoutPage instead.'
    {
        public MenuLateral()
        {
            InitializeComponent();
            Master = new SubMenu(); //Es la ventana desplegada
            Detail = new NavigationPage(new Detail()); //Es la vista que aparece en blanco con el nombre del usuario logueado en la parte superior derecha
        }

        
    }
}