using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;
using Ubicacion_Articulos.Vistas;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ubicacion_Articulos.VistaModelo
{
    public class VMFaltantes : BaseViewModel
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
            string sql = "SELECT top 50 art_codtex +'-'+ CONVERT(VARCHAR,art_codnum) AS Codigo, art_codtex, art_codfab, art_codinterno, art_codnum, " +
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
                "Where " + aux + " " +
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
        public ObservableCollection<MFaltantes> LlenarLista(string campo)
        {
            ObservableCollection<MFaltantes> Faltantes = new ObservableCollection<MFaltantes>();
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
                    MFaltantes art = new MFaltantes()
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
                    Faltantes.Add(art);
                    var serializer = JsonConvert.SerializeObject(Faltantes[0]);
                    if (Faltantes != null && Faltantes.Count > 0)
                    {
                        Preferences.Set("datoUbi", serializer);
                    }
                }

            }
            return Faltantes;
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
                if (ubi != null && ubi.Count > 0)
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

        public ObservableCollection<MStock> mStocks(string aux)
        {
            ObservableCollection<MStock> stock = new ObservableCollection<MStock>();
            SqlCommand cmd = new SqlCommand("Select det_codtex +'-'+ CONVERT(VARCHAR,det_codnum) AS Codigo, * from DetalleFaltanteApp where "+aux+"", ConexionMaestra.con);
            SqlDataReader st = cmd.ExecuteReader();
            while (st.Read())
            {
                MStock s = new MStock()
                {
                    aux_codtex = st["det_codtex"].ToString(),
                    aux_codnum = Convert.ToInt32(st["det_codnum"].ToString()),
                    aux_descri = st["det_descri"].ToString(),
                    aux_adicional = st["det_adicional"].ToString(),
                    aux_usuario = Convert.ToInt32(st["det_usuario"].ToString()),
                    aux_cantidad = Convert.ToInt32(st["det_cantidad"].ToString()),
                    codigo = st["Codigo"].ToString(),
                    aux_detCodigo = Convert.ToInt32(st["det_codigo"].ToString())
                };
                stock.Add(s);
                var serializer = JsonConvert.SerializeObject(stock[0]);
                if (stock != null && stock.Count > 0)
                {
                    Preferences.Set("stock", serializer);
                }
            }
            return stock;
        }

        public ObservableCollection<MEstadoFaltante> Faltantes(string aux)
        {
            var fecha = "";
            var dato = Preferences.Get("EstadoPicker", "");
            var deserializer = JsonConvert.DeserializeObject<string>(dato);
            if (deserializer == "FINALIZADOS")
            {
                fecha = ", fal_horafin, (RIGHT('00' + CONVERT(VARCHAR,DAY(fal_fecfin)),2) + '/' + RIGHT('00' + CONVERT(VARCHAR,MONTH(fal_fecfin)),2) + '/' + RIGHT('0000' + CONVERT(VARCHAR,YEAR(fal_fecfin)),4)) as fal_fecfin";
            }
            else
            {
                fecha = ", '' as fal_horafin, '' as fal_fecfin";
            }
            ObservableCollection<MEstadoFaltante> faltantes = new ObservableCollection<MEstadoFaltante>();
            SqlCommand faltante = new SqlCommand("Select fal_id, fal_idusu, fal_terminado, fal_estado, fal_comentario, fal_fecini, fal_horaini"+fecha+" from FaltantesApp where " + aux+"", ConexionMaestra.con);
            SqlDataReader dr = faltante.ExecuteReader();
            while (dr.Read())
            {
                MEstadoFaltante est = new MEstadoFaltante()
                {
                    idUser = Convert.ToInt32(dr["fal_idusu"].ToString()),
                    //idDeposito = Convert.ToInt32(dr["fal_deposito"].ToString()),
                    comentario = dr["fal_comentario"].ToString(),
                    terminado = Convert.ToInt32(dr["fal_terminado"].ToString()),
                    Estado = dr["fal_estado"].ToString(),
                    DateTime = Convert.ToDateTime(dr["fal_horaini"].ToString()),
                    horaFin = dr["fal_horafin"].ToString(),
                    fechaFin = dr["fal_fecfin"].ToString(),
                    idFaltantes = Convert.ToInt32(dr["fal_id"].ToString())
                };
                faltantes.Add(est);
                var serializer = JsonConvert.SerializeObject(faltantes[0]);
                if (faltantes != null)
                {
                    Preferences.Set("Faltante", serializer);
                }
            }
            return faltantes;
        }

        public ObservableCollection<MDetFaltantes> DetFaltantes(string cad)
        {
            ObservableCollection<MDetFaltantes> Detfaltantes = new ObservableCollection<MDetFaltantes>();
            string hostIMG = "http://190.123.89.13:70/";
            SqlCommand faltante = new SqlCommand("select det_codtex +'-'+ CONVERT(VARCHAR,det_codnum) AS Codigo, det_codigo, det_codtex, det_codnum, det_descri, det_adicional, det_usuario, det_fecing, det_horaing, det_cantidad, det_check, CASE WHEN ISNULL(ada_codnum,0) = 0 Then REPLACE(art_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') Else REPLACE(ada_pathfoto, 'Z:\\SISTEMAS\\CORONEL\\FOTOS\','" + hostIMG + "') END AS imagen FROM DetalleFaltanteApp LEFT JOIN AdicionalxArtic ON(det_codtex = AdicionalxArtic.ada_codtex AND det_codnum = AdicionalxArtic.ada_codnum AND det_adicional = AdicionalxArtic.ada_adicional and AdicionalxArtic.ada_vigencia = 1) LEFT JOIN Adicional ON(AdicionalxArtic.ada_adicional = Adicional.adi_codigo) left join Articulo ON(det_codtex = art_codtex and det_codnum = art_codnum) where " + cad + "", ConexionMaestra.con);
            SqlDataReader dr = faltante.ExecuteReader();
            while (dr.Read())
            {
                MDetFaltantes est = new MDetFaltantes()
                {
                    codigo = dr["Codigo"].ToString(),
                    det_codigo = Convert.ToInt32(dr["det_codigo"].ToString()),
                    det_codtex = dr["det_codtex"].ToString(),
                    det_codnum = Convert.ToInt32(dr["det_codnum"].ToString()),
                    det_descri = dr["det_descri"].ToString(),
                    det_adicional = dr["det_adicional"].ToString(),
                    det_cantidad = Convert.ToInt32(dr["det_cantidad"].ToString()),
                    det_usuario = Convert.ToInt32(dr["det_usuario"].ToString()),
                    imagen = dr["imagen"].ToString(),
                    det_check = Convert.ToInt32(dr["det_check"].ToString())
                };
                Detfaltantes.Add(est);
                var serializer = JsonConvert.SerializeObject(Detfaltantes[0]);
                if (Detfaltantes != null)
                {
                    Preferences.Set("DetFaltante", serializer);
                }
            }
            return Detfaltantes;
        }

        public ICommand btnAgStock { get; set; }
        public ICommand btnElimStock { get; set; }
        public ICommand btnEstadoFaltante { get; set; }
        public ICommand btnImgDetFal { get; set; }
        public ICommand RefreshCommand { private set; get; }
        public ICommand agregarStock { get; set; }
        public ICommand btnEliminarFaltante { get; set; }
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

        public VMFaltantes()
        {
            btnAgStock = new Command(BtnAgStock);
            btnElimStock = new Command(BtnElimStock);
            RefreshCommand = new Command(async () => await Refresh());
            //btnEstadoFaltante = new Command(BtnEstadoFaltante);
            btnImgDetFal = new Command(BtnImgDetFal);
            agregarStock = new Command(AgStock);
            //btnEliminarFaltante = new Command(BtnEliminarFal);
            ValidarConexionInternet();
        }

        //private async void BtnEliminarFal(object obj)
        //{
        //    var data = Preferences.Get("data", "");
        //    var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
        //    var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
        //    var faltante = obj as MDetFaltantes;
        //    SqlCommand etiqueta = new SqlCommand("Select det_cantidad from DetalleFaltanteApp where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
        //    int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
        //    if (dr == 1)
        //    {
        //        var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
        //        if (accion == "SI")
        //        {
        //            SqlCommand insert = new SqlCommand("Delete from DetalleFaltanteApp where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
        //            insert.ExecuteNonQuery();
        //            await DisplayAlert("Mensaje", "Ha eliminado el articulo del stock", "OK");
        //            return;
        //        }
        //        else
        //        {

        //        }
        //    }
        //    if (dr > 0)
        //    {
        //        var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
        //        if (accion == "SI")
        //        {
        //            int cont = dr;
        //            cont -= 1;
        //            SqlCommand update = new SqlCommand("Update DetalleFaltanteApp set det_cantidad = " + cont + " where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
        //            update.ExecuteNonQuery();
        //            await DisplayAlert("Aclaracion", "Quedan " + cont + " etiquetas", "OK");
        //        }
        //        else
        //        {

        //        }
        //    }
        //}

        private async void AgStock(object obj)//Agrega articulos al faltante ya realizado
        {
            var dato = Preferences.Get("DetFaltante", "");
            var deserializer = JsonConvert.DeserializeObject<MDetFaltantes>(dato);
            var usu_codigo = deserializer?.det_usuario;
            var id_faltante = deserializer?.det_codigo;
            var f = obj as MFaltantes;

            SqlCommand stock = new SqlCommand("Select count(*) from DetalleFaltanteApp where det_codtex = '" + f.art_codtex + "' and det_codnum = " + f.art_codnum + " and det_adicional = '" + f.adi_codigo + "' and det_codigo = "+ id_faltante +"", ConexionMaestra.con);
            var dr = Convert.ToInt32(stock.ExecuteScalar());
            if (dr == 0)
            {
                var x = await Application.Current.MainPage.DisplayPromptAsync("Mensaje", "Cantidad de stock", "OK", "Cancelar", null, 5, Keyboard.Numeric , "1");
                if (x != Convert.ToString(0) && x != null)
                {
                    SqlCommand insert = new SqlCommand("Insert into DetalleFaltanteApp (det_codigo,det_codtex,det_codnum,det_descri,det_adicional,det_usuario,det_fecing,det_horaing,det_cantidad,det_check) values " +
                        "(" + id_faltante + ", '" + f.art_codtex + "', " + f.art_codnum + ", '" + f.art_descri + "', '" + f.adi_codigo + "', " + usu_codigo + ",'" + DateTime.Now.ToString("d") + "','" + DateTime.Now.ToString("T") + "', " + x + ", 0)", ConexionMaestra.con);
                    insert.ExecuteNonQuery();
                    await DisplayAlert("Mensaje", "Se han agregado " + x + " articulos", "OK");
                }
                else if (x == null)
                {
                    return;
                }
            }
            if (dr > 0)
            {
                var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea modificar la cantidad?", null, "Cancelar", "SI", "NO");
                if (accion == "SI")
                {
                    SqlCommand query = new SqlCommand("Select det_cantidad from DetalleFaltanteApp where det_codtex = '" + f.art_codtex + "' and det_codnum = " + f.art_codnum + " and det_adicional = '" + f.adi_codigo + "' and det_codigo = " + id_faltante + "", ConexionMaestra.con);
                    int cont = Convert.ToInt32(query.ExecuteScalar());
                    var x = await Application.Current.MainPage.DisplayPromptAsync("Mensaje", "Ingrese la cantidad", "OK", "Cancelar", null, 5, null, "" + cont + "");
                    SqlCommand update = new SqlCommand("Update DetalleFaltanteApp set det_cantidad = " + x + " where det_codtex = '" + f.art_codtex + "' and det_codnum = " + f.art_codnum + " and det_adicional = '" + f.adi_codigo + "' and det_codigo = " + id_faltante + "", ConexionMaestra.con);
                    update.ExecuteNonQuery();
                }
                else
                {
                    return;
                }
            }
        }

        private void BtnImgDetFal(object obj)
        {
            var img = obj as MDetFaltantes;
            PopupNavigation.Instance.PushAsync(new PopImagenFaltante(img));
        }

        //private async void BtnEstadoFaltante(object obj)
        //{
        //    try
        //    {
        //        var dato = Preferences.Get("Faltante", "");
        //        var deserializer = JsonConvert.DeserializeObject<MEstadoFaltante>(dato);
        //        var EstadoFaltante = deserializer?.Estado;
        //        var item = obj as MEstadoFaltante;
        //        if (item.Estado == "R")
        //        {
        //            var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea finalizar éste faltante?", null, "Cancelar", "SI", "NO");
        //            if (accion == "SI")
        //            {
        //                SqlCommand update = new SqlCommand("Update FaltantesApp set fal_estado = 'F', fal_fecfin = '" + DateTime.Now.ToString("d") + "', fal_horafin = '" + DateTime.Now.ToString("T") + "', fal_terminado = 1 where fal_id = " + item.idFaltantes + " and fal_estado = 'R'", ConexionMaestra.con);
        //                update.ExecuteNonQuery();
        //                await DisplayAlert("Mensaje", "El faltante ha sido finalizado", "OK");
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //        else if (EstadoFaltante == "F")
        //        {
        //            SqlCommand update = new SqlCommand("Update FaltantesApp set fal_estado = 'R' where fal_id = " + item.idFaltantes + " and fal_estado = 'F'", ConexionMaestra.con);
        //            update.ExecuteNonQuery();
        //            await DisplayAlert("Mensaje", "Cambio realizado con Exito", "OK");
        //        }
        //    }
        //    catch 
        //    {
        //        await DisplayAlert("Mensaje", "Error", "OK");
        //    }

        //}

        //string name = DeviceInfo.Name;
        public static string ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
        public static int name = Convert.ToInt32(File.ReadAllText(ruta));
        private async void BtnElimStock(object obj)
        {
            //var data = Preferences.Get("data", "");
            //var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
            //var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
            //var s = obj as MStock;
            //SqlCommand lastid = new SqlCommand("Select MAX(fal_id) from FaltantesApp where fal_idusu = " + usu_codigo + " and fal_terminal = " + name + "", ConexionMaestra.con);
            //var id = lastid.ExecuteScalar();
            //SqlCommand etiqueta = new SqlCommand("Select det_cantidad from DetalleFaltanteApp where det_codtex = '" + s.aux_codtex + "' and det_codnum = " + s.aux_codnum + " and det_adicional = '" + s.aux_adicional + "' and det_codigo = "+id+"", ConexionMaestra.con);
            //int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
            //if (dr == 1)
            //{
            //    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
            //    if (accion == "SI")
            //    {
            //        SqlCommand insert = new SqlCommand("Delete from DetalleFaltanteApp where det_codtex = '" + s.aux_codtex + "' and det_codnum = " + s.aux_codnum + " and det_adicional = '" + s.aux_adicional + "' and det_codigo = " + id + "", ConexionMaestra.con);
            //        insert.ExecuteNonQuery();
            //        await DisplayAlert("Mensaje", "Ha eliminado el articulo del stock", "OK");
            //        //await PopupNavigation.Instance.PopAsync();
            //        await Application.Current.MainPage.Navigation.PopAsync();
            //        //await PopupNavigation.Instance.PushAsync(new ListaStockFaltante());
            //        return;
            //    }
            //    else
            //    {

            //    }
            //}
            //if (dr > 0)
            //{
            //    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
            //    if (accion=="SI")
            //    {
            //        int cont = dr;
            //        cont -= 1;
            //        SqlCommand update = new SqlCommand("Update DetalleFaltanteApp set det_cantidad = " + cont + " where det_codtex = '" + s.aux_codtex + "' and det_codnum = " + s.aux_codnum + " and det_adicional = '" + s.aux_adicional + "' and det_codigo = " + id + "", ConexionMaestra.con);
            //        update.ExecuteNonQuery();
            //        await DisplayAlert("Aclaracion", "Quedan " + cont + " etiquetas", "OK");
            //        //await PopupNavigation.Instance.PopAsync();
            //        //await PopupNavigation.Instance.PushAsync(new ListaStockFaltante());
            //        await Application.Current.MainPage.Navigation.PopAsync();
            //    }
            //    else
            //    {

            //    }
            //}
        }

        private async Task Refresh()
        {
            IsRefreshing = true;
        }

        private async void BtnAgStock(object obj)
        {
            var data = Preferences.Get("data", "");
            var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
            var usu_codigo = Convert.ToString(deserialize?.usu_codigo);
            var st = obj as MFaltantes;
            SqlCommand lastid = new SqlCommand("Select MAX(fal_id) from FaltantesApp where fal_idusu = "+usu_codigo+" and fal_terminal = "+name+"", ConexionMaestra.con);
            var id = lastid.ExecuteScalar();
            SqlCommand stock = new SqlCommand("Select count(*) from DetalleFaltanteApp where det_codtex = '" + st.art_codtex+"' and det_codnum = "+st.art_codnum+" and det_adicional = '"+st.adi_codigo+ "' and det_codigo = " + id + "", ConexionMaestra.con);
            var dr = Convert.ToInt32(stock.ExecuteScalar());
            if (dr == 0)
            {
                var x = await Application.Current.MainPage.DisplayPromptAsync("Mensaje", "Cantidad de stock", "OK", "Cancelar", null, 5, Keyboard.Numeric , "1");
                if (x != Convert.ToString(0) && x != null)
                {
                    SqlCommand insert = new SqlCommand("Insert into DetalleFaltanteApp (det_codigo,det_codtex,det_codnum,det_descri,det_adicional,det_usuario,det_fecing,det_horaing,det_cantidad,det_check) values " +
                        "("+id+", '"+st.art_codtex+"', "+st.art_codnum+", '"+st.art_descri+"', '"+st.adi_codigo+"', "+usu_codigo+",'"+DateTime.Now.ToString("d")+"','"+DateTime.Now.ToString("T")+"', "+x+", 0)", ConexionMaestra.con);
                    insert.ExecuteNonQuery();
                    await DisplayAlert("Mensaje", "Se han agregado " + x + " articulos", "OK");
                }
                else if (x == null)
                {
                    return;
                }
            }
            if (dr > 0)
            {
                var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea modificar la cantidad?", null, "Cancelar", "SI", "NO");
                if (accion == "SI")
                {
                    SqlCommand query = new SqlCommand("Select det_cantidad from DetalleFaltanteApp where det_codtex = '" + st.art_codtex+"' and det_codnum = "+st.art_codnum+" and det_adicional = '"+st.adi_codigo+ "' and det_codigo = " + id + "", ConexionMaestra.con);
                    int cont = Convert.ToInt32(query.ExecuteScalar());
                    var x = await Application.Current.MainPage.DisplayPromptAsync("Mensaje", "Ingrese la cantidad", "OK", "Cancelar", null, 5, Keyboard.Numeric , "" + cont + "");
                    SqlCommand update = new SqlCommand("Update DetalleFaltanteApp set det_cantidad = " + x+ " where det_codtex = '" + st.art_codtex + "' and det_codnum = " + st.art_codnum + " and det_adicional = '" + st.adi_codigo + "' and det_codigo = " + id + "", ConexionMaestra.con);
                    update.ExecuteNonQuery();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
