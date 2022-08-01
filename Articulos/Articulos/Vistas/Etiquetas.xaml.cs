using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Etiquetas : ContentPage
    {
        VMUbicaciones funcion = new VMUbicaciones();
        public Etiquetas()
        {
            InitializeComponent();
            ColorMostrarLista();
            picker.Items.Add("Descripción");
            picker.Items.Add("Ubicación");
            picker.Items.Add("Código de Barras");
            picker.Items.Add("Código Interno");
            picker.Items.Add("Código");
            picker.Items.Add("Código Fábrica");
        }
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
        public static int name = Convert.ToInt32(File.ReadAllText(ruta));
        private void ColorMostrarLista()
        {
            try
            {
                SqlCommand query = new SqlCommand("Select * from AuxApp_ImpEtiqueta where imp_terminal = " + name + " and imp_tipo = 1", ConexionMaestra.con);
                SqlDataReader dr = query.ExecuteReader();

                if (dr.Read())
                {
                    btnMostrarListaEtiq.BackgroundColor = Color.FromHex("#2196F3");
                    btnMostrarListaEtiq.TextColor = Color.White;
                }
                else
                {
                    btnMostrarListaEtiq.BackgroundColor = Color.FromHex("#E0E0E0");
                    btnMostrarListaEtiq.TextColor = Color.Black;
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
                ColorMostrarLista();
                refreshing.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }

        }
        //Escanear codigo de barra o QR
        private async void Scanner()
        {
            ZXingScannerPage scannerPage = new ZXingScannerPage();
            scannerPage.OnScanResult += (result) =>
            {
                scannerPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(() =>
                {

                    Navigation.PopAsync();
                    //DisplayAlert("Resultado", result.Text, "OK");
                    txtCodBarra.Text = result.Text;
                    buscarArticulo();
                });
            };
            await Navigation.PushAsync(scannerPage);
        }
        private void picker_SelectedIndexChanged(object sender, EventArgs e) //ComboBox
        {
            if (Convert.ToString(picker.SelectedItem) == "Descripción")
            {
                txtUbica1.IsVisible = false;
                txtUbica2.IsVisible = false;
                txtUbica3.IsVisible = false;
                txtUbica4.IsVisible = false;
                txtCodNum.IsVisible = false;
                txtCodTex.IsVisible = false;
                txtCodBarra.IsVisible = false;
                btnCamara.IsVisible = false;
                txtArticulo.IsVisible = true;
                txtCodFabrica.IsVisible = false;
                txtCodigoInt.IsVisible = false;
            }
            else if (Convert.ToString(picker.SelectedItem) == "Ubicación")
            {
                txtUbica1.IsVisible = true;
                txtUbica2.IsVisible = true;
                txtUbica3.IsVisible = true;
                txtUbica4.IsVisible = true;
                txtArticulo.IsVisible = false;
                txtCodBarra.IsVisible = false;
                txtCodNum.IsVisible = false;
                txtCodTex.IsVisible = false;
                btnCamara.IsVisible = false;
                txtCodigoInt.IsVisible = false;
                txtCodFabrica.IsVisible = false;
            }
            else if (Convert.ToString(picker.SelectedItem) == "Código de Barras")
            {
                txtUbica1.IsVisible = false;
                txtUbica2.IsVisible = false;
                txtUbica3.IsVisible = false;
                txtUbica4.IsVisible = false;
                txtArticulo.IsVisible = false;
                txtCodBarra.IsVisible = true;
                btnCamara.IsVisible = true;
                txtCodNum.IsVisible = false;
                txtCodTex.IsVisible = false;
                txtCodigoInt.IsVisible = false;
                txtCodFabrica.IsVisible = false;
            }
            else if (Convert.ToString(picker.SelectedItem) == "Código")
            {
                txtUbica1.IsVisible = false;
                txtUbica2.IsVisible = false;
                txtUbica3.IsVisible = false;
                txtUbica4.IsVisible = false;
                txtArticulo.IsVisible = false;
                txtCodBarra.IsVisible = false;
                btnCamara.IsVisible = false;
                txtCodNum.IsVisible = true;
                txtCodTex.IsVisible = true;
                txtCodigoInt.IsVisible = false;
                txtCodFabrica.IsVisible = false;
            }
            else if (Convert.ToString(picker.SelectedItem) == "Código Interno")
            {
                txtUbica1.IsVisible = false;
                txtUbica2.IsVisible = false;
                txtUbica3.IsVisible = false;
                txtUbica4.IsVisible = false;
                txtArticulo.IsVisible = false;
                txtCodBarra.IsVisible = false;
                btnCamara.IsVisible = false;
                txtCodNum.IsVisible = false;
                txtCodTex.IsVisible = false;
                txtCodigoInt.IsVisible = true;
                txtCodFabrica.IsVisible = false;
            }
            else if (Convert.ToString(picker.SelectedItem) == "Código Fábrica")
            {
                txtUbica1.IsVisible = false;
                txtUbica2.IsVisible = false;
                txtUbica3.IsVisible = false;
                txtUbica4.IsVisible = false;
                txtArticulo.IsVisible = false;
                txtCodBarra.IsVisible = false;
                btnCamara.IsVisible = false;
                txtCodNum.IsVisible = false;
                txtCodTex.IsVisible = false;
                txtCodigoInt.IsVisible = false;
                txtCodFabrica.IsVisible = true;
            }
        }
        public async void buscarArticulo() // Hago la busqueda correspondiente dependiendo lo que elegi en mi combobox
        {
            int habArt;
            if (CheckArtHabilitados.IsChecked == true)
            {
                habArt = 1;
            }
            else
            {
                habArt = 2;
            }
            string AuxCampo;

            if (Convert.ToString(picker.SelectedItem) == "Descripción") //Busqueda por descripcion de articulos
            {
                try
                {
                    AuxCampo = " art_descri like '%" + txtArticulo.Text + "%' and art_vigencia = " + habArt + "";
                    ListaArt.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtArticulo.Text))
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.art_descri.ToLower().Contains(txtArticulo.Text.ToLower()));
                        txtArticulo.Text = "";
                    }
                    ListaArt.EndRefresh();
                }
                catch
                {
                    await DisplayAlert("Fallo la conexión", "Por favor vuelva a buscar", "Aceptar");
                    ConexionMaestra.abrir();
                }

            }
            else if (Convert.ToString(picker.SelectedItem) == "Código de Barras") //Busqueda por codigo de barra 
            {
                try
                {
                    AuxCampo = " CASE WHEN ada_codnum is null then art_codbarra ELSE ada_codbarra END = '" + txtCodBarra.Text + "' and art_vigencia = " + habArt + "";
                    ListaArt.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtCodBarra.Text))
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.CBarra.ToLower().Contains(txtCodBarra.Text.ToLower()));
                        txtCodBarra.Text = "";
                    }
                    ListaArt.EndRefresh();
                }
                catch
                {
                    await DisplayAlert("Fallo la conexión", "Por favor vuelva a buscar", "Aceptar");
                    ConexionMaestra.abrir();
                }
            }
            else if (Convert.ToString(picker.SelectedItem) == "Ubicación") //Busqueda por ubicacion
            {
                try
                {
                    string strUbi = txtUbica1.Text;
                    if (string.IsNullOrEmpty(strUbi))
                    {
                        string longi = string.Empty.Length.ToString(strUbi);
                        //txtUbica1.Text = string.Format(strUbi, "000");
                        txtUbica1.Text = longi.PadLeft(3, '0');
                    }
                    if (txtUbica1.Text.Length == 1)
                    {
                        txtUbica1.Text = strUbi.PadLeft(3, '0');
                    }
                    if (txtUbica1.Text.Length == 2)
                    {
                        txtUbica1.Text = strUbi.PadLeft(3, '0');
                    }
                    string strUbi2 = txtUbica2.Text;
                    if (string.IsNullOrEmpty(strUbi2))
                    {
                        string longi = string.Empty.Length.ToString(strUbi2);
                        //txtUbica1.Text = string.Format(strUbi, "000");
                        txtUbica2.Text = longi.PadLeft(3, '0');
                    }
                    if (txtUbica2.Text.Length == 1)
                    {
                        txtUbica2.Text = strUbi2.PadLeft(3, '0');
                    }
                    if (txtUbica2.Text.Length == 2)
                    {
                        txtUbica2.Text = strUbi2.PadLeft(3, '0');
                    }
                    string strUbi3 = txtUbica3.Text;
                    if (string.IsNullOrEmpty(strUbi3))
                    {
                        string longi = string.Empty.Length.ToString(strUbi3);
                        //txtUbica1.Text = string.Format(strUbi, "000");
                        txtUbica3.Text = longi.PadLeft(3, '0');
                    }
                    if (txtUbica3.Text.Length == 1)
                    {
                        txtUbica3.Text = strUbi3.PadLeft(3, '0');
                    }
                    if (txtUbica3.Text.Length == 2)
                    {
                        txtUbica3.Text = strUbi3.PadLeft(3, '0');
                    }
                    string strUbi4 = txtUbica4.Text;
                    if (string.IsNullOrEmpty(strUbi4))
                    {
                        string longi = string.Empty.Length.ToString(strUbi4);
                        //txtUbica1.Text = string.Format(strUbi, "000");
                        txtUbica4.Text = longi.PadLeft(3, '0');
                    }
                    if (txtUbica4.Text.Length == 1)
                    {
                        txtUbica4.Text = strUbi4.PadLeft(3, '0');
                    }
                    if (txtUbica4.Text.Length == 2)
                    {
                        txtUbica4.Text = strUbi4.PadLeft(3, '0');
                    }
                    var ubica = txtUbica1.Text + '-' + txtUbica2.Text + '-' + txtUbica3.Text + '-' + txtUbica4.Text;
                    AuxCampo = "(ubi_ubica1 + '-' + ubi_ubica2 +'-'+ ubi_ubica3 +'-'+ ubi_ubica4) = '" + ubica + "' and art_vigencia = " + habArt + "";
                    ListaArt.BeginRefresh();
                    if (ubica == "000-000-000-000")
                    {
                        await DisplayAlert("Advertencia", "Debe Completar Campos para Realizar la Busqueda", "OK");
                        txtUbica1.Text = "";
                        txtUbica2.Text = "";
                        txtUbica3.Text = "";
                        txtUbica4.Text = "";
                    }
                    else
                    {
                        ListaArt.ItemsSource = funcion.BuscarUbicacion(AuxCampo);
                        txtUbica1.Text = "";
                        txtUbica2.Text = "";
                        txtUbica3.Text = "";
                        txtUbica4.Text = "";
                    }
                    ListaArt.EndRefresh();
                }
                catch
                {
                    await DisplayAlert("Fallo la conexión", "Por favor vuelva a buscar", "Aceptar");
                    ConexionMaestra.abrir();
                }
            }
            else if (Convert.ToString(picker.SelectedItem) == "Código") //Busqueda por Codigo 
            {
                try
                {
                    var codigo = txtCodTex.Text + '-' + txtCodNum.Text;
                    if (!string.IsNullOrEmpty(txtCodTex.Text) && !string.IsNullOrEmpty(txtCodNum.Text))
                    {
                        AuxCampo = "art_codtex +'-'+ CONVERT(VARCHAR,art_codnum) = '" + codigo + "' and art_vigencia = " + habArt + "";
                        ListaArt.BeginRefresh();
                        if (string.IsNullOrWhiteSpace(codigo))
                        {
                            ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo);
                        }
                        else
                        {
                            ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.Codigo.ToLower().Contains(codigo.ToLower()));
                            txtCodNum.Text = "";
                            txtCodTex.Text = "";
                        }
                        ListaArt.EndRefresh();
                    }
                    else if (string.IsNullOrEmpty(txtCodNum.Text))
                    {
                        await DisplayAlert("Error", "El campo del código numerico no puede ser vacio", "OK");
                    }
                    else
                    {
                        var codigo1 = txtCodNum.Text;
                        AuxCampo = "CONVERT(VARCHAR,art_codnum) = '" + codigo1 + "' and art_vigencia = " + habArt + "";
                        ListaArt.BeginRefresh();
                        if (string.IsNullOrWhiteSpace(codigo1))
                        {
                            ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo);
                        }
                        else
                        {
                            ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.Codigo.ToLower().Contains(codigo1.ToLower()));
                            txtCodNum.Text = "";
                            txtCodTex.Text = "";
                        }
                        ListaArt.EndRefresh();
                    }
                }
                catch
                {
                    await DisplayAlert("Fallo la conexión", "Por favor vuelva a buscar", "Aceptar");
                    ConexionMaestra.abrir();
                }

            }
            else if (Convert.ToString(picker.SelectedItem) == "Código Interno") //Busqueda por codigo interno
            {
                try
                {
                    AuxCampo = "art_codinterno = '" + txtCodigoInt.Text + "' and art_vigencia = " + habArt + "";
                    ListaArt.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtCodigoInt.Text))
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.art_codinterno.ToLower().Contains(txtCodigoInt.Text.ToLower()));
                        txtCodigoInt.Text = "";
                    }
                    ListaArt.EndRefresh();
                }
                catch
                {
                    await DisplayAlert("Fallo la conexión", "Por favor vuelva a buscar", "Aceptar");
                    ConexionMaestra.abrir();
                }
            }
            else if (Convert.ToString(picker.SelectedItem) == "Código Fábrica")
            {
                try
                {
                    AuxCampo = "art_codfab = '" + txtCodFabrica.Text + "' and art_vigencia = " + habArt + "";
                    ListaArt.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtCodFabrica.Text))
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaArt.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.art_codfab.ToString().ToLower().Contains(txtCodFabrica.Text.ToLower()));
                        txtCodFabrica.Text = "";
                    }
                    ListaArt.EndRefresh();
                }
                catch
                {
                    await DisplayAlert("Fallo la conexión", "Por favor vuelva a buscar", "Aceptar");
                    ConexionMaestra.abrir();
                }
            }
        }
        private void btnCamara_Clicked(object sender, EventArgs e)
        {
            Scanner();
        }

        private void btnBuscar_Clicked(object sender, EventArgs e)
        {
            buscarArticulo();
        }

        private void ListaArt_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void btnMostrarListaEtiq_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ListaEtiquetas());
        }

        private void refreshing_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
            ColorMostrarLista();
        }

        private void ColorFila_SizeChanged(object sender, EventArgs e)
        {
            Frame frame = (Frame)sender;
            if (CheckArtHabilitados.IsChecked == false)
            {
                frame.BackgroundColor = Color.LightGray;
            }
        }
    }
}