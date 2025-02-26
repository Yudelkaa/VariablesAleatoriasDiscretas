using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion
{
    public class Habitacion
    {
        public int Numero { get; set; }
        public int Precio { get; set; }
        public string Tipo { get; set; } // Económica, Estándar, Lujo
        public string Tamano { get; set; } // Pequeña, Mediana, Grande
        public int Camas { get; set; }
        public int VecesOcupada { get; set; } = 0;
        public bool TieneVistaAlMar {get;set;}
        public bool CercaAscensor{get;set;}
        public bool TieneBalcon { get;set;}
        public bool Ocupada { get; set; }
        public Habitacion(int numero, string tipo, string tamano, int camas, bool vista, bool ascensor, bool balcon)
        {
            Numero = numero; Tipo = tipo; Tamano = tamano; Camas = camas;
            TieneVistaAlMar = vista; CercaAscensor = ascensor; TieneBalcon = balcon;
        }

    }
}