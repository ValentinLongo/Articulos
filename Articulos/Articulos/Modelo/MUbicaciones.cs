using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Ubicacion_Articulos.VistaModelo;
using Xamarin.Forms;

namespace Ubicacion_Articulos.Modelo
{
    public class MUbicaciones : BaseViewModel
    {
        public string art_descri { get; set; }
        public string Ubicacion { get; set; }
        public string Codigo { get; set; }
        public string imagen { get; set; }
        public string adi_descri { get; set; }
        public string CBarra { get; set; }
        public string art_codtex { get; set; }
        public int art_codnum { get; set; }
        public string art_codfab { get; set; }
        public string art_codinterno { get; set; }
        public string adi_codigo { get; set; }
        public int Vigencia { get; set; }
        public int art_vigencia { get; set; }
        public int ada_vigencia { get; set; }

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
        //public int ubi_predef { get; set; }
    }
}
