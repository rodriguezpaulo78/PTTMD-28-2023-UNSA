using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos
{
    internal class CDClase
    {
        public CDClase() { }
        public string Nombre { get; set; } = string.Empty;
        public int NumeroMetodos { get; set; }
        public int NumeroPropiedades { get; set; }
        public int LineasCodigo { get; set; }
        public int VariablesLocales { get; set; }
        public decimal PorcentajeComentarios { get; set; }
        public int SumaAscii { get; set; }
        public int ProyectoId { get; set; }
    }
}
