using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;
using PTTMD_28_2023_UNSA.Utilidades;

namespace PTTMD_28_2023_UNSA.CapaDatos
{
    /// <summary>
    /// Línea base del cierre de un proyecto (etapas 3 y 5 del procedimiento).
    /// Guarda en un archivo XML el resultado completo del análisis estructural de la versión de cierre,
    /// para verificar después las versiones posteriores sin volver a analizar la carpeta original.
    /// Mientras no se complete la persistencia en SQL Server, este archivo es el registro del cierre.
    /// </summary>
    public static class LineaBaseCD
    {
        public const string pcExtension = ".lineabase.xml";

        /// <summary>Guarda la evaluación en un archivo XML (UTF-8) y devuelve el SHA-256 del archivo generado.</summary>
        public static string mxGuardar(Evaluacion toEvaluacion, string tcRuta)
        {
            if (toEvaluacion == null) throw new ArgumentNullException("toEvaluacion");

            var serializador = new XmlSerializer(typeof(Evaluacion));
            var opciones = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };
            using (var escritor = XmlWriter.Create(tcRuta, opciones))
            {
                serializador.Serialize(escritor, toEvaluacion);
            }
            return mxHashArchivo(tcRuta);
        }

        /// <summary>Carga una línea base guardada con mxGuardar.</summary>
        public static Evaluacion mxCargar(string tcRuta)
        {
            if (!File.Exists(tcRuta)) throw new FileNotFoundException("No existe el archivo de línea base: " + tcRuta);

            var serializador = new XmlSerializer(typeof(Evaluacion));
            using (var lector = XmlReader.Create(tcRuta))
            {
                var evaluacion = serializador.Deserialize(lector) as Evaluacion;
                if (evaluacion == null) throw new InvalidDataException("El archivo no contiene una línea base válida.");
                return evaluacion;
            }
        }

        /// <summary>SHA-256 del archivo en disco; se imprime en el documento de cierre para comprobar que la línea base es la misma.</summary>
        public static string mxHashArchivo(string tcRuta)
        {
            return Utilitarios.mxSha256(File.ReadAllBytes(tcRuta));
        }
    }
}
