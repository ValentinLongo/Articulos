using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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
    public partial class DetallesArt : ContentPage
    {
        VMUbicaciones funcion = new VMUbicaciones();
        public DetallesArt()
        {
            InitializeComponent();
            inicializarVista();
        }

        private void inicializarVista()
        {
            try
            {
                var data = Preferences.Get("datoUbi", "");
                // var item = JsonConvert.DeserializeObject<MUbicaciones>(data);
                var item = JsonConvert.DeserializeObject<MUbicaciones>(data);
                string aux = "ubi_codtex = '" + item.art_codtex + "' and ubi_codnum = " + item.art_codnum + "  and ubi_adicional = '" + item.adi_codigo + "'";
                var serializer = JsonConvert.SerializeObject(aux);
                if (aux != null)
                {
                    Preferences.Set("aux", serializer);
                }
                listaUbi.ItemsSource = funcion.traerUbicacion(aux);
                txtCodigo.Text = item.Codigo;
                txtDescri.Text = item.art_descri;
                imgProducto.Source = item.imagen;
                txtAdi_descri.Text = item.adi_descri;
                txtCodNum.Text = Convert.ToString(item.art_codnum);
                txtCodText.Text = item.art_codtex;
                txtAdi_codigo.Text = item.adi_codigo;
                var vigencia = "art_codtex = '" + txtCodText.Text + "' and art_codnum = " + txtCodNum.Text + " and ISNULL(adi_codigo,'') = '"+txtAdi_codigo.Text+"'";
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
        protected override void OnAppearing()
        {
            try
            {
                inicializarVista();
                refreshDetalle.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();

            }

        }

        private async void agUbicacion_Clicked(object obj, EventArgs e)
        {
            await Navigation.PushAsync(new AgregarArt());
        }

        private void checkUbi_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            try
            {
                var cb = (CheckBox)sender;
                var item = (MEditarUbicacion)cb.BindingContext;
                var x = e.Value;
                if (x == false)
                {
                    SqlCommand check = new SqlCommand("Select count(ubi_ubica1) from Ubicacion where ubi_codtex = '" + txtCodText.Text + "' and ubi_codnum = " + txtCodNum.Text + " and ubi_adicional = '" + txtAdi_codigo.Text + "'", ConexionMaestra.con);
                    var chk = check.ExecuteScalar();
                    int intcheck = (int)chk;
                    if (intcheck == 1)
                    {
                        cb.IsChecked = true;
                    }
                }
                else
                {
                    string auxiliar = "ubi_codtex = '" + txtCodText.Text + "' and ubi_codnum = " + txtCodNum.Text + "  and ubi_adicional = '" + txtAdi_codigo.Text + "'";
                    for (int i = 0; i < funcion.traerUbicacion(auxiliar).Count; i++)
                    {
                            try
                            {
                                if (item.ubi_predef == 1)
                                {
                                    cb.IsChecked = true;
                                }
                                else
                                {
                                    cb.IsChecked = false;
                                }
                            }
                            catch
                            {

                            }
                    }
                    SqlCommand Query = new SqlCommand("update Ubicacion set ubi_predef = 0 where ubi_codtex = '" + txtCodText.Text + "' and ubi_codnum = " + txtCodNum.Text + "  and ubi_adicional = '" + txtAdi_codigo.Text + "'", ConexionMaestra.con);
                    Query.ExecuteNonQuery();
                    SqlCommand str = new SqlCommand("update Ubicacion set ubi_predef = 1 where ubi_codigo = " + item.ubi_codigo + "", ConexionMaestra.con);
                    str.ExecuteNonQuery();
                    //string auxi = "ubi_codtex = '" + txtCodText.Text + "' and ubi_codnum = " + txtCodNum.Text + "  and ubi_adicional = '" + txtAdicional.Text + "'";
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private void refreshDetalle_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
            inicializarVista();
        }

        private async void btnEditar_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            var h = (MEditarUbicacion)button.BindingContext;
            await Navigation.PushAsync(new EditarUbicacion(h));
        }

        public bool PermisoxUsu() //Corroboro que el usuario logueado tenga los permisos correspondientes
        {
            var data = Preferences.Get("data", "");
            var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
            var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
            //pxu = PermisosxUsu
            SqlCommand pxu = new SqlCommand("Select * From Permisos Left Join PermisosxUsu on pef_codigo = pxu_codigo Where pef_sistema = 'AD' And pef_formulario = 'aux_ubicaciones' And pef_control = 'Agregar' And pxu_usuario = " + usu_codigo + " and pxu_activo = 1", ConexionMaestra.con);
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
        private async void btnEliminar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var vigencia = Preferences.Get("vigencia", "");
                var deseria = JsonConvert.DeserializeObject<MUbicaciones>(vigencia);
                var txtArt_vigencia = Convert.ToString(deseria?.art_vigencia);
                var txtVigencia = Convert.ToString(deseria?.Vigencia);
                Button button = (Button)sender;
                var h = (MEditarUbicacion)button.BindingContext;
                //funcion.ubi.Remove(h);
                SqlCommand cmd = new SqlCommand("select count(ubi_ubica1) from Ubicacion where ubi_codtex = '" + h.ubi_codtex + "' and ubi_codnum = " + h.ubi_codnum + " and ubi_adicional = '" + h.ubi_adicional + "'", ConexionMaestra.con);
                var sql = cmd.ExecuteScalar();
                int intsql = (int)sql;
                if (intsql > 1) //Si dicho articulo tiene mas de una ubicacion 
                {
                    if (h.ubi_predef == 1) // Si la ubicacion que quiero eliminar es la predefinida
                    {
                        await DisplayAlert("Advertencia", "Deberá Predefinir otra Ubicación antes de Eliminar ésta.", "OK");
                        return;
                    }
                }

                bool ubi = Preferences.ContainsKey("datoUbi");
                if (ubi)
                {
                    var datoUbi = Preferences.Get("datoUbi", "");
                    var deserializer = JsonConvert.DeserializeObject<MUbicaciones>(datoUbi);
                    var txtada_vigencia = Convert.ToString(deserializer?.ada_vigencia);
                    //var txtArt_vigencia = Convert.ToString(deserializer?.art_vigencia);
                    var txtCodtex = deserializer?.art_codtex;
                    var txtCodnum = Convert.ToString(deserializer?.art_codnum);
                    var txtUbiAdi = deserializer?.adi_codigo;
                    //var txtVigencia = Convert.ToString(deserializer?.Vigencia);

                    var data = Preferences.Get("data", "");
                    var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
                    var txtUbi_usualta = Convert.ToString(deserialize?.usu_codigo);

                    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar el registro?", "Cancelar", null, "SI", "NO");
                    if (accion == "SI")
                    {
                        if (intsql == 1) //Si el articulo tiene una unica ubicacion 
                        {
                            if (txtVigencia == Convert.ToString(1)) // art_vigencia y ada_vigencia son = 1
                            {
                                if (PermisoxUsu() == false)
                                {

                                }
                                else
                                {
                                    var ac = await Application.Current.MainPage.DisplayActionSheet("Atencion: El Articulo y/o su Adicional se encuentra Habilitado. ¿Desea Inhabilitarlo?", "Cancelar", null, "SI", "NO");
                                    if (ac == "NO")
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        if (txtUbiAdi != "") //Adicional <> ""
                                        {
                                            SqlCommand consul = new SqlCommand("update AdicionalxArtic set ada_vigencia = 0 where ada_codtex = '" + txtCodtex + "' and ada_codnum = " + txtCodnum + " and ada_adicional = '" + txtUbiAdi + "'", ConexionMaestra.con);
                                            consul.ExecuteNonQuery();
                                            if (txtArt_vigencia == Convert.ToString(1))
                                            {
                                                SqlCommand artVigencia = new SqlCommand("Select * from AdicionalxArtic where ada_codtex = '" + txtCodtex + "' and ada_codnum = " + txtCodnum + " and ada_vigencia = 1", ConexionMaestra.con);
                                                SqlDataReader artVig = artVigencia.ExecuteReader();
                                                if (artVig.Read())
                                                {

                                                }
                                                else
                                                {
                                                    SqlCommand art = new SqlCommand("update Articulo set art_vigencia = 2, art_usuest = " + txtUbi_usualta + ", art_fecest = '" + DateTime.Now.ToString("d") + "', art_horaest = '" + DateTime.Now.ToString("T") + "' where art_codtex = '" + txtCodtex + "' and art_codnum = " + txtCodnum + "", ConexionMaestra.con);
                                                    art.ExecuteNonQuery();
                                                }

                                            }
                                        }
                                        else // Adicional = ""
                                        {
                                            SqlCommand sqlquery = new SqlCommand("update Articulo set art_vigencia = 2, art_usuest = " + txtUbi_usualta + ", art_fecest = '" + DateTime.Now.ToString("d") + "', art_horaest = '" + DateTime.Now.ToString("T") + "' where art_codtex = '" + txtCodtex + "' and art_codnum = " + txtCodnum + "", ConexionMaestra.con);
                                            sqlquery.ExecuteNonQuery();
                                        }
                                        SqlCommand par_Actcarrito = new SqlCommand("select * from Parametro where par_ActCarritoxVigencia = 1", ConexionMaestra.con);
                                        SqlDataReader val = par_Actcarrito.ExecuteReader();
                                        if (val.Read())
                                        {
                                            SqlCommand carrito = new SqlCommand("update Articulo set art_carrito = 0 where art_codtex = '" + txtCodtex + "' and art_codnum = " + txtCodnum + "", ConexionMaestra.con);
                                            carrito.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            //Inserto el registro borrado en la tabla HistoUbicacion para saber que usuario elimino el registro
                            SqlCommand Hubi = new SqlCommand("Insert into HistoUbicacion (ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef," +
                                " ubi_observa, ubi_adicional, ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi,ubi_deposito, ubi_usubaja," +
                                " ubi_fecbaja, ubi_horabaja) Select ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef, ubi_observa, ubi_adicional," +
                                " ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi, ubi_deposito, 17, '" + DateTime.Now.ToString("d") + "', '" + DateTime.Now.ToString("T") + "' " +
                                "from Ubicacion where ubi_codtex = '" + h.ubi_codtex + "' and ubi_codnum = " + h.ubi_codnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
                            Hubi.ExecuteNonQuery();
                            SqlCommand sqlDelete = new SqlCommand("Delete from Ubicacion where ubi_codtex = '" + txtCodtex + "' and ubi_codnum = " + txtCodnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
                            int cant = sqlDelete.ExecuteNonQuery();
                            if (cant == 1)
                            {
                                await DisplayAlert("Mensaje", "La Ubicación ha sido Eliminada", "OK");
                                OnAppearing();
                            }
                        }
                        else
                        {
                            SqlCommand Hubi = new SqlCommand("Insert into HistoUbicacion (ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef," +
                                " ubi_observa, ubi_adicional, ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi,ubi_deposito, ubi_usubaja," +
                                " ubi_fecbaja, ubi_horabaja) Select ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef, ubi_observa, ubi_adicional," +
                                " ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi, ubi_deposito, 17, '" + DateTime.Now.ToString("d") + "', '" + DateTime.Now.ToString("T") + "' " +
                                "from Ubicacion where ubi_codtex = '" + h.ubi_codtex + "' and ubi_codnum = " + h.ubi_codnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
                            Hubi.ExecuteNonQuery();
                            SqlCommand sqlDelete = new SqlCommand("Delete from Ubicacion where ubi_codtex = '" + txtCodtex + "' and ubi_codnum = " + txtCodnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
                            int cant = sqlDelete.ExecuteNonQuery();
                            if (cant == 1)
                            {
                                await DisplayAlert("Mensaje", "La Ubicación ha sido Eliminada", "OK");
                                OnAppearing();
                            }
                        }
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