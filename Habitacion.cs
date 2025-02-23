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
        public string Tipo { get; set; } // Económica, Estándar, Lujo
        public string Tamano { get; set; } // Pequeña, Mediana, Grande
        public int Camas { get; set; }
        public int VecesOcupada { get; set; } = 0;
    }
}