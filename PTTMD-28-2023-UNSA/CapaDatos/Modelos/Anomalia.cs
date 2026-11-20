using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad AnomaliasDetectadas
        public class Anomalia
        {
            // PK
            public int pnCodAnom { get; set; }

            // FK a ResultadosComparacion.pnCodResCom
            public int pnCodResCom { get; set; }

            // Tipo de anomalía (ej. EliminacionSilenciosa, AumentoComplejidad)
            public string pcTipoAnom { get; set; }

            // Descripción / evidencia
            public string pcDescripcion { get; set; }

            // Fecha de detección
            public DateTime? pdFecDet { get; set; }

            // Severidad (opcional)
            public string pcSeveridad { get; set; }

            // ---- Datos del análisis estructural (se llenan en memoria; no son columnas de la tabla) ----

            // Nivel jerárquico del elemento afectado
            public NivelElemento pnNivel { get; set; }

            // Ruta del archivo donde está el elemento
            public string pcArchivo { get; set; }

            // Nombre de la clase o del método; vacío si la anomalía es de archivo o de proyecto
            public string pcElemento { get; set; }

            // Tipo de cambio detectado
            public TipoCambio pnCambio { get; set; }

            // Métrica evaluada
            public string pcMetrica { get; set; }

            // Valor en la versión base
            public string pcValorBase { get; set; }

            // Valor en la versión nueva
            public string pcValorNuevo { get; set; }

            // Variación entre versiones
            public string pcVariacion { get; set; }

            // Severidad asignada por la regla
            public Severidad pnSeveridad { get; set; }

            // Clave de la regla que generó la clasificación
            public string pcRegla { get; set; }
        }
}
