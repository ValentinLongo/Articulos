//using Acr.UserDialogs;
using Newtonsoft.Json;
using Plugin.DeviceInfo;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;
using Ubicacion_Articulos.Vistas;
using Xamarin.Forms;
using Xamarin.Essentials;
//using Intuit.Ipp.DataService;
using System.IO;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;

namespace Ubicacion_Articulos.VistaModelo
{
    public class VMUbicaciones : BaseViewModel
    {
        public ObservableCollection<MEditarUbicacion> _ubi;
        public ObservableCollection<MEditarUbicacion> ubi
        {
            get
            {
                return _ubi;
            }
            set
            {
                _ubi = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MUbicaciones> BuscarUbicacion(string aux)
        {
            ObservableCollection<MUbicaciones> ubica = new ObservableCollection<MUbicaciones>();
            string hostIMG = "http://190.123.89.13:70/";
            string sql = "SELECT top 50 art_codtex +'-'+ CONVERT(VARCHAR,art_codnum) AS Codigo, art_codtex, art_codfab, art_codinterno, art_codnum, "+
                        "(CASE WHEN Adicional.adi_codigo IS NULL THEN '' ELSE Adicional.adi_codigo END) AS adi_codigo, " +
                        "(CASE WHEN Adicional.adi_descri IS NULL THEN '' ELSE Adicional.adi_descri END) AS adi_descri, " +
                        "(CASE WHEN AdicionalxArtic.ada_adicional IS NULL THEN '' ELSE AdicionalxArtic.ada_adicional END) AS ada_adicional, " +
                        "ISNULL(art_descriWeb, art_descri) AS art_descri, ISNULL( ada_codbarra, art_codbarra ) AS CBarra, " +
                        "(CASE WHEN ada_codtex IS NULL THEN ISNULL(art_vigencia, 0) else ISNULL(ada_vigencia, 0) end) as Vigencia, " +
                        "art_vigencia,(case when ada_vigencia IS NULL then '' else ada_vigencia end) ada_vigencia, " +
                        "CASE WHEN ISNULL(ada_codnum, 0) = 0 Then REPLACE(art_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') " +
                        "Else REPLACE(ada_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') END AS imagen FROM Articulo " +
                        "LEFT JOIN AdicionalxArtic ON(art_codtex = AdicionalxArtic.ada_codtex AND art_codnum = AdicionalxArtic.ada_codnum AND " +
                        "AdicionalxArtic.ada_vigencia = 1) LEFT JOIN Adicional ON(AdicionalxArtic.ada_adicional = Adicional.adi_codigo) " +
                        "Left Join Ubicacion on ubi_codtex = art_codtex And ubi_codnum = art_codnum And IsNUll(ubi_adicional ,'') = IsNUll(ada_adicional, '') " +
                "Where "+aux+" "+
                "Group By art_codtex,art_codnum, art_codfab, art_codinterno, ISNULL(art_descriWeb, art_descri), art_pathfoto, art_codbarra, art_vigencia,  " +
                "ada_codtex, ada_codnum, ada_adicional, ada_codbarra, ada_vigencia, ada_pathfoto, adi_descri, adi_codigo";
            SqlCommand lista = new SqlCommand(sql, ConexionMaestra.con);
            SqlDataReader dr = lista.ExecuteReader();
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
                        Codigo = dr["Codigo"].ToString(),
                        art_descri = dr["art_descri"].ToString(),
                        imagen = dr["imagen"].ToString(),
                        adi_descri = dr["adi_descri"].ToString(),
                        CBarra = dr["CBarra"].ToString(),
                        Vigencia = Convert.ToInt32(dr["Vigencia"].ToString()),
                        art_vigencia = Convert.ToInt32(dr["art_vigencia"].ToString()),
                        art_codtex = dr["art_codtex"].ToString(),
                        art_codinterno = dr["art_codinterno"].ToString(),
                        art_codnum = Convert.ToInt32(dr["art_codnum"].ToString()),
                        adi_codigo = dr["ada_adicional"].ToString(),
                        ada_vigencia = Convert.ToInt32(dr["ada_vigencia"].ToString()),
                        art_codfab = dr["art_codfab"].ToString()
                    };
                    ubica.Add(art);
                }
            }
            return ubica;
        }
        public ObservableCollection<MUbicaciones> LlenarLista(string campo)
        {
            ObservableCollection<MUbicaciones> Ubicaciones = new ObservableCollection<MUbicaciones>();
            string hostIMG = "http://190.123.89.13:70/";
            string sqlQuery = "SELECT top 50 art_codtex +'-'+ CONVERT(VARCHAR,art_codnum) AS Codigo, art_codtex, art_codfab, art_codinterno, art_codnum, "+
                "(CASE WHEN Adicional.adi_codigo IS NULL THEN '' ELSE Adicional.adi_codigo END) AS adi_codigo, "+
                "(CASE WHEN Adicional.adi_descri IS NULL THEN '' ELSE Adicional.adi_descri END) AS adi_descri, "+
                "(CASE WHEN AdicionalxArtic.ada_adicional IS NULL THEN '' ELSE AdicionalxArtic.ada_adicional END) AS ada_adicional, "+
                 "ISNULL(art_descriWeb, art_descri) AS art_descri, ISNULL( ada_codbarra, art_codbarra ) AS CBarra, "+
                 "(CASE WHEN ada_codtex IS NULL THEN ISNULL(art_vigencia, 0) else ISNULL(ada_vigencia, 0) end) as Vigencia, " +
                 "art_vigencia,(case when ada_vigencia IS NULL then '' else ada_vigencia end) ada_vigencia, " +
                 "CASE WHEN ISNULL(ada_codnum, 0) = 0 Then REPLACE(art_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') " +
                "Else REPLACE(ada_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') END AS imagen FROM Articulo " +
                 "LEFT JOIN AdicionalxArtic ON(art_codtex = AdicionalxArtic.ada_codtex AND art_codnum = AdicionalxArtic.ada_codnum AND " +
                "AdicionalxArtic.ada_vigencia = 1) LEFT JOIN Adicional ON(AdicionalxArtic.ada_adicional = Adicional.adi_codigo) WHERE art_codnum<> -1 and " + campo + "";
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
                        Codigo = dr["Codigo"].ToString(),
                        art_descri = dr["art_descri"].ToString(),
                        imagen = dr["imagen"].ToString(),
                        adi_descri = dr["adi_descri"].ToString(),
                        CBarra = dr["CBarra"].ToString(),
                        Vigencia = Convert.ToInt32(dr["Vigencia"].ToString()),
                        art_vigencia = Convert.ToInt32(dr["art_vigencia"].ToString()),
                        art_codtex = dr["art_codtex"].ToString(),
                        art_codinterno = dr["art_codinterno"].ToString(),
                        art_codnum = Convert.ToInt32(dr["art_codnum"].ToString()),
                        adi_codigo = dr["ada_adicional"].ToString(),
                        ada_vigencia = Convert.ToInt32(dr["ada_vigencia"].ToString()),
                        art_codfab = dr["art_codfab"].ToString()
                    };
                    Ubicaciones.Add(art);
                    var serializer = JsonConvert.SerializeObject(Ubicaciones[0]);
                    if (Ubicaciones != null && Ubicaciones.Count > 0)
                    {
                        Preferences.Set("datoUbi", serializer);
                    }
                }
            }
            return Ubicaciones;
        }

        public ObservableCollection<MEditarUbicacion> traerUbicacion(string campo)
        {
            ubi = new ObservableCollection<MEditarUbicacion>();
            string query = "select (ubi_ubica1 + '-' + ubi_ubica2 +'-'+ ubi_ubica3 +'-'+ ubi_ubica4) as ubicacion, * from Ubicacion left join Deposito on ubi_deposito = dep_codigo where " + campo + "";
            SqlCommand sqlUbica = new SqlCommand(query, ConexionMaestra.con);
            SqlDataReader dr = sqlUbica.ExecuteReader();
            while (dr.Read())
            {
                MEditarUbicacion x = new MEditarUbicacion()
                {
                    ubicacion = dr["ubicacion"].ToString(),
                    deposito = dr["dep_descri"].ToString(),
                    ubi_codtex = dr["ubi_codtex"].ToString(),
                    ubi_adicional = dr["ubi_adicional"].ToString(),
                    ubi_codnum = Convert.ToInt32(dr["ubi_codnum"].ToString()),
                    ubi_codigo = Convert.ToInt32(dr["ubi_codigo"].ToString()),
                    ubi_deposito = Convert.ToInt32(dr["ubi_deposito"].ToString()),
                    ubi_predef = Convert.ToInt32(dr["ubi_predef"].ToString()),
                    ubi_ubica1 = dr["ubi_ubica1"].ToString(),
                    ubi_ubica2 = dr["ubi_ubica2"].ToString(),
                    ubi_ubica3 = dr["ubi_ubica3"].ToString(),
                    ubi_ubica4 = dr["ubi_ubica4"].ToString()
                    
                };
                ubi.Add(x);
                var serializer = JsonConvert.SerializeObject(ubi[0]);
                if (ubi !=null && ubi.Count > 0)
                {
                    Preferences.Set("dato", serializer);
                }
            }
            return ubi;
        }
        public ObservableCollection<MDeposito> SelecDepo()
        {
            ObservableCollection<MDeposito> deposito = new ObservableCollection<MDeposito>();
            SqlCommand pic = new SqlCommand("select dep_codigo, dep_descri from Deposito order by dep_descri asc", ConexionMaestra.con);
            SqlDataReader dr = pic.ExecuteReader();
            while (dr.Read())
            {
                MDeposito depo = new MDeposito()
                {
                    dep_codigo = Convert.ToInt32(dr["dep_codigo"].ToString()),
                    dep_descri = dr["dep_descri"].ToString()
                };
                deposito.Add(depo);
            }
            return deposito;
        }

        public ObservableCollection<MEtiquetas> ImpEtiqueta(int campo)
        {
            ObservableCollection<MEtiquetas> etiqueta = new ObservableCollection<MEtiquetas>();
            SqlCommand eti = new SqlCommand("Select imp_codtex +'-'+ CONVERT(VARCHAR,imp_codnum) AS Codigo, * from AuxApp_ImpEtiqueta where imp_terminal = "+campo+" and imp_tipo = 1", ConexionMaestra.con);
            SqlDataReader DReti = eti.ExecuteReader();
            while (DReti.Read())
            {
                MEtiquetas a = new MEtiquetas()
                {
                    imp_codtex = DReti["imp_codtex"].ToString(),
                    imp_codnum = Convert.ToInt32(DReti["imp_codnum"].ToString()),
                    Codigo = DReti["Codigo"].ToString(),
                    imp_adicional = DReti["imp_adicional"].ToString(),
                    imp_orden = Convert.ToInt32(DReti["imp_orden"].ToString()),
                    imp_usuario = Convert.ToInt32(DReti["imp_usuario"].ToString()),
                    imp_descri = DReti["imp_descri"].ToString(),
                    imp_terminal = Convert.ToInt32(DReti["imp_terminal"].ToString()),
                    imp_cantimp = Convert.ToInt32(DReti["imp_cantimp"].ToString())
                };
                etiqueta.Add(a);
                var serializer = JsonConvert.SerializeObject(etiqueta[0]);
                if (etiqueta != null && etiqueta.Count > 0)
                {
                    Preferences.Set("etiqueta", serializer);
                }
            }
            return etiqueta;
        }
        public ObservableCollection<MEtiquetas> ImpUbicacion(int campo)
        {
            ObservableCollection<MEtiquetas> etiqueta = new ObservableCollection<MEtiquetas>();
            SqlCommand eti = new SqlCommand("Select imp_codtex +'-'+ CONVERT(VARCHAR,imp_codnum) AS Codigo, * from AuxApp_ImpEtiqueta where imp_terminal = " + campo + " AND imp_tipo = 2", ConexionMaestra.con);
            SqlDataReader DReti = eti.ExecuteReader();
            while (DReti.Read())
            {
                MEtiquetas a = new MEtiquetas()
                {
                    imp_codtex = DReti["imp_codtex"].ToString(),
                    imp_codnum = Convert.ToInt32(DReti["imp_codnum"].ToString()),
                    Codigo = DReti["Codigo"].ToString(),
                    imp_adicional = DReti["imp_adicional"].ToString(),
                    imp_orden = Convert.ToInt32(DReti["imp_orden"].ToString()),
                    imp_usuario = Convert.ToInt32(DReti["imp_usuario"].ToString()),
                    imp_descri = DReti["imp_descri"].ToString(),
                    imp_terminal = Convert.ToInt32(DReti["imp_terminal"].ToString()),
                    imp_cantimp = Convert.ToInt32(DReti["imp_cantimp"].ToString())
                };
                etiqueta.Add(a);
                var serializer = JsonConvert.SerializeObject(etiqueta[0]);
                if (etiqueta != null && etiqueta.Count > 0)
                {
                    Preferences.Set("etiqueta", serializer);
                }
            }
            return etiqueta;
        }
        public ICommand btnEditar { get; set; }
        public ICommand btnEliminar { get; set; }
        public ICommand btnAgEtiq { get; set; }
        public ICommand btnElimEtiq { get; set; }
        public ICommand RefreshCommand { private set; get; }
        public ICommand btnAgUbic { get; set; }
        private bool _isRerfeshing;
        public bool IsRefreshing
        {
            get { return _isRerfeshing; }
            set
            {
                _isRerfeshing = value;
                OnPropertyChanged();
            }
        }

        public void Listar()
        {
            var auxiliar = Preferences.Get("aux", "");
            var deserialize = JsonConvert.DeserializeObject<string>(auxiliar);
            traerUbicacion(deserialize);
        }

        public VMUbicaciones()
        {
            //btnEditar = new Command(BtnEditar);
            //btnEliminar = new Command(BtnEliminar);
            btnAgEtiq = new Command(BtnAgEtiq);
            btnElimEtiq = new Command(BtnElimEtiq);
            btnAgUbic = new Command(BtnAgUbic);
            ValidarConexionInternet();
        }

        private async void BtnAgUbic(object obj)
        {
            var datados = Preferences.Get("datoUbi", "");
            var deserializedos = JsonConvert.DeserializeObject<MUbicaciones>(datados);
            var descripcionArt = deserializedos?.art_descri;
            var data = Preferences.Get("data", "");
            var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
            var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
            var etiq = obj as MEditarUbicacion;
            SqlCommand etiqueta = new SqlCommand("Select count(*) from AuxApp_ImpEtiqueta where imp_codtex = '" + etiq.ubi_codtex + "' and imp_codnum = " + etiq.ubi_codnum + " and imp_adicional = '" + etiq.ubi_adicional + "' and imp_terminal = " + name + " and imp_tipo = 2", ConexionMaestra.con);
            int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
            if (dr == 0)
            {
                var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea incluir este articulo en la impresion?", "Cancelar", null, "SI", "NO");
                if (accion == "SI")
                {
                    SqlCommand numorden = new SqlCommand("Select ISNULL(MAX(imp_orden),0) from AuxApp_ImpEtiqueta where imp_terminal = " + name + " and imp_tipo = 2", ConexionMaestra.con);
                    int ordenmax = Convert.ToInt32(numorden.ExecuteScalar());
                    int ultOrden = ordenmax + 10;
                    SqlCommand insert = new SqlCommand("Insert into AuxApp_ImpEtiqueta (imp_codtex, imp_codnum, imp_adicional, imp_orden, imp_usuario, imp_descri, imp_terminal, imp_cantimp, imp_tipo) values ('" + etiq.ubi_codtex + "', " + etiq.ubi_codnum + ", '" + etiq.ubi_adicional + "', " + ultOrden + ", " + usu_codigo + ", '" + descripcionArt + "'," + name + ", 1, 2)", ConexionMaestra.con);
                    insert.ExecuteNonQuery();
                    await DisplayAlert("Mensaje", "Se ha guardado correctamente", "OK");
                }
            }
            else
            {
                await DisplayAlert("Mensaje", "El articulo ya se encuentra en la lista de impresion", "OK");
            }
        }

        public Command RefreshCommad
        {
            get
            {
                return new Command(() =>
                {
                    var auxiliar = Preferences.Get("aux", "");
                    var deserialize = JsonConvert.DeserializeObject<string>(auxiliar);
                    traerUbicacion(deserialize);
                    IsRefreshing = false;
                });
            }
            
        }

        private async void BtnElimEtiq(object obj)
        {
            //var data = Preferences.Get("data", "");
            //var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
            //var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
            //var etiq = obj as MEtiquetas;
            //SqlCommand etiqueta = new SqlCommand("Select imp_cantimp from AuxApp_ImpEtiqueta where imp_codtex = '" + etiq.imp_codtex + "' and imp_codnum = " + etiq.imp_codnum + " and imp_adicional = '" + etiq.imp_adicional + "' and imp_terminal = "+name+" and imp_tipo = 1", ConexionMaestra.con);
            //int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
            //if (dr == 1)
            //{
            //    var z = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una etiqueta de este articulo?", null, "Cancelar", "SI", "NO");
            //    if (z=="SI")
            //    {
            //        SqlCommand insert = new SqlCommand("Delete from AuxApp_ImpEtiqueta where imp_codtex = '" + etiq.imp_codtex + "' and imp_codnum = " + etiq.imp_codnum + " and imp_adicional = '" + etiq.imp_adicional + "' and imp_terminal = " + name + " and imp_tipo = 1", ConexionMaestra.con);
            //        insert.ExecuteNonQuery();
            //        await DisplayAlert("Mensaje", "Elimino la última etiqueta en la cola de impresión", "OK");
            //        await PopupNavigation.Instance.PopAsync();
            //        await PopupNavigation.Instance.PushAsync(new ListaEtiquetas());
            //        return;
            //    }
            //    else
            //    {

            //    }
            //}
            //if (dr > 0)
            //{
            //    var z = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una etiqueta de este articulo?", null, "Cancelar", "SI", "NO");
            //    if (z == "SI")
            //    {
            //        int cont = dr;
            //        cont -= 1;
            //        SqlCommand update = new SqlCommand("Update AuxApp_ImpEtiqueta set imp_cantimp = " + cont + " where imp_codtex = '" + etiq.imp_codtex + "' and imp_codnum = " + etiq.imp_codnum + " and imp_adicional = '" + etiq.imp_adicional + "' and imp_terminal = " + name + " and imp_tipo = 1", ConexionMaestra.con);
            //        update.ExecuteNonQuery();
            //        await DisplayAlert("Aclaracion", "Quedan " + cont + " etiquetas", "OK");
            //        await PopupNavigation.Instance.PopAsync();
            //        await PopupNavigation.Instance.PushAsync(new ListaEtiquetas());
            //        return;
            //    }
            //    else
            //    {

            //    }
            //}
        }

        string deviceID = CrossDeviceInfo.Current.Id;
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
        public static int name = Convert.ToInt32(File.ReadAllText(ruta));
        private async void BtnAgEtiq(object obj)
        {
            try
            {
                var data = Preferences.Get("data", "");
                var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
                var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
                var etiq = obj as MUbicaciones;
                SqlCommand etiqueta = new SqlCommand("Select count(*) from AuxApp_ImpEtiqueta where imp_codtex = '" + etiq.art_codtex + "' and imp_codnum = " + etiq.art_codnum + " and imp_adicional = '" + etiq.adi_codigo + "' and imp_terminal = " + name + " and imp_tipo = 1", ConexionMaestra.con);
                int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
                if (dr == 0)
                {
                    SqlCommand numorden = new SqlCommand("Select ISNULL(MAX(imp_orden),0) from AuxApp_ImpEtiqueta where imp_terminal = " + name + " and imp_tipo = 1", ConexionMaestra.con);
                    int ordenmax = Convert.ToInt32(numorden.ExecuteScalar());
                    int ultOrden = ordenmax + 10;
                    var x = await Application.Current.MainPage.DisplayPromptAsync("Mensaje", "Ingrese la cantidad de etiquetas a imprimir", "OK", "Cancelar", null, 5, Keyboard.Numeric, "1");
                    if (x != Convert.ToString(0) || x != null)
                    {
                        SqlCommand insert = new SqlCommand("Insert into AuxApp_ImpEtiqueta (imp_codtex, imp_codnum, imp_adicional, imp_orden, imp_usuario, imp_descri, imp_terminal, imp_cantimp, imp_tipo) values ('" + etiq.art_codtex + "', " + etiq.art_codnum + ", '" + etiq.adi_codigo + "', " + ultOrden + ", " + usu_codigo + ", '" + etiq.art_descri + "'," + name + ", " + x + ", 1)", ConexionMaestra.con);
                        insert.ExecuteNonQuery();
                        await DisplayAlert("Mensaje", "Se han agregado " + x + " etiqueta/s a la cola de impresión", "OK");
                    }
                    else if (x == null || x== Convert.ToString(0))
                    {
                        await DisplayAlert("Advertencia", "No puede agregar valores nulos", "OK");
                        return;
                    }
                }
                if (dr > 0)
                {
                    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea modificar la cantidad de etiquetas de este articulo?", null, "Cancelar", "SI", "NO");
                    if (accion == "SI")
                    {
                        SqlCommand query = new SqlCommand("Select imp_cantimp from AuxApp_ImpEtiqueta where imp_codtex = '" + etiq.art_codtex + "' and imp_codnum = " + etiq.art_codnum + " and imp_adicional = '" + etiq.adi_codigo + "' and imp_terminal = " + name + " and imp_tipo = 1", ConexionMaestra.con);
                        int cont = Convert.ToInt32(query.ExecuteScalar());
                        var x = await Application.Current.MainPage.DisplayPromptAsync("Mensaje", "Ingrese la cantidad de etiquetas a imprimir", "OK", "Cancelar", null, 5, null, "" + cont + "");
                        SqlCommand update = new SqlCommand("Update AuxApp_ImpEtiqueta set imp_cantimp = " + x + " where imp_codtex = '" + etiq.art_codtex + "' and imp_codnum = " + etiq.art_codnum + " and imp_adicional = '" + etiq.adi_codigo + "' and imp_terminal = " + name + " and imp_tipo = 1", ConexionMaestra.con);
                        update.ExecuteNonQuery();
                        await DisplayAlert("Mensaje", "Cantidad modificada con éxito", "OK");
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        //private async void BtnEliminar(object obj) //Elimino la ubicacion de tal articulo
        //{
        //    var h = obj as MEditarUbicacion;
        //    SqlCommand cmd = new SqlCommand("select count(ubi_ubica1) from Ubicacion where ubi_codtex = '" + h.ubi_codtex + "' and ubi_codnum = " + h.ubi_codnum + " and ubi_adicional = '" + h.ubi_adicional + "'", ConexionMaestra.con);
        //    var sql = cmd.ExecuteScalar();
        //    int intsql = (int)sql;
        //    if (intsql > 1) //Si dicho articulo tiene mas de una ubicacion 
        //    {
        //        if (h.ubi_predef == 1) // Si la ubicacion que quiero eliminar es la predefinida
        //        {
        //            await DisplayAlert("Advertencia", "Deberá Predefinir otra Ubicación antes de Eliminar ésta.", "OK");
        //            return;
        //        }
        //    }
            
        //    bool ubi = Preferences.ContainsKey("datoUbi");
        //    if (ubi)
        //    {
        //        var datoUbi = Preferences.Get("datoUbi", "");
        //        var deserializer = JsonConvert.DeserializeObject<MUbicaciones>(datoUbi);
        //        var txtada_vigencia = Convert.ToString(deserializer?.ada_vigencia);
        //        var txtArt_vigencia = Convert.ToString(deserializer?.art_vigencia);
        //        var txtCodtex = deserializer?.art_codtex;
        //        var txtCodnum = Convert.ToString(deserializer?.art_codnum);
        //        var txtUbiAdi = deserializer?.adi_codigo;
        //        var txtVigencia = Convert.ToString(deserializer?.Vigencia);

        //        var data = Preferences.Get("data", "");
        //        var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
        //        var txtUbi_usualta = Convert.ToString(deserialize?.usu_codigo);

        //        var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar el registro?", "Cancelar", null, "SI", "NO");
        //        if (accion == "SI")
        //        {
        //            if (intsql == 1) //Si el articulo tiene una unica ubicacion 
        //            {
        //                if (txtVigencia == Convert.ToString(1)) // art_vigencia y ada_vigencia son = 1
        //                {
        //                    if (PermisoxUsu() == false)
        //                    {
                                
        //                    }
        //                    else
        //                    {
        //                        var ac = await Application.Current.MainPage.DisplayActionSheet("Atencion: El Articulo y/o su Adicional se encuentra Habilitado. ¿Desea Inhabilitarlo?", "Cancelar", null, "SI", "NO");
        //                        if (ac == "NO")
        //                        {
        //                            return;
        //                        }
        //                        else
        //                        {
        //                            if (txtUbiAdi != "") //Adicional <> ""
        //                            {
        //                                SqlCommand consul = new SqlCommand("update AdicionalxArtic set ada_vigencia = 0 where ada_codtex = '" + txtCodtex + "' and ada_codnum = " + txtCodnum + " and ada_adicional = '" + txtUbiAdi + "'", ConexionMaestra.con);
        //                                consul.ExecuteNonQuery();
        //                                if (txtArt_vigencia == Convert.ToString(1))
        //                                {
        //                                    SqlCommand artVigencia = new SqlCommand("Select * from AdicionalxArtic where ada_codtex = '" + txtCodtex + "' and ada_codnum = " + txtCodnum + " and ada_vigencia = 1", ConexionMaestra.con);
        //                                    SqlDataReader artVig = artVigencia.ExecuteReader();
        //                                    if (artVig.Read())
        //                                    {

        //                                    }
        //                                    else
        //                                    {
        //                                        SqlCommand art = new SqlCommand("update Articulo set art_vigencia = 2, art_usuest = " + txtUbi_usualta + ", art_fecest = '" + DateTime.Now.ToString("d") + "', art_horaest = '" + DateTime.Now.ToString("T") + "' where art_codtex = '" + txtCodtex + "' and art_codnum = " + txtCodnum + "", ConexionMaestra.con);
        //                                        art.ExecuteNonQuery();
        //                                    }

        //                                }
        //                            }
        //                            else // Adicional = ""
        //                            {
        //                                SqlCommand sqlquery = new SqlCommand("update Articulo set art_vigencia = 2, art_usuest = " + txtUbi_usualta + ", art_fecest = '" + DateTime.Now.ToString("d") + "', art_horaest = '" + DateTime.Now.ToString("T") + "' where art_codtex = '" + txtCodtex + "' and art_codnum = " + txtCodnum + "", ConexionMaestra.con);
        //                                sqlquery.ExecuteNonQuery();
        //                            }
        //                            SqlCommand par_Actcarrito = new SqlCommand("select * from Parametro where par_ActCarritoxVigencia = 1", ConexionMaestra.con);
        //                            SqlDataReader val = par_Actcarrito.ExecuteReader();
        //                            if (val.Read())
        //                            {
        //                                SqlCommand carrito = new SqlCommand("update Articulo set art_carrito = 0 where art_codtex = '" + txtCodtex + "' and art_codnum = " + txtCodnum + "", ConexionMaestra.con);
        //                                carrito.ExecuteNonQuery();
        //                            }
        //                        }
        //                    }
        //                }
        //                //Inserto el registro borrado en la tabla HistoUbicacion para saber que usuario elimino el registro
        //                SqlCommand Hubi = new SqlCommand("Insert into HistoUbicacion (ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef," +
        //                    " ubi_observa, ubi_adicional, ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi,ubi_deposito, ubi_usubaja," +
        //                    " ubi_fecbaja, ubi_horabaja) Select ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef, ubi_observa, ubi_adicional," +
        //                    " ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi, ubi_deposito, 17, '" + DateTime.Now.ToString("d") + "', '" + DateTime.Now.ToString("T") + "' " +
        //                    "from Ubicacion where ubi_codtex = '" + h.ubi_codtex + "' and ubi_codnum = " + h.ubi_codnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
        //                Hubi.ExecuteNonQuery();
        //                SqlCommand sqlDelete = new SqlCommand("Delete from Ubicacion where ubi_codtex = '" + txtCodtex + "' and ubi_codnum = " + txtCodnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
        //                int cant = sqlDelete.ExecuteNonQuery();
        //                if (cant == 1)
        //                {
        //                    await DisplayAlert("Mensaje", "La Ubicación ha sido Eliminada", "OK");
        //                }
        //            }
        //            else
        //            {
        //                SqlCommand Hubi = new SqlCommand("Insert into HistoUbicacion (ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef," +
        //                    " ubi_observa, ubi_adicional, ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi,ubi_deposito, ubi_usubaja," +
        //                    " ubi_fecbaja, ubi_horabaja) Select ubi_codigo, ubi_codtex, ubi_codnum, ubi_ubica1, ubi_ubica2, ubi_ubica3, ubi_ubica4, ubi_predef, ubi_observa, ubi_adicional," +
        //                    " ubi_incluyeprepped, ubi_usualta, ubi_fecalta, ubi_horalta, ubi_usumodi, ubi_fecmodi, ubi_horamodi, ubi_deposito, 17, '" + DateTime.Now.ToString("d") + "', '" + DateTime.Now.ToString("T") + "' " +
        //                    "from Ubicacion where ubi_codtex = '" + h.ubi_codtex + "' and ubi_codnum = " + h.ubi_codnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
        //                Hubi.ExecuteNonQuery();
        //                SqlCommand sqlDelete = new SqlCommand("Delete from Ubicacion where ubi_codtex = '" + txtCodtex + "' and ubi_codnum = " + txtCodnum + " and ubi_adicional = '" + txtUbiAdi + "' and ubi_codigo = " + h.ubi_codigo + "", ConexionMaestra.con);
        //                int cant = sqlDelete.ExecuteNonQuery();
        //                if (cant == 1)
        //                {
        //                    await DisplayAlert("Mensaje", "La Ubicación ha sido Eliminada", "OK");
        //                }
        //            }
        //        }
        //    }
        //}           
        //private void BtnEditar(object obj)
        //{
        //    try
        //    {
        //        var h = obj as MEditarUbicacion;
        //        Application.Current.MainPage.Navigation.PushAsync(new NavigationPage(new EditarUbicacion(h)));
        //    }
        //    catch 
        //    {
                
        //    }

        //}

    }
}