using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
    public partial class ListaStockFaltante : ContentPage
    {
        VMFaltantes fun = new VMFaltantes();
        public ListaStockFaltante()
        {
            InitializeComponent();
            inicializarVista();


        }
        //string name = DeviceInfo.Name;
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
        public static int name = Convert.ToInt32(File.ReadAllText(ruta));
        private void inicializarVista()
        {
            try
            {
                bool keys = Preferences.ContainsKey("stock");
                if (keys)
                {
                    var dato = Preferences.Get("stock", "");
                    var deserializer = JsonConvert.DeserializeObject<MStock>(dato);
                    fal_codtex.Text = deserializer?.aux_codtex;
                    fal_codnum.Text = Convert.ToString(deserializer?.aux_codnum);
                    fal_descri.Text = deserializer?.aux_descri;
                    fal_adicional.Text = deserializer?.aux_adicional;
                    fal_usuID.Text = Convert.ToString(deserializer?.aux_usuario);
                    fal_cant.Text = Convert.ToString(deserializer?.aux_cantidad);
                }

                bool data = Preferences.ContainsKey("cliente");
                if (data)
                {
                    var dato = Preferences.Get("cliente", "");
                    var deserializer = JsonConvert.DeserializeObject<MClientes>(dato);
                    fal_idcliente.Text = Convert.ToString(deserializer?.cli_codigo);
                }

                bool z = Preferences.ContainsKey("picker");
                if (z)
                {
                    var zz = Preferences.Get("picker", "");
                    var deserializer = JsonConvert.DeserializeObject<MDeposito>(zz);
                    fal_deposito.Text = Convert.ToString(deserializer?.dep_codigo);
                }

                string auxiliar;
                SqlCommand id = new SqlCommand("Select ISNULL(MAX(fal_id),1) from FaltantesApp", ConexionMaestra.con);
                var lastid = id.ExecuteScalar();
                var max = lastid;
                auxiliar = "det_codigo = " + lastid + "";
                if (lastid == max)
                {
                    ListaStock.ItemsSource = fun.mStocks(auxiliar);
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                inicializarVista();
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private void btnAceptar_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void btnFinalizar_Clicked(object sender, EventArgs e)
        {
            try
            {
                SqlCommand last_id = new SqlCommand("Select ISNULL(MAX(fal_id),1) from FaltantesApp", ConexionMaestra.con);
                var lastID = last_id.ExecuteScalar();
                SqlCommand query = new SqlCommand("Select * from DetalleFaltanteApp where det_codigo = "+lastID+"", ConexionMaestra.con);
                SqlDataReader dr = query.ExecuteReader();
                if (dr.Read())
                {
                    var accion = await DisplayActionSheet("¿Desea finalizar la reposición para éste faltante?", null, "Cancelar", "SI", "NO");
                    if (accion == "SI")
                    {
                        SqlCommand ins = new SqlCommand("Update FaltantesApp set fal_terminado = 0, fal_estado = 'REGISTRADO' where fal_terminal = " + name + " and fal_id = " + lastID + "", ConexionMaestra.con);
                        ins.ExecuteNonQuery();
                        await DisplayAlert("Mensaje", "Registro Exitoso!", "OK");
                        await Navigation.PopModalAsync();
                    }
                    else
                    {

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

        private async void btnEliminarStock_Clicked(object sender, EventArgs e)
        {
            try
            {
                Button boton = (Button)sender;
                var data = Preferences.Get("data", "");
                var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
                var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
                var s = (MStock)boton.BindingContext;
                SqlCommand lastid = new SqlCommand("Select MAX(fal_id) from FaltantesApp where fal_idusu = " + usu_codigo + " and fal_terminal = " + name + "", ConexionMaestra.con);
                var id = lastid.ExecuteScalar();
                SqlCommand etiqueta = new SqlCommand("Select det_cantidad from DetalleFaltanteApp where det_codtex = '" + s.aux_codtex + "' and det_codnum = " + s.aux_codnum + " and det_adicional = '" + s.aux_adicional + "' and det_codigo = " + id + "", ConexionMaestra.con);
                int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
                if (dr == 1)
                {
                    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
                    if (accion == "SI")
                    {
                        SqlCommand insert = new SqlCommand("Delete from DetalleFaltanteApp where det_codtex = '" + s.aux_codtex + "' and det_codnum = " + s.aux_codnum + " and det_adicional = '" + s.aux_adicional + "' and det_codigo = " + id + "", ConexionMaestra.con);
                        insert.ExecuteNonQuery();
                        await DisplayAlert("Mensaje", "Ha eliminado el articulo del stock", "OK");
                        OnAppearing();
                        return;
                    }
                    else
                    {

                    }
                }
                if (dr > 0)
                {
                    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
                    if (accion == "SI")
                    {
                        int cont = dr;
                        cont -= 1;
                        SqlCommand update = new SqlCommand("Update DetalleFaltanteApp set det_cantidad = " + cont + " where det_codtex = '" + s.aux_codtex + "' and det_codnum = " + s.aux_codnum + " and det_adicional = '" + s.aux_adicional + "' and det_codigo = " + id + "", ConexionMaestra.con);
                        update.ExecuteNonQuery();
                        await DisplayAlert("Aclaracion", "Quedan " + cont + " etiquetas", "OK");
                        OnAppearing();
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }

        }
    }
}