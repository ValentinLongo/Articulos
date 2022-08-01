using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text;
using Ubicacion_Articulos.Conexion;
using Ubicacion_Articulos.Modelo;
using Ubicacion_Articulos.Vistas;
using Xamarin.Forms;

namespace Ubicacion_Articulos.VistaModelo
{
    public class VMLogin: BaseViewModel
    {
        public VMLogin()
        {
            ValidarConexionInternet();
        }
        public ObservableCollection<MLogin> ValidarUsuario(string usu, string pas)
        {
            //Paso mis variables a mayusculas
            usu = usu.ToUpper(); 
            pas = pas.ToUpper();
            ConexionMaestra.abrir();
            // Creo una lista donde voy a guardar los registros que me traera la DB
            ObservableCollection<MLogin> login = new ObservableCollection<MLogin>();
            SqlCommand cmd = new SqlCommand();
            string cad = "select * from Usuario where usu_login = '" + usu + "' and usu_contraseña = '" + pas + "'";
            cmd = new SqlCommand(cad, ConexionMaestra.con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows != true)
            {
                Application.Current.MainPage.DisplayAlert("Error", "Revise su Usuario o Contraseña", "OK");
            }
            else
            {
                while (dr.Read())
                {
                    MLogin u = new MLogin()
                    {
                        // Atributos de mi modelo 'MLogin'
                        usu_codigo = Convert.ToInt32(dr["usu_codigo"].ToString()),
                        usu_nombre = dr["usu_nombre"].ToString(),
                        usu_perfil = Convert.ToInt32(dr["usu_perfil"].ToString())
                    };
                    login.Add(u); //Agrego los datos q traigo de la DB a mi lista 'login'
                    if (usu == u.usu_login || pas == u.usu_contraseña) //Valido q el nombre de usuario sea escrito en mayuscula
                    {
                        Application.Current.MainPage.DisplayAlert("Error", "Debe escribir su Usuario y/o Contraseña correctamente", "OK");
                    }
                    else //Si todo esta correcto, me dirige a mi 'MenuLateral'
                    {
                        //Serializar es convertir un objeto a un un archivo Json. En la variable guardo el registro completo que traigo de la DB
                        var serializer = Newtonsoft.Json.JsonConvert.SerializeObject(login[0]);
                        if (login != null && login.Count > 0)
                            Xamarin.Essentials.Preferences.Set("data", serializer);
                        Application.Current.MainPage = new MenuLateral();
                    }
                }
            }
            return login;
        }
    }
}
