using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
    public partial class DetFaltante : ContentPage
    {
        VMFaltantes vMFaltantes = new VMFaltantes();
        public DetFaltante()
        {
            InitializeComponent();
            inicializarVista();
        }

        private void inicializarVista()
        {            
            try
            {
                var dato = Preferences.Get("DetFaltantes", "");
                var deserializer = JsonConvert.DeserializeObject<MEstadoFaltante>(dato);
                IdFaltante.Text = Convert.ToString(deserializer?.idFaltantes);
                var cadena = "det_codigo = " + IdFaltante.Text + "";
                listaDetFalt.ItemsSource = vMFaltantes.DetFaltantes(cadena);

                SqlCommand fin = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + IdFaltante.Text + "' and fal_estado = 'FINALIZADO'", ConexionMaestra.con);
                SqlDataReader finalizado = fin.ExecuteReader();
                if (finalizado.Read())
                {
                    btnAgFaltantes.IsVisible = false;
                    btnFinalizarFaltante.IsVisible = false;
                    btnFinalizarReg.IsVisible = false;
                }
                else
                {
                    SqlCommand reg = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + IdFaltante.Text + "' and fal_estado = 'REGISTRADO'", ConexionMaestra.con);
                    SqlDataReader registrado = reg.ExecuteReader();
                    if (registrado.Read())
                    {
                        btnAgFaltantes.IsVisible = false;
                        btnFinalizarFaltante.IsVisible = false;
                        btnFinalizarReg.IsVisible = false;
                    }
                    else
                    {
                        SqlCommand curso = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + IdFaltante.Text + "' and fal_estado = 'EN CURSO'", ConexionMaestra.con);
                        SqlDataReader enCurso = curso.ExecuteReader();
                        if (enCurso.Read())
                        {
                            btnAgFaltantes.IsVisible = true;
                            btnFinalizarFaltante.IsVisible = false;
                            btnFinalizarReg.IsVisible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }
        private async void btnFinalizarFaltante_Clicked(object sender, EventArgs e)
        {
            try
            {
                SqlCommand querycheck = new SqlCommand("select det_check from DetalleFaltanteApp where det_codigo = " + IdFaltante.Text + " and det_check = 0", ConexionMaestra.con);
                SqlDataReader reader = querycheck.ExecuteReader();
                if (reader.Read())
                {
                    await Navigation.PopAsync();

                }
                else
                {
                    var accion = await DisplayActionSheet("¿Desea finalizar con el faltante?", "Cancelar", null, "SI", "NO");
                    if (accion == "SI")
                    {
                        
                        SqlCommand updateFaltante = new SqlCommand("Update FaltantesApp set fal_estado = 'FINALIZADO', fal_terminado = 1, fal_fecfin = UPPER('" + DateTime.Now.ToString("d") + "'), fal_horafin = UPPER('" + DateTime.Now.ToString("T") + "') where fal_id = '" + IdFaltante.Text + "'", ConexionMaestra.con);
                        updateFaltante.ExecuteNonQuery();
                        await DisplayAlert("Mensaje", "Ha finalizado el Faltante", "OK");
                        await Navigation.PopAsync();
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

        private void btnAgFaltantes_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new AddFaltantes()));
            Navigation.PopAsync();
        }

        private async void chkDetFaltante_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var dato2 = Preferences.Get("DetFaltante", "");
            var deseria = JsonConvert.DeserializeObject<MDetFaltantes>(dato2);
            var det_codigo = deseria?.det_codigo;
            try
            {
                var cb = (CheckBox)sender;
                var item = (MDetFaltantes)cb.BindingContext;
                var codtex = item.det_codtex;
                var codnum = item.det_codnum;
                var codadi = item.det_adicional;
                var x = e.Value;
                if (x == true) //Pongo el check en 1 o 0 cuando el faltante esta en estado registrado
                {
                    SqlCommand chec = new SqlCommand("Update DetalleFaltanteApp set det_check = 1 where det_codtex = '" + codtex + "' and det_codnum = " + codnum + " and det_adicional = '" + codadi + "' and det_codigo = " + det_codigo + "", ConexionMaestra.con);
                    chec.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand chec = new SqlCommand("Update DetalleFaltanteApp set det_check = 0 where det_codtex = '" + codtex + "' and det_codnum = " + codnum + " and det_adicional = '" + codadi + "' and det_codigo = " + det_codigo + "", ConexionMaestra.con);
                    chec.ExecuteNonQuery();
                }
                //Deshabilito el check cuando el estado se encuentra en estado Finalizado
                SqlCommand sql = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + IdFaltante.Text + "' and fal_estado = 'FINALIZADO'", ConexionMaestra.con);
                SqlDataReader rd = sql.ExecuteReader();
                if (rd.Read())
                {
                    cb.IsEnabled = false;
                }
                //Deshabilito el check cuando el estado se encuentra en estado En Cruso
                SqlCommand a = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + IdFaltante.Text + "' and fal_estado = 'EN CURSO'", ConexionMaestra.con);
                SqlDataReader b = a.ExecuteReader();
                if (b.Read())
                {
                    cb.IsEnabled = false;
                    cb.IsChecked = false;
                }

                //Pregunto si hay check en 0 cuando mi faltante esta en estado Registrado
                SqlCommand c = new SqlCommand("Select det_check from DetalleFaltanteApp where det_codigo = "+IdFaltante.Text+" and det_check = 0",ConexionMaestra.con);
                if (c.ExecuteReader().Read())
                {
                    //Si hay check=0 no se hace nada
                }
                else //Sino pregunto si estoy en estado finalizado
                {
                    SqlCommand s = new SqlCommand("Select fal_estado from FaltantesApp where (fal_estado = 'FINALIZADO' OR fal_estado = 'EN CURSO') AND fal_id = " + IdFaltante.Text + "", ConexionMaestra.con);
                    SqlDataReader sql1 = s.ExecuteReader();
                    if (sql1.Read())
                    {
                        //Si es asi no hago nada
                    }
                    else
                    {   //Sino, si esta en registrado y chequeo el ultimo elemento de mi lista automaticamente finalizo el faltante
                        SqlCommand updateFaltante = new SqlCommand("Update FaltantesApp set fal_estado = 'FINALIZADO', fal_terminado = 1, fal_fecfin = UPPER('" + DateTime.Now.ToString("d") + "'), fal_horafin = UPPER('" + DateTime.Now.ToString("T") + "') where fal_id = '" + IdFaltante.Text + "'", ConexionMaestra.con);
                        updateFaltante.ExecuteNonQuery();
                        await DisplayAlert("Mensaje", "Ha finalizado el Faltante", "OK");
                        await Navigation.PopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void listaDetFalt_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //var img = e.Item as MDetFaltantes;
            //PopupNavigation.Instance.PushAsync(new PopImagenFaltante(img));
        }

        //private async void btnImgEliminarFaltante_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var button = (ImageButton)sender;
        //        var faltante = (MDetFaltantes)button.BindingContext;
        //        var data = Preferences.Get("data", "");
        //        var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
        //        var usu_codigo = Convert.ToString(deserialize?.usu_codigo);

        //        SqlCommand sql = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + faltante.det_codigo + "' and fal_estado = 'FINALIZADO'", ConexionMaestra.con);
        //        SqlDataReader rd = sql.ExecuteReader();
        //        if (rd.Read())
        //        {
        //            button.IsEnabled = false;
        //            return;
        //        }
        //        SqlCommand s = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + faltante.det_codigo + "' and fal_estado = 'REGISTRADO'", ConexionMaestra.con);
        //        SqlDataReader dataReader = s.ExecuteReader();
        //        if (dataReader.Read())
        //        {
        //            button.IsEnabled = false;
        //            return;
        //        }
        //        SqlCommand etiqueta = new SqlCommand("Select det_cantidad from DetalleFaltanteApp where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
        //        int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
        //        if (dr == 1)
        //        {
        //            var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar el último árticulo?", null, "Cancelar", "SI", "NO");
        //            if (accion == "SI")
        //            {
        //                SqlCommand insert = new SqlCommand("Delete from DetalleFaltanteApp where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
        //                insert.ExecuteNonQuery();
        //                //await DisplayAlert("Mensaje", "Elimino el último árticulo", "OK");
        //                SqlCommand ultimo = new SqlCommand("Select * from DetalleFaltanteApp where det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
        //                SqlDataReader d = ultimo.ExecuteReader();
        //                if (d.Read())
        //                {
        //                    OnAppearing();
        //                    return;
        //                }
        //                else
        //                {
        //                    SqlCommand delete = new SqlCommand("Delete from FaltantesApp where fal_id = " + faltante.det_codigo + "", ConexionMaestra.con);
        //                    delete.ExecuteNonQuery();
        //                    await DisplayAlert("Advertencia", "Se ha eliminado el faltante registrado", "OK");
        //                    await Navigation.PopAsync();
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //        if (dr > 0)
        //        {
        //            var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
        //            if (accion == "SI")
        //            {
        //                int cont = dr;
        //                cont -= 1;
        //                SqlCommand update = new SqlCommand("Update DetalleFaltanteApp set det_cantidad = " + cont + " where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
        //                update.ExecuteNonQuery();
        //                await DisplayAlert("Aclaracion", "Quedan " + cont + " etiquetas", "OK");
        //                OnAppearing();
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        ConexionMaestra.abrir();
        //    }
        //}

        protected override void OnAppearing()
        {
            try
            {
                inicializarVista();
                refresh.IsRefreshing = false;
            }
            catch (Exception ex)
            {
                //DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private async void btnFinalizarReg_Clicked(object sender, EventArgs e)
        {
            try
            {
                var accion = await DisplayActionSheet("¿Desea finalizar con el registro?", "Cancelar", null, "SI", "NO");
                if (accion == "SI")
                {
                    SqlCommand updateFaltante = new SqlCommand("Update FaltantesApp set fal_estado = 'REGISTRADO', fal_terminado = 0 where fal_id = '" + IdFaltante.Text + "'", ConexionMaestra.con);
                    updateFaltante.ExecuteNonQuery();
                    await DisplayAlert("Mensaje", "Ha registrado el Faltante", "OK");
                    await Navigation.PopAsync();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Mensaje", "" + ex.Message, "OK");
                ConexionMaestra.abrir();
            }
        }

        private async void btnImgEliminarFaltante_Clicked(object sender, EventArgs e)
        {
            try
            {
                var button = (SwipeItem)sender;
                var faltante = (MDetFaltantes)button.BindingContext;
                var data = Preferences.Get("data", "");
                var deserialize = JsonConvert.DeserializeObject<MLogin>(data);
                var usu_codigo = Convert.ToString(deserialize?.usu_codigo);

                SqlCommand sql = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + faltante.det_codigo + "' and fal_estado = 'FINALIZADO'", ConexionMaestra.con);
                SqlDataReader rd = sql.ExecuteReader();
                if (rd.Read())
                {
                    button.IsEnabled = false;
                    await DisplayAlert("Advertencia", "No se puede realizar esta accion cuando esta finalizado", "OK");
                    return;
                }
                SqlCommand s = new SqlCommand("Select fal_estado from FaltantesApp where fal_id = '" + faltante.det_codigo + "' and fal_estado = 'REGISTRADO'", ConexionMaestra.con);
                SqlDataReader dataReader = s.ExecuteReader();
                if (dataReader.Read())
                {
                    button.IsEnabled = false;
                    await DisplayAlert("Advertencia", "No se puede realizar esta accion cuando esta registrado", "OK");
                    return;
                }
                SqlCommand etiqueta = new SqlCommand("Select det_cantidad from DetalleFaltanteApp where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
                int dr = Convert.ToInt32(etiqueta.ExecuteScalar());
                if (dr == 1)
                {
                    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar el último árticulo?", null, "Cancelar", "SI", "NO");
                    if (accion == "SI")
                    {
                        SqlCommand insert = new SqlCommand("Delete from DetalleFaltanteApp where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
                        insert.ExecuteNonQuery();
                        //await DisplayAlert("Mensaje", "Elimino el último árticulo", "OK");
                        SqlCommand ultimo = new SqlCommand("Select * from DetalleFaltanteApp where det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
                        SqlDataReader d = ultimo.ExecuteReader();
                        if (d.Read())
                        {
                            OnAppearing();
                            return;
                        }
                        else
                        {
                            SqlCommand delete = new SqlCommand("Delete from FaltantesApp where fal_id = " + faltante.det_codigo + "", ConexionMaestra.con);
                            delete.ExecuteNonQuery();
                            await DisplayAlert("Advertencia", "Se ha eliminado el faltante registrado", "OK");
                            await Navigation.PopAsync();
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                if (dr > 0)
                {
                    var accion = await Application.Current.MainPage.DisplayActionSheet("¿Desea eliminar una unidad de éste articulo?", null, "Cancelar", "SI", "NO");
                    if (accion == "SI")
                    {
                        int cont = dr;
                        cont -= 1;
                        SqlCommand update = new SqlCommand("Update DetalleFaltanteApp set det_cantidad = " + cont + " where det_codtex = '" + faltante.det_codtex + "' and det_codnum = " + faltante.det_codnum + " and det_adicional = '" + faltante.det_adicional + "' and det_codigo = " + faltante.det_codigo + "", ConexionMaestra.con);
                        update.ExecuteNonQuery();
                        await DisplayAlert("Aclaracion", "Quedan " + cont + " etiquetas", "OK");
                        OnAppearing();
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

        private async void btnVolver_Clicked(object sender, EventArgs e)
        {
            try
            {
                SqlCommand querycheck = new SqlCommand("select det_check from DetalleFaltanteApp where det_codigo = " + IdFaltante.Text + " and det_check = 0", ConexionMaestra.con);
                SqlDataReader reader = querycheck.ExecuteReader();
                if (reader.Read())
                {
                    await Navigation.PopAsync();

                }
                else
                {
                    SqlCommand sql = new SqlCommand("Select fal_estado from FaltantesApp where fal_estado = 'FINALIZADO' AND fal_id = "+IdFaltante.Text+"", ConexionMaestra.con);
                    SqlDataReader sql1 = sql.ExecuteReader();
                    if (sql1.Read())
                    {
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        var accion = await DisplayActionSheet("¿Desea finalizar con el faltante?", "Cancelar", null, "SI", "NO");
                        if (accion == "SI")
                        {

                            SqlCommand updateFaltante = new SqlCommand("Update FaltantesApp set fal_estado = 'FINALIZADO', fal_terminado = 1, fal_fecfin = UPPER('" + DateTime.Now.ToString("d") + "'), fal_horafin = UPPER('" + DateTime.Now.ToString("T") + "') where fal_id = '" + IdFaltante.Text + "'", ConexionMaestra.con);
                            updateFaltante.ExecuteNonQuery();
                            await DisplayAlert("Mensaje", "Ha finalizado el Faltante", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {

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

        private void refresh_Refreshing(object sender, EventArgs e)
        {
            OnAppearing();
        }
    }
}