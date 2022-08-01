using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class AgregarArt : ContentPage
    {

        MAgregarUbi ag = new MAgregarUbi();
        VMUbicaciones funcion = new VMUbicaciones();
        public AgregarArt()
        {
            InitializeComponent();
            inicializarVista();
            PickDepo.ItemsSource = funcion.SelecDepo();
        }

        private void inicializarVista()
        {
            try
            {
                bool keys = Preferences.ContainsKey("dato");
                if (keys)
                {
                    var dato = Preferences.Get("dato", "");
                    var deserializer = JsonConvert.DeserializeObject<MEditarUbicacion>(dato);
                    //txtCodtex.Text = deserializer?.ubi_codtex;
                    //txtCodnum.Text = Convert.ToString(deserializer?.ubi_codnum);
                    //txtUbiAdi.Text = deserializer?.ubi_adicional;
                    txtubi_codigo.Text = Convert.ToString(deserializer?.ubi_codigo);

                }

                bool val = Preferences.ContainsKey("data");
                if (val)
                {
                    var data = Preferences.Get("data", "");
                    var deserializer = JsonConvert.DeserializeObject<MLogin>(data);
                    txtUbi_usualta.Text = Convert.ToString(deserializer?.usu_codigo);

                }

                bool ubi = Preferences.ContainsKey("datoUbi");
                if (ubi)
                {
                    var datoUbi = Preferences.Get("datoUbi", "");
                    var deserializer = JsonConvert.DeserializeObject<MUbicaciones>(datoUbi);
                    txtada_vigencia.Text = Convert.ToString(deserializer?.ada_vigencia);
                    //txtArt_vigencia.Text = Convert.ToString(deserializer?.art_vigencia);
                    txtCodtex.Text = deserializer?.art_codtex;
                    txtCodnum.Text = Convert.ToString(deserializer?.art_codnum);
                    txtUbiAdi.Text = deserializer?.adi_codigo;
                    //txtVigencia.Text = Convert.ToString(deserializer?.Vigencia);
                }
                var vigencia = "art_codtex = '" + txtCodtex.Text + "' and art_codnum = " + txtCodnum.Text + " and ISNULL(adi_codigo,'') = '"+txtUbiAdi.Text+"'";
                ConsultarVigencia(vigencia);
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }

        }

        public ObservableCollection<MUbicaciones> ConsultarVigencia(string campo)
        {
            ObservableCollection<MUbicaciones> Ubicaciones = new ObservableCollection<MUbicaciones>();
            string hostIMG = "http://190.123.89.13:70/";
            string sqlQuery = "SELECT top 50 art_codtex +'-'+ CONVERT(VARCHAR,art_codnum) AS Codigo, art_codtex, art_codfab, art_codinterno, art_codnum, " +
                "(CASE WHEN Adicional.adi_codigo IS NULL THEN '' ELSE Adicional.adi_codigo END) AS adi_codigo, " +
                "(CASE WHEN Adicional.adi_descri IS NULL THEN '' ELSE Adicional.adi_descri END) AS adi_descri, " +
                "(CASE WHEN AdicionalxArtic.ada_adicional IS NULL THEN '' ELSE AdicionalxArtic.ada_adicional END) AS ada_adicional, " +
                 "ISNULL(art_descriWeb, art_descri) AS art_descri, ISNULL( ada_codbarra, art_codbarra ) AS CBarra, " +
                 "(CASE WHEN ada_codtex IS NULL THEN ISNULL(art_vigencia, 0) else ISNULL(ada_vigencia, 0) end) as Vigencia, " +
                 "art_vigencia,(case when ada_vigencia IS NULL then '' else ada_vigencia end) ada_vigencia, " +
                 "CASE WHEN ISNULL(ada_codnum, 0) = 0 Then REPLACE(art_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') " +
                "Else REPLACE(ada_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') END AS imagen FROM Articulo " +
                 "LEFT JOIN AdicionalxArtic ON(art_codtex = AdicionalxArtic.ada_codtex AND art_codnum = AdicionalxArtic.ada_codnum AND " +
                "AdicionalxArtic.ada_vigencia = 1) LEFT JOIN Adicional ON(AdicionalxArtic.ada_adicional = Adicional.adi_codigo) WHERE " + campo + "";
            SqlCommand list = new SqlCommand(sqlQuery, ConexionMaestra.con);
            SqlDataReader dr = list.ExecuteReader();
            if (dr.HasRows != true)
            {
                Application.Current.MainPage.DisplayAlert("Error", "No se encontro el articulo buscado!", "OK");
            }
            else
            {
                while (dr.Read())
                {
                    MUbicaciones art = new MUbicaciones()
                    {
                        Vigencia = Convert.ToInt32(dr["Vigencia"].ToString()),
                        art_vigencia = Convert.ToInt32(dr["art_vigencia"].ToString())
                    };
                    Ubicaciones.Add(art);
                    var serializer = JsonConvert.SerializeObject(Ubicaciones[0]);
                    if (Ubicaciones != null && Ubicaciones.Count > 0)
                    {
                        Preferences.Set("vigencia", serializer);
                    }
                }
            }
            return Ubicaciones;
        }
        public void LlenarPickerDeposito()
        {
            try
            {
                SqlCommand query = new SqlCommand("Select ubi_deposito, dep_descri From Ubicacion Left Join Deposito on ubi_deposito = dep_codigo Where ubi_ubica1 = '" + txtDeposito.Text + "' Group By ubi_deposito, dep_descri Order BY ubi_deposito, dep_descri", ConexionMaestra.con);
                SqlDataReader dr = query.ExecuteReader();
                while (dr.Read())
                {
                    int valor = Convert.ToInt32(dr["ubi_deposito"].ToString());
                    PickDepo.SelectedIndex = valor;
                    txtUbiDepo.Text = dr["ubi_deposito"].ToString();
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        public bool PermisoxUsu() //Corroboro que el usuario logueado tenga los permisos correspondientes
        {

            //pxu = PermisosxUsu
            SqlCommand pxu = new SqlCommand("Select * From Permisos Left Join PermisosxUsu on pef_codigo = pxu_codigo Where pef_sistema = 'AD' And pef_formulario = 'aux_ubicaciones' And pef_control = 'Agregar' And pxu_usuario = " + txtUbi_usualta.Text + " and pxu_activo = 1", ConexionMaestra.con);
            SqlDataReader resu = pxu.ExecuteReader();
            if (resu.Read())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async void agregarUbicacion() //Funcion para agregar una nueva Ubicacion a un articulo
        {
            try
            {
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
                ag.ubi_ubica1 = txtDeposito.Text;
                ag.ubi_ubica2 = txtPasillo.Text;
                ag.ubi_ubica3 = txtFila.Text;
                ag.ubi_ubica4 = txtColumna.Text;

                var vigencia = Preferences.Get("vigencia", "");
                var deserializer = JsonConvert.DeserializeObject<MUbicaciones>(vigencia);
                txtArt_vigencia.Text = Convert.ToString(deserializer?.art_vigencia);
                txtVigencia.Text = Convert.ToString(deserializer?.Vigencia);
                if (txtVigencia.Text != Convert.ToString(1))
                {
                    if (PermisoxUsu() == false) //Si el usuario no tiene permisos no podra hacer ningun cambio
                    {
                        await DisplayAlert("Aviso", "El Articulo y/o su Adicional se encuentra Inhabilitado. Debera Habilitarlo para poder vincular una Ubicación", "Aceptar");
                        return;
                    }
                    else
                    {
                        //Pregunto si se desea habilitar el articulo y/o adicional
                        var accion = await DisplayAlert("Aviso", "El Articulo y/o Adicional se encuentra Inhabilitado. ¿Desea Habilitarlo?", "SI", "NO");
                        if (accion == false)
                        {
                            return; //Salgo de mi ciclo
                        }
                        else // accion == true
                        {
                            if (txtUbiAdi.Text != "") //Si el Adicional es <> ""
                            {
                                SqlCommand cmd = new SqlCommand("update AdicionalxArtic set ada_vigencia = 1 where ada_codtex = '" + txtCodtex.Text + "' and ada_codnum = " + txtCodnum.Text + " and ada_adicional = '" + txtUbiAdi.Text + "'", ConexionMaestra.con);
                                cmd.ExecuteNonQuery();
                                if (txtArt_vigencia.Text != Convert.ToString(1))
                                {
                                    SqlCommand art = new SqlCommand("update Articulo set art_vigencia = 1, art_usuest = " + txtUbi_usualta.Text + ", art_fecest = '" + DateTime.Now.ToString("d") + "', art_horaest = '" + DateTime.Now.ToString("T") + "' where art_codtex = '" + txtCodtex.Text + "' and art_codnum = " + txtCodnum.Text + "", ConexionMaestra.con);
                                    art.ExecuteNonQuery();
                                }
                            }
                            else //Si el Adicional es = ""
                            {
                                SqlCommand sqlquery = new SqlCommand("update Articulo set art_vigencia = 1, art_usuest = " + txtUbi_usualta.Text + ", art_fecest = '" + DateTime.Now.ToString("d") + "', art_horaest = '" + DateTime.Now.ToString("T") + "' where art_codtex = '" + txtCodtex.Text + "' and art_codnum = " + txtCodnum.Text + "", ConexionMaestra.con);
                                sqlquery.ExecuteNonQuery();
                            }
                            SqlCommand par_Actcarrito = new SqlCommand("select * from Parametro where par_ActCarritoxVigencia = 1", ConexionMaestra.con);
                            SqlDataReader val = par_Actcarrito.ExecuteReader();
                            if (val.Read())
                            {
                                SqlCommand carrito = new SqlCommand("update Articulo set art_carrito = 1 where art_codtex = '" + txtCodtex.Text + "' and art_codnum = " + txtCodnum.Text + "", ConexionMaestra.con);
                                carrito.ExecuteNonQuery();
                            }
                        }
                    }
                }
                var ubi_cod = new SqlCommand("select MAX(ubi_codigo)+1 from Ubicacion", ConexionMaestra.con);
                var ubiCodigo = ubi_cod.ExecuteScalar();
                // Consulto a la DB si la Ubicacion que quiero agregar ya existe para otro articulo
                SqlCommand consulUbi = new SqlCommand("select * from Ubicacion where ubi_ubica1 = '" + ag.ubi_ubica1 + "' and ubi_ubica2 = '" + ag.ubi_ubica2 + "' and ubi_ubica3 = '" + ag.ubi_ubica3 + "' and ubi_ubica4 = '" + ag.ubi_ubica4 + "' and NOT(ubi_codtex = '" + txtCodtex.Text + "' and ubi_codnum = " + txtCodnum.Text + " and ubi_adicional = '" + txtUbiAdi.Text + "')", ConexionMaestra.con);
                SqlDataReader consulubi = consulUbi.ExecuteReader();
                if (consulubi.Read())
                {
                    // Pregunto si desea continuar de todas maneras 
                    var rta = await DisplayActionSheet("Atencion: Se ha detectado que la ubicación ya ha sido definida para otro/s Articulo/s ¿Desea Continuar?", "Cancelar", null, "SI", "NO");
                    if (rta == "NO")
                    {
                        return; //Salgo del ciclo
                    }
                    else // rta == SI
                    {
                        SqlCommand consulta = new SqlCommand("insert into Ubicacion(ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_adicional, ubi_usualta, ubi_deposito, ubi_fecalta, ubi_horalta) values (" + ubiCodigo + ",'" + txtCodtex.Text + "'," + txtCodnum.Text + ", UPPER('" + ag.ubi_ubica1 + "'), UPPER('" + ag.ubi_ubica2 + "'), UPPER('" + ag.ubi_ubica3 + "'), UPPER('" + ag.ubi_ubica4 + "'), UPPER('" + txtUbiAdi.Text + "'), UPPER(" + txtUbi_usualta.Text + "), UPPER(" + txtUbiDepo.Text + "), UPPER('" + DateTime.Now.ToString("d") + "'), UPPER('" + DateTime.Now.ToString("T") + "'))", ConexionMaestra.con);
                        int result = consulta.ExecuteNonQuery();
                        if (result > 0)
                        {
                            await DisplayAlert("Mensaje del Sistema", "Se registro con éxito", "OK");
                            txtDeposito.Text = "";
                            txtPasillo.Text = "";
                            txtFila.Text = "";
                            txtColumna.Text = "";
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Mensaje del Sistema", "No se pudo registrar la ubicación", "OK");
                            await Navigation.PopAsync();

                        }
                    }
                }
                else
                {
                    SqlCommand consulta = new SqlCommand("insert into Ubicacion(ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_adicional, ubi_usualta, ubi_deposito, ubi_fecalta, ubi_horalta) values (" + ubiCodigo + ",'" + txtCodtex.Text + "'," + txtCodnum.Text + ", UPPER('" + ag.ubi_ubica1 + "'), UPPER('" + ag.ubi_ubica2 + "'), UPPER('" + ag.ubi_ubica3 + "'), UPPER('" + ag.ubi_ubica4 + "'), UPPER('" + txtUbiAdi.Text + "'), UPPER(" + txtUbi_usualta.Text + "), UPPER(" + txtUbiDepo.Text + "), UPPER('" + DateTime.Now.ToString("d") + "'), UPPER('" + DateTime.Now.ToString("T") + "'))", ConexionMaestra.con);
                    int result = consulta.ExecuteNonQuery();
                    if (result > 0)
                    {
                        await DisplayAlert("Mensaje del Sistema", "Se registro con éxito", "OK");
                        txtDeposito.Text = "";
                        txtPasillo.Text = "";
                        txtFila.Text = "";
                        txtColumna.Text = "";
                        //await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Mensaje del Sistema", "No se pudo registrar la ubicación", "OK");
                        //await Navigation.PopAsync();
                    }
                }
                // Pregunto si el articulo al que quiero agregar la nueva ubicacion ya tiene otras ubicaciones
                SqlCommand x = new SqlCommand("select count(ubi_ubica1) from Ubicacion where ubi_codtex = '" + txtCodtex.Text + "' and ubi_codnum = " + txtCodnum.Text + " and ubi_adicional = '" + txtUbiAdi.Text + "'", ConexionMaestra.con);
                var sqlData = x.ExecuteScalar();
                int intData = (int)sqlData;
                if (intData == 1) // Si es la primer ubicacion para dicho articulo la hago predefinida
                {
                    SqlCommand update = new SqlCommand("update Ubicacion set ubi_predef = 1 where ubi_codtex = '" + txtCodtex.Text + "' and ubi_codnum = " + txtCodnum.Text + " and ubi_adicional = '" + txtUbiAdi.Text + "' and ubi_codigo = " + ubiCodigo + "", ConexionMaestra.con);
                    update.ExecuteNonQuery();
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private void btnAgregar_Clicked(object sender, EventArgs e)
        {
            agregarUbicacion();
        }

        private void txtDeposito_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtDeposito.Text != "")
            {
                LlenarPickerDeposito();
            }
            else
            {
                PickDepo.SelectedItem = "";
            }

        }
    }
}