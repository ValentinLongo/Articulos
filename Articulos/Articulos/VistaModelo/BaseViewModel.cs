using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ubicacion_Articulos.VistaModelo
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        bool _conectado;
        bool _sinConexion;
        public bool Conectado
        {
            get { return _conectado; }
            set
            {
                _conectado = value;
                OnPropertyChanged();
            }
        }
        public bool SinConexion
        {
            get { return _sinConexion; }
            set
            {
                _sinConexion = value;
                OnPropertyChanged();
            }
        }
        public void ValidarConexionInternet()
        {
            var tiempo = TimeSpan.FromSeconds(1);
            Device.StartTimer(tiempo, () => 
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ProbarConexion();
                });
                return true;
            });
        }
        private void ProbarConexion()
        {
            if (Connectivity.NetworkAccess!= NetworkAccess.Internet)
            {
                Conectado = false;
                SinConexion = true;
            }
            else
            {
                Conectado = true;
                SinConexion = false; 
            }
        }
    }
}
