using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Ubicacion_Articulos.VistaModelo;
using System.Data.SqlClient;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditarUbicacion : ContentPage
    {
        VMUbicaciones fun = new VMUbicaciones();
        public EditarUbicacion(MEditarUbicacion ubi)
        {
            InitializeComponent();
            inicializarVista(ubi);
            //PickDepo.ItemsSource = fun.SelecDepo();
        }

        private void inicializarVista(MEditarUbicacion ubi)
        {
            txtDeposito.Text = ubi.ubi_ubica1;
            txtPasillo.Text = ubi.ubi_ubica2;
            txtFila.Text = ubi.ubi_ubica3;
            txtColumna.Text = ubi.ubi_ubica4;
            txtUbi_codigo.Text = Convert.ToString(ubi.ubi_codigo);
            txtCodTex.Text = ubi.ubi_codtex;
            txtCodNum.Text = Convert.ToString(ubi.ubi_codnum);
            chkUbi_predef.IsChecked = Convert.ToBoolean(ubi.ubi_predef);
            txtAdicional.Text = ubi.ubi_adicional;
            txtUbi_predef.Text = Convert.ToString(ubi.ubi_predef);

            bool val = Xamarin.Essentials.Preferences.ContainsKey("data");
            if (val)
            {
                var data = Xamarin.Essentials.Preferences.Get("data", "");
                var deserializer = Newtonsoft.Json.JsonConvert.DeserializeObject<MLogin>(data);
                txtUbi_usumod.Text = Convert.ToString(deserializer?.usu_codigo);
            }

            bool keys = Xamarin.Essentials.Preferences.ContainsKey("dato");
            if (keys)
            {
                var dato = Xamarin.Essentials.Preferences.Get("dato", "");
                var deserializer = Newtonsoft.Json.JsonConvert.DeserializeObject<MEditarUbicacion>(dato);
                //txtUbi_predef.Text = Convert.ToString(deserializer?.ubi_predef);
            }
        }

        public void LlenarPickerDeposito()
        {
            try
            {
                SqlCommand query = new SqlCommand("Select ubi_deposito, dep_descri From Ubicacion Left Join Deposito on ubi_deposito = dep_codigo Where ubi_ubica1 = '" + txtDeposito.Text + "' Group By ubi_deposito, dep_descri Order BY ubi_deposito, dep_descri", ConexionMaestra.con);
                SqlDataReader dr = query.ExecuteReader();
                while (dr.Read())
                {
                    Deposito.Text = dr["dep_descri"].ToString();
                    //PickDepo.SelectedIndex = Convert.ToInt32(dr["ubi_deposito"].ToString());
                    txtUbi_deposito.Text = dr["ubi_deposito"].ToString();
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }

        }

        private void txtDeposito_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtDeposito.Text != "")
            {
                LlenarPickerDeposito();
            }
            else
            {
                Deposito.Text = "";
                //PickDepo.SelectedItem = "";
            }
        }

        public async void UpdateUbicacion()
        {
            try
            {
                if (chkUbi_predef.IsChecked == true)
                {
                    SqlCommand consul = new SqlCommand("select * from Ubicacion where ubi_codtex = '" + txtCodTex.Text + "' and ubi_codnum = " + txtCodNum.Text + " and ubi_adicional = '" + txtAdicional.Text + "' and ubi_predef = 1", ConexionMaestra.con);
                    SqlDataReader dr = consul.ExecuteReader();
                    if (dr.Read())
                    {
                        SqlCommand cmd = new SqlCommand("Update Ubicacion set ubi_predef = 0 where ubi_codtex = '" + txtCodTex.Text + "' and ubi_codnum = " + txtCodNum.Text + " and ubi_adicional = '" + txtAdicional.Text + "' and ubi_predef = 1", ConexionMaestra.con);
                        cmd.ExecuteNonQuery();
                        SqlCommand str = new SqlCommand("update Ubicacion set ubi_predef = 1 where ubi_codigo = " + txtUbi_codigo.Text + "", ConexionMaestra.con);
                        str.ExecuteNonQuery();
                        //await Navigation.PopAsync();
                    }
                }

                string strUbi = txtDeposito.Text;
                if (string.IsNullOrEmpty(strUbi))
                {
                    string longi = string.Empty.Length.ToString(strUbi);
                    //txtUbica1.Text = string.Format(strUbi, "000");
                    txtDeposito.Text = longi.PadLeft(3, '0');
                }
                if (txtDeposito.Text.Length == 1)
                {
                    txtDeposito.Text = strUbi.PadLeft(3, '0');
                }
                if (txtDeposito.Text.Length == 2)
                {
                    txtDeposito.Text = strUbi.PadLeft(3, '0');
                }
                string strUbi2 = txtPasillo.Text;
                if (string.IsNullOrEmpty(strUbi2))
                {
                    string longi = string.Empty.Length.ToString(strUbi2);
                    //txtUbica1.Text = string.Format(strUbi, "000");
                    txtPasillo.Text = longi.PadLeft(3, '0');
                }
                if (txtPasillo.Text.Length == 1)
                {
                    txtPasillo.Text = strUbi2.PadLeft(3, '0');
                }
                if (txtPasillo.Text.Length == 2)
                {
                    txtPasillo.Text = strUbi2.PadLeft(3, '0');
                }
                string strUbi3 = txtFila.Text;
                if (string.IsNullOrEmpty(strUbi3))
                {
                    string longi = string.Empty.Length.ToString(strUbi3);
                    //txtUbica1.Text = string.Format(strUbi, "000");
                    txtFila.Text = longi.PadLeft(3, '0');
                }
                if (txtFila.Text.Length == 1)
                {
                    txtFila.Text = strUbi3.PadLeft(3, '0');
                }
                if (txtFila.Text.Length == 2)
                {
                    txtFila.Text = strUbi3.PadLeft(3, '0');
                }
                string strUbi4 = txtColumna.Text;
                if (string.IsNullOrEmpty(strUbi4))
                {
                    string longi = string.Empty.Length.ToString(strUbi4);
                    //txtUbica1.Text = string.Format(strUbi, "000");
                    txtColumna.Text = longi.PadLeft(3, '0');
                }
                if (txtColumna.Text.Length == 1)
                {
                    txtColumna.Text = strUbi4.PadLeft(3, '0');
                }
                if (txtColumna.Text.Length == 2)
                {
                    txtColumna.Text = strUbi4.PadLeft(3, '0');
                }

                var accion = await DisplayActionSheet("¿Desea realizar la modificación?", "Cancelar", null, "SI", "NO");
                if (accion == "SI")
                {
                    SqlCommand editquery = new SqlCommand("update Ubicacion set ubi_ubica1 = UPPER('" + txtDeposito.Text + "'), ubi_ubica2 = UPPER('" + txtPasillo.Text + "'), ubi_ubica3 = UPPER('" + txtFila.Text + "'), ubi_ubica4 = UPPER('" + txtColumna.Text + "'), ubi_deposito = UPPER('" + txtUbi_deposito.Text + "'), ubi_usumodi = UPPER('" + txtUbi_usumod.Text + "'), ubi_fecmodi = UPPER('" + DateTime.Now.ToString("d") + "'), ubi_horamodi = UPPER('" + DateTime.Now.ToString("T") + "') where ubi_codigo = '" + txtUbi_codigo.Text + "'", ConexionMaestra.con);
                    int cant;
                    cant = editquery.ExecuteNonQuery();
                    if (cant == 1)
                    {
                        await DisplayAlert("Modificacion", "Registro Modificado", "OK");
                        await Navigation.PopAsync();
                    }
                }
                else 
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
        private void btnGuardar_Clicked(object sender, EventArgs e)
        {
            UpdateUbicacion();
        }

        //private async void btnAceptar_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (chkUbi_predef.IsChecked == true)
        //        {
        //            SqlCommand consul = new SqlCommand("select * from Ubicacion where ubi_codtex = '" + txtCodTex.Text + "' and ubi_codnum = " + txtCodNum.Text + " and ubi_adicional = '" + txtAdicional.Text + "' and ubi_predef = 1", ConexionMaestra.con);
        //            SqlDataReader dr = consul.ExecuteReader();
        //            if (dr.Read())
        //            {
        //                SqlCommand cmd = new SqlCommand("Update Ubicacion set ubi_predef = 0 where ubi_codtex = '" + txtCodTex.Text + "' and ubi_codnum = " + txtCodNum.Text + " and ubi_adicional = '" + txtAdicional.Text + "' and ubi_predef = 1", ConexionMaestra.con);
        //                cmd.ExecuteNonQuery();
        //                SqlCommand str = new SqlCommand("update Ubicacion set ubi_predef = 1 where ubi_codigo = " + txtUbi_codigo.Text + "", ConexionMaestra.con);
        //                str.ExecuteNonQuery();
        //                await DisplayAlert("Mensaje", "Cambio Realizado", "OK");
        //                await PopupNavigation.PopAsync();
        //            }
        //        }
        //        else
        //        {
        //            SqlCommand consul = new SqlCommand("select COUNT(*) from Ubicacion where ubi_codtex = '" + txtCodTex.Text + "' and ubi_codnum = " + txtCodNum.Text + " and ubi_adicional = '" + txtAdicional.Text + "' and ubi_predef = 1", ConexionMaestra.con);
        //            int query = (int)consul.ExecuteScalar();
        //            if (query == 1)
        //            {
        //                await DisplayAlert("Advertencia", "No puede quitar la unica ubicacion predefinida", "OK");
        //                await PopupNavigation.PopAsync();
        //            }
        //            else
        //            {
        //                await DisplayAlert("Advertencia", "Seleccione otra ubicacion como predefinida para poder quitar ésta", "OK");
        //                await PopupNavigation.PopAsync();
        //            }
        //        }
        //    }
        //    catch 
        //    {
        //        ConexionMaestra.abrir();
        //    }
        //}

        private void chkUbi_predef_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            SqlCommand consul = new SqlCommand("select COUNT(*) from Ubicacion where ubi_codtex = '" + txtCodTex.Text + "' and ubi_codnum = " + txtCodNum.Text + " and ubi_adicional = '" + txtAdicional.Text + "' and ubi_predef = 1", ConexionMaestra.con);
            int query = (int)consul.ExecuteScalar();
            if (query == 1)
            {
                chkUbi_predef.IsEnabled = false;
            }
        }
    }
}