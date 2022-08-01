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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopDescriUbicacion : PopupPage
    {
        public PopDescriUbicacion()
        {
            InitializeComponent();
            inicializarVista();
        }

        private void inicializarVista()
        {
            bool keys = Preferences.ContainsKey("etiqueta");
            if (keys)
            {
                var dato = Preferences.Get("etiqueta", "");
                var deserializer = JsonConvert.DeserializeObject<MEtiquetas>(dato);
                imp_codtex.Text = deserializer?.imp_codtex;
                imp_codnum.Text = Convert.ToString(deserializer?.imp_codnum);
                imp_adicional.Text = Convert.ToString(deserializer?.imp_adicional);
                imp_descri.Text = deserializer?.imp_descri;
                imp_usuario.Text = Convert.ToString(deserializer?.imp_usuario);
                imp_orden.Text = Convert.ToString(deserializer?.imp_orden);
                imp_terminal.Text = Convert.ToString(deserializer?.imp_terminal);
                imp_cantimp.Text = Convert.ToString(deserializer?.imp_cantimp);
            }

            bool val = Preferences.ContainsKey("data");
            if (val)
            {
                var data = Preferences.Get("data", "");
                var deserializer = JsonConvert.DeserializeObject<MLogin>(data);
                userName.Text = Convert.ToString(deserializer?.usu_nombre);
                userID.Text = Convert.ToString(deserializer?.usu_codigo);
            }
        }

        private async void btnAceptar_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtDescriEti.Text))
                {
                    await DisplayAlert("Advertencia", "Escriba un comentario.", "OK");
                }
                else
                {
                    SqlCommand last_id = new SqlCommand("Select ISNULL(MAX(enc_codigo)+1,1) from MovImpEt", ConexionMaestra.con);
                    var lastID = last_id.ExecuteScalar();

                    SqlCommand cmd = new SqlCommand("Insert into MovImpEt (enc_codigo, enc_descri, enc_disp, enc_usuario, enc_fecha, enc_hora, enc_tipo) " +
                        "values (" + lastID + ", UPPER('" + txtDescriEti.Text + "'), '" + imp_terminal.Text + "', " + userID.Text + ", UPPER('" + DateTime.Now.ToString("d") + "'), UPPER('" + DateTime.Now.ToString("T") + "'), 2)", ConexionMaestra.con);
                    int rta = cmd.ExecuteNonQuery();

                    SqlCommand consulta = new SqlCommand("Insert into DetImpEt (det_codigo, det_codtex, det_codnum, det_adicional, det_orden, det_cantidad) " +
                        "Select " + lastID + ", imp_codtex, imp_codnum, imp_adicional, imp_orden, imp_cantimp from AuxApp_ImpEtiqueta where imp_terminal = '" + imp_terminal.Text + "' and imp_tipo = 2", ConexionMaestra.con);
                    int r = consulta.ExecuteNonQuery();

                    SqlCommand delete = new SqlCommand("Delete from AuxApp_ImpEtiqueta where imp_terminal = '" + imp_terminal.Text + "' and imp_tipo = 2", ConexionMaestra.con);
                    delete.ExecuteNonQuery();
                    await DisplayAlert("Mensaje", "Se Registro con Exito", "OK");
                    await PopupNavigation.Instance.PopAllAsync();
                }
            }
            catch
            {
                ConexionMaestra.abrir();
            }
        }

        private void btnCancelar_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}