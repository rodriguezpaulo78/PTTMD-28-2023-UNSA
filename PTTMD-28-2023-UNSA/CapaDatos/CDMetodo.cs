using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos
{
    internal class CDMetodo
    {
        public CDMetodo() { }
        public string Nombre { get; set; }
        public int LineasCodigo { get; set; }
        public int NumeroCiclos { get; set; }
        public int NumeroCondicionales { get; set; }
        public string CodigoFuente { get; set; }
        public int ComplejidadCiclomatica { get; set; }
        public int NumeroParametros { get; set; }
        public int ClaseId { get; set; }
    }
}
