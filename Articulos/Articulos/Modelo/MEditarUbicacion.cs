using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Ubicacion_Articulos.VistaModelo;

namespace Ubicacion_Articulos.Modelo
{
    public class MEditarUbicacion : BaseViewModel
    {
        public int ubi_codigo { get; set; }
        public string ubi_adicional { get; set; }
        public int ubi_deposito { get; set; }
        public string ubicacion { get; set; }
        public string deposito { get; set; }
        public string ubi_codtex { get; set; }
        public int ubi_codnum { get; set; }
        private int Ubi_predef;
        public int ubi_predef 
        {
            get { return Ubi_predef; }
            set
            {
                if (value != Ubi_predef)
                {
                    Ubi_predef = value;
                    OnPropertyChanged("ubi_predef");
                }
            }
        }
        public string ubi_ubica1 { get; set; }
        public string ubi_ubica2 { get; set; }
        public string ubi_ubica3 { get; set; }
        public string ubi_ubica4 { get; set; }
    }
}
