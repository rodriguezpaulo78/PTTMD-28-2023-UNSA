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

            // ---- Datos del análisis estructural (se llenan en memoria; no son columnas de la tabla) ----

            // Nombre + tipos de los parámetros. Identifica al método dentro de su clase
            public string pcFirma { get; set; }

            // Visibilidad (public, private, protected, internal)
            public string pcAcceso { get; set; }

            // Número de parámetros
            public int pnParametros { get; set; }

            // Llamadas a otros métodos
            public int pnLlamadas { get; set; }

            // Bloques try/catch
            public int pnTryCatch { get; set; }

            // Número de instrucciones
            public int pnInstrucciones { get; set; }

            // SHA-256 del cuerpo sin espacios ni comentarios (nivel lógico)
            public string pcHash { get; set; }

            // Líneas del cuerpo normalizadas; se usan para medir cuánto cambió
            public List<string> paLineasCuerpo { get; set; }

            public Metodo()
            {
                paLineasCuerpo = new List<string>();
            }
        }
}
