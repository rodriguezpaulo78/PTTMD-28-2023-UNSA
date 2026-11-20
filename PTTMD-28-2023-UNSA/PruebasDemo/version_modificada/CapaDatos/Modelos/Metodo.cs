using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Metodos (métodos detectados en una clase)
        public class Metodo
        {
            // PK
            public int pnCodMet { get; set; }

            // FK a Clases.pnCodCla
            public int pnCodCla { get; set; }

            // Nombre del método
            public string pcNomMet { get; set; }

            // Tipo de retorno
            public string pcTipoRetorno { get; set; }

            // Cantidad de líneas
            public int pnCantLineas { get; set; }

            // Complejidad ciclomática (u otra métrica)
            public int pnComplejidad { get; set; }
        }
}
