using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos
{
    public class CDProyecto
    {
        public CDProyecto() { }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int NumeroArchivos { get; set; }
        public int NumeroClases { get; set; }
        public int LineasCodigo { get; set; }
        public int NumeroMetodos { get; set; }
    }
}
