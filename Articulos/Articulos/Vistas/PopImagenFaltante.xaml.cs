using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
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
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopImagenFaltante : PopupPage
    {
        VMFaltantes faltante = new VMFaltantes();
        public PopImagenFaltante(MDetFaltantes img)
        {
            InitializeComponent();
            inicializarVista(img);
        }

        private void inicializarVista(MDetFaltantes img)
        {
            try
            {
                //var data = Preferences.Get("ubicacion", "");
                //var deseria = JsonConvert.DeserializeObject<MEditarUbicacion>(data);
                var dato = Preferences.Get("Faltante", "");
                var deserializer = JsonConvert.DeserializeObject<MDetFaltantes>(dato);
                string aux = "ubi_codtex = '" + img.det_codtex + "' and ubi_codnum = " + img.det_codnum + "  and ubi_adicional = '" + img.det_adicional + "'";
                listaUbicaciones.ItemsSource = faltante.traerUbicacion(aux);
                txtCodTex.Text = img.det_codtex;
                txtCodNum.Text = Convert.ToString(img.det_codnum);
                txtAdicional.Text = img.det_adicional;
                imgArtFaltante.Source = img.imagen;
            }
            catch
            {
                ConexionMaestra.abrir();
            }
        }

        private void btnOk_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}