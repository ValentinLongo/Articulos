using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Essentials;
using System.Data.SqlClient;
using Ubicacion_Articulos.Conexion;
using Newtonsoft.Json;
using Ubicacion_Articulos.Modelo;
using System.IO;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListaImprimirUbi : ContentPage
    {
        VMUbicaciones fun = new VMUbicaciones();
        //string name = DeviceInfo.Name;
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
        public static int name = Convert.ToInt32(File.ReadAllText(ruta));
        public ListaImprimirUbi()
        {
            InitializeComponent();
            InicializarVista();
        }

        protected override void OnAppearing()
        {
            try
            {
                InicializarVista();
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        public void InicializarVista()
        {
            try
            {
                ListaEtiqueta.ItemsSource = fun.ImpUbicacion(name);
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
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private async void ButtonFinalizar_Clicked(object sender, EventArgs e)
        {
            try
            {
                SqlCommand query = new SqlCommand("Select * from AuxApp_ImpEtiqueta where imp_terminal = " + imp_terminal.Text + " and imp_tipo = 2", ConexionMaestra.con);
                SqlDataReader dr = query.ExecuteReader();
                if (dr.Read())
                {
                    //await PopupNavigation.Instance.PushAsync(new PopDescriUbicacion());
                    var accion = await DisplayPromptAsync("Mensaje", "Escriba un comentario!", "OK", "Cancelar", "Comentario");
                    if (accion == "")
                    {
                        await DisplayAlert("Advertencia", "Escriba un comentario.", "OK");
                    }
                    else
                    {
                        SqlCommand last_id = new SqlCommand("Select ISNULL(MAX(enc_codigo)+1,1) from MovImpEt", ConexionMaestra.con);
                        var lastID = last_id.ExecuteScalar();

                        SqlCommand cmd = new SqlCommand("Insert into MovImpEt (enc_codigo, enc_descri, enc_disp, enc_usuario, enc_fecha, enc_hora, enc_tipo) " +
                            "values (" + lastID + ", UPPER('" + accion + "'), '" + imp_terminal.Text + "', " + imp_usuario.Text + ", UPPER('" + DateTime.Now.ToString("d") + "'), UPPER('" + DateTime.Now.ToString("T") + "'), 2)", ConexionMaestra.con);
                        int rta = cmd.ExecuteNonQuery();

                        SqlCommand consulta = new SqlCommand("Insert into DetImpEt (det_codigo, det_codtex, det_codnum, det_adicional, det_orden, det_cantidad) " +
                            "Select " + lastID + ", imp_codtex, imp_codnum, imp_adicional, imp_orden, imp_cantimp from AuxApp_ImpEtiqueta where imp_terminal = '" + imp_terminal.Text + "' and imp_tipo = 2", ConexionMaestra.con);
                        int r = consulta.ExecuteNonQuery();

                        SqlCommand delete = new SqlCommand("Delete from AuxApp_ImpEtiqueta where imp_terminal = '" + imp_terminal.Text + "' and imp_tipo = 2", ConexionMaestra.con);
                        delete.ExecuteNonQuery();
                        await DisplayAlert("Mensaje", "Se Registro con Exito", "OK");
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    btnFinalizar.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private void btnContinuar_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void Eliminar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var data = Preferences.Get("data", "");
                var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
                var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
                Button obj = (Button)sender;
                var etiq = (MEtiquetas)obj.BindingContext;
                SqlCommand etiqueta = new SqlCommand("Select imp_cantimp from AuxApp_ImpEtiqueta where imp_codtex = '" + etiq.imp_codtex + "' and imp_codnum = " + etiq.imp_codnum + " and imp_adicional = '" + etiq.imp_adicional + "' and imp_terminal = " + name + " and imp_tipo = 2", ConexionMaestra.con);
                int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
                if (dr == 1)
                {
                    var z = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar este articulo de la lista?", null, "Cancelar", "SI", "NO");
                    if (z == "SI")
                    {
                        SqlCommand insert = new SqlCommand("Delete from AuxApp_ImpEtiqueta where imp_codtex = '" + etiq.imp_codtex + "' and imp_codnum = " + etiq.imp_codnum + " and imp_adicional = '" + etiq.imp_adicional + "' and imp_terminal = " + name + " and imp_tipo = 2", ConexionMaestra.con);
                        insert.ExecuteNonQuery();
                        SqlCommand ultimo = new SqlCommand("Select * from AuxApp_ImpEtiqueta where imp_terminal = " + name + " and imp_tipo = 2", ConexionMaestra.con);
                        SqlDataReader d = ultimo.ExecuteReader();
                        if (d.Read())
                        {
                            OnAppearing();
                            return;
                        }
                        else
                        {
                            await DisplayAlert("Mensaje", "Eliminó el articulo de la lista de ubicaciones a imprimir", "OK");
                            await Navigation.PopAsync();
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }

        }
    }
}