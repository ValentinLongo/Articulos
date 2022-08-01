using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Data;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Diagnostics;

namespace Ubicacion_Articulos.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PedidoIP : ContentPage
    {
        public PedidoIP()
        {
            InitializeComponent();            
        }

        string nameBD;
        string ip;
        string term;
        string ruta;
        int IdUsuario;
        string parte1 = "Data Source = ";
        string parte2 = "; Initial Catalog = ";
        string parte3 = "; User ID = sa; Password=1220; MultipleActiveResultSets=True;Encrypt=False;TrustServerCertificate=true";
        string cadena_de_conexion;
        //"Data Source = 192.168.1.210\\MASER_INF;Initial Catalog =MAURO; User ID = sa; Password=1220; MultipleActiveResultSets=True";
        private void btnConectar_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIPServer.Text) || !string.IsNullOrEmpty(txtNameDB.Text))
            {
                ProbarConexion();
                ValidarConexion();
                terminalDispositivo();
                IpServidor();
                nombreBD();
            }
            else
            {
                DisplayAlert("Ingrese la Conexion", "Se requieren Datos", "OK");
            }
        }

        private async void ValidarConexion()
        {
            if (IdUsuario > 0)
            {
                crearArchivo();
                await DisplayAlert("Listo", "Vuelve abrir la aplicacion", "Ok");
                Process.GetCurrentProcess().CloseMainWindow();
            }
            else
            {
                await DisplayAlert("Sin Conexion", "Pida una conexion valida", "Ok");
            }
        }
        private void ProbarConexion()
        {
            cadena_de_conexion = parte1 + txtIPServer.Text + parte2 + txtNameDB.Text + parte3;

            try
            {
                SqlConnection conexionmanual = new SqlConnection(cadena_de_conexion);
                conexionmanual.Open();
                SqlCommand cmd = new SqlCommand("select top 1 usu_codigo from Usuario", conexionmanual);
                IdUsuario = Convert.ToInt32(cmd.ExecuteScalar());
                conexionmanual.Close();
            }
            catch (Exception)
            {
                IdUsuario = 0;
                DisplayAlert("Error", "Sin conexion", "OK");
            }
        }

        private void IpServidor()
        {
            ip = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ipServidor.txt");
            FileInfo file = new FileInfo(ip);
            StreamWriter sw;
            try
            {
                if (File.Exists(ip) == false)
                {
                    sw = File.CreateText(ip);
                    sw.WriteLine(txtIPServer.Text);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
                else if (File.Exists(ip) == true)
                {
                    File.Delete(ip); //Elimino el archivo
                    sw = File.CreateText(ip);
                    sw.WriteLine(txtIPServer.Text);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje", "" + ex.Message, "OK");
            }
        }

        private void nombreBD()
        {
            nameBD = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "nombreBD.txt");
            FileInfo file = new FileInfo(nameBD);
            StreamWriter sw;
            try
            {
                if (File.Exists(nameBD) == false)
                {
                    sw = File.CreateText(nameBD);
                    sw.WriteLine(txtNameDB.Text);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
                else if (File.Exists(nameBD) == true)
                {
                    File.Delete(nameBD); //Elimino el archivo
                    sw = File.CreateText(nameBD);
                    sw.WriteLine(txtNameDB.Text);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje", "" + ex.Message, "OK");
            }
        }
        private void crearArchivo()
        {
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "conexion.txt");
            FileInfo fi = new FileInfo(ruta); //Me permite manipular archivos.. Crear, eliminar, sobreescribir
            StreamWriter sw; //Es para sobreescribir archivos
            try
            {
                //Empiezo a crear la cadena de conexion
                if (File.Exists(ruta) == false)
                {
                    sw = File.CreateText(ruta);
                    sw.WriteLine(parte1 + txtIPServer.Text + parte2 + txtNameDB.Text + parte3);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
                else if (File.Exists(ruta) == true)
                {
                    File.Delete(ruta); //Elimino el archivo
                    sw = File.CreateText(ruta);
                    sw.WriteLine(parte1 + txtIPServer.Text + parte2 + txtNameDB.Text + parte3);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje", "" + ex.Message, "OK");
            }
        }
        private void terminalDispositivo()
        {
            term = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "terminal.txt");
            StreamWriter sw;
            try
            {
                if (File.Exists(term) == false)
                {
                    sw = File.CreateText(term);
                    sw.WriteLine(terminal.Text);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
                else if (File.Exists(ruta) == true)
                {
                    File.Delete(term); //Elimino el archivo
                    sw = File.CreateText(term);
                    sw.WriteLine(terminal.Text);
                    sw.Flush(); //Guardo el archivo
                    sw.Close(); //Cierro el archivo
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje", "" + ex.Message, "OK");
            }
        }
    }
}