using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddFaltantes : ContentPage
    {
        VMFaltantes funcion = new VMFaltantes();
        public AddFaltantes()
        {
            InitializeComponent();
            inicializarVista();
            picker.Items.Add("Descripción");
            picker.Items.Add("Ubicación");
            picker.Items.Add("Codigo de Barras");
            picker.Items.Add("Codigo Interno");
            picker.Items.Add("Codigo");
            picker.Items.Add("Código Fábrica");
        }

        private void inicializarVista()
        {

        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
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
            else if (Convert.ToString(picker.SelectedItem) == "Codigo de Barras")
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
            else if (Convert.ToString(picker.SelectedItem) == "Codigo")
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
            else if (Convert.ToString(picker.SelectedItem) == "Codigo Interno")
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
                    ListaFaltante.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtArticulo.Text))
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.art_descri.ToLower().Contains(txtArticulo.Text.ToLower()));
                        txtArticulo.Text = "";
                    }
                    ListaFaltante.EndRefresh();
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
                    ListaFaltante.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtCodBarra.Text))
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.CBarra.ToLower().Contains(txtCodBarra.Text.ToLower()));
                        txtCodBarra.Text = "";
                    }
                    ListaFaltante.EndRefresh();
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
                    ListaFaltante.BeginRefresh();
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
                        ListaFaltante.ItemsSource = funcion.BuscarUbicacion(AuxCampo);
                        txtUbica1.Text = "";
                        txtUbica2.Text = "";
                        txtUbica3.Text = "";
                        txtUbica4.Text = "";
                    }
                    ListaFaltante.EndRefresh();
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
                        ListaFaltante.BeginRefresh();
                        if (string.IsNullOrWhiteSpace(codigo))
                        {
                            ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo);
                        }
                        else
                        {
                            ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.Codigo.ToLower().Contains(codigo.ToLower()));
                            txtCodNum.Text = "";
                            txtCodTex.Text = "";
                        }
                        ListaFaltante.EndRefresh();
                    }
                    else if (string.IsNullOrEmpty(txtCodNum.Text))
                    {
                        await DisplayAlert("Error", "El campo del código numerico no puede ser vacio", "OK");
                    }
                    else
                    {
                        var codigo1 = txtCodNum.Text;
                        AuxCampo = "CONVERT(VARCHAR,art_codnum) = '" + codigo1 + "' and art_vigencia = " + habArt + "";
                        ListaFaltante.BeginRefresh();
                        if (string.IsNullOrWhiteSpace(codigo1))
                        {
                            ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo);
                        }
                        else
                        {
                            ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.Codigo.ToLower().Contains(codigo1.ToLower()));
                            txtCodNum.Text = "";
                            txtCodTex.Text = "";
                        }
                        ListaFaltante.EndRefresh();
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
                    ListaFaltante.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtCodigoInt.Text))
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.art_codinterno.ToLower().Contains(txtCodigoInt.Text.ToLower()));
                        txtCodigoInt.Text = "";
                    }
                    ListaFaltante.EndRefresh();
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
                    ListaFaltante.BeginRefresh();
                    if (string.IsNullOrWhiteSpace(txtCodFabrica.Text))
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo);
                    }
                    else
                    {
                        ListaFaltante.ItemsSource = funcion.LlenarLista(AuxCampo).Where(x => x.art_codfab.ToString().ToLower().Contains(txtCodFabrica.Text.ToLower()));
                        txtCodFabrica.Text = "";
                    }
                    ListaFaltante.EndRefresh();
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

        private void ListaFaltante_ItemTapped(object sender, ItemTappedEventArgs e)
        {

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