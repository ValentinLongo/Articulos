using Newtonsoft.Json;
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
    public partial class DetalleArticuloPedido : ContentPage
    {
        VMDetalleArticuloPedido funcion = new VMDetalleArticuloPedido();
        public DetalleArticuloPedido()
        {
            InitializeComponent();
            InicializarVista();
        }

        private void InicializarVista()
        {
            try
            {
                var codigo = Preferences.Get("articulo", "");
                var deserialize = JsonConvert.DeserializeObject<MDetallePedido>(codigo);
                CodigoArticulo.Text = Convert.ToString(deserialize?.IdArticulos);
                CodigoPedido.Text = Convert.ToString(deserialize?.IdPedido);
                DescripcionArticulo.Text = Convert.ToString(deserialize?.Articulo);
                AdicionalArticulo.Text = Convert.ToString(deserialize?.DescAdicional);
                CodTex.Text = Convert.ToString(deserialize?.CodTex);
                Cantidad.Text = Convert.ToString(deserialize?.Cantidad);
                CodAdi.Text = Convert.ToString(deserialize?.Adicional);
                Cantidadd.Text = Convert.ToString(deserialize?.numPrep);
                //Ubicacion.Text = Convert.ToString(deserialize?.Ubicacion);
                string path = Convert.ToString(deserialize?.Imagen);
                imgProducto.Source = path;

                string aux = $"ubi_codtex = '{deserialize?.CodTex}' and ubi_codnum = {deserialize?.IdArticulos}  and ubi_adicional = '{deserialize?.DescAdicional}'";
                listaUbi.ItemsSource = funcion.traerUbicacion(aux);

                ArticuloReemplazo.Text = "";
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
                var bandera = Preferences.Get("bandera", "");
                if (bandera == "1")
                {
                    Navigation.PopAsync();
                    Preferences.Set("bandera", "");
                }
                var codigo2 = Preferences.Get("datoUbic", "");
                var deserialize2 = JsonConvert.DeserializeObject<MUbicaciones>(codigo2);
                if (codigo2 != "")
                {
                    ArticuloReemplazo.Text = Convert.ToString(deserialize2?.art_descri);
                    CodTexRe.Text = Convert.ToString(deserialize2?.art_codtex);
                    CodNumRe.Text = Convert.ToString(deserialize2?.art_codnum);
                    AdicionalRe.Text = Convert.ToString(deserialize2?.adi_codigo);
                    AdicionalDescriRem.Text = Convert.ToString(deserialize2?.adi_descri);
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private void BtnUbi_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BuscarArticuloPedido());
        }

        private void Combinar_Clicked(object sender, EventArgs e)
        {
            try
            {
                int cantidadPreparada = Convert.ToInt32(Cantidadd.Text);
                int cantidadPedida = Convert.ToInt32(Cantidad.Text);
                if (Cantidadd.Text != null && (cantidadPreparada < cantidadPedida))
                {
                    SqlCommand query = new SqlCommand($"UPDATE MovArticPrep SET his_cpreparada = {cantidadPreparada} WHERE his_codigo = {CodigoPedido.Text} and his_codtex = '{CodTex.Text}' and his_codnum = {CodigoArticulo.Text} AND his_adicional = '{CodAdi.Text}'", ConexionMaestra.con);
                    query.ExecuteNonQuery();
                    Navigation.PushAsync(new CombinarArticuloPedido());
                }
                else
                {
                    DisplayAlert("Mensaje", "Para combinar, la cantidad preparada debe ser mayor a cero y menor a la cantidad requerida", "Ok");
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private void Aceptar_Clicked(object sender, EventArgs e)
        {
            try
            {
                int cantidadPreparada = Convert.ToInt32(Cantidadd.Text);
                int cantidadPedida = Convert.ToInt32(Cantidad.Text);

                if (cantidadPreparada <= cantidadPedida)
                {
                    SqlCommand query = new SqlCommand($"UPDATE MovArticPrep SET his_cpreparada = {cantidadPreparada} WHERE his_codigo = {CodigoPedido.Text} and his_codtex = '{CodTex.Text}' and his_codnum = {CodigoArticulo.Text} AND his_adicional = '{CodAdi.Text}'", ConexionMaestra.con);
                    query.ExecuteNonQuery();
                    Navigation.PopAsync();
                }
                else
                {
                    DisplayAlert("Mensaje", "La cantidad ingresada es mayor a la solicitada", "OK");
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