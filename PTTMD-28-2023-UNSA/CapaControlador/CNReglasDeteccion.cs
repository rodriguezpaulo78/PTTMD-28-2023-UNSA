using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;

namespace PTTMD_28_2023_UNSA.CapaControlador
{
    /// <summary>
    /// Reglas y umbrales del modelo (Doc 01, tabla 6.3.3).
    /// Se pueden leer de un archivo de texto para calibrarlas sin recompilar.
    /// </summary>
    public class CNReglasDeteccion
    {
        private readonly Dictionary<string, ReglaDeteccion> paReglas = new Dictionary<string, ReglaDeteccion>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<ReglaDeteccion> paTodas { get { return paReglas.Values; } }

        public ReglaDeteccion mxObtener(string tcClave)
        {
            ReglaDeteccion r;
            if (!paReglas.TryGetValue(tcClave, out r)) throw new KeyNotFoundException("No existe la regla: " + tcClave);
            return r;
        }

        private void mxAgregar(string tcClave, TipoRegla tnTipo, double tnNormal, double tnModerada, Severidad tnFija, string tcDescripcion)
        {
            paReglas[tcClave] = new ReglaDeteccion { pcClave = tcClave, pnTipo = tnTipo, pnLimiteNormal = tnNormal, pnLimiteModerada = tnModerada, pnSeveridadFija = tnFija, pcDescripcion = tcDescripcion };
        }

        // ---------------- Evaluación ----------------

        /// <summary>Severidad de una variación porcentual, una diferencia en unidades o un conteo (se usa el valor absoluto).</summary>
        public Severidad mxEvaluarMagnitud(string tcClave, double tnMagnitud)
        {
            ReglaDeteccion r = mxObtener(tcClave);
            if (r.pnTipo == TipoRegla.Fija) return r.pnSeveridadFija;
            double m = Math.Abs(tnMagnitud);
            if (m <= r.pnLimiteNormal) return Severidad.Normal;
            if (m <= r.pnLimiteModerada) return Severidad.Moderada;
            return Severidad.Critica;
        }

        /// <summary>Categoría de un valor absoluto: menor que pnLimiteNormal = Normal; hasta pnLimiteModerada = Moderada; mayor = Crítica.</summary>
        public Severidad mxCategoriaValor(string tcClave, double tnValor)
        {
            ReglaDeteccion r = mxObtener(tcClave);
            if (tnValor < r.pnLimiteNormal) return Severidad.Normal;
            if (tnValor <= r.pnLimiteModerada) return Severidad.Moderada;
            return Severidad.Critica;
        }

        public Severidad mxSeveridadFija(string tcClave) { return mxObtener(tcClave).pnSeveridadFija; }

        // ---------------- Valores por defecto ----------------

        public static CNReglasDeteccion mxPorDefecto()
        {
            var c = new CNReglasDeteccion();
            const Severidad N = Severidad.Normal, M = Severidad.Moderada, C = Severidad.Critica;

            // Proyecto
            c.mxAgregar("proyecto.archivos", TipoRegla.CambioPorcentual, 10, 30, N, "Variación del número total de archivos");
            c.mxAgregar("proyecto.clases", TipoRegla.CambioPorcentual, 10, 30, N, "Variación del número total de clases");
            c.mxAgregar("proyecto.lineas", TipoRegla.CambioPorcentual, 15, 35, N, "Variación del total de líneas de código");
            c.mxAgregar("proyecto.complejidad_promedio", TipoRegla.ValorAbsoluto, 10, 15, N, "Complejidad ciclomática promedio por método");

            // Archivo
            c.mxAgregar("archivo.lineas", TipoRegla.CambioPorcentual, 15, 30, N, "Variación de líneas del archivo");
            c.mxAgregar("archivo.clases", TipoRegla.DiferenciaAbsoluta, 1, 3, N, "Diferencia en el número de clases del archivo");
            c.mxAgregar("archivo.movido", TipoRegla.Conteo, 2, 5, N, "Cantidad de archivos reubicados");
            c.mxAgregar("archivo.renombrado", TipoRegla.Conteo, 2, 5, N, "Cantidad de archivos renombrados");
            c.mxAgregar("archivo.agregado", TipoRegla.Fija, 0, 0, M, "Archivo nuevo");
            c.mxAgregar("archivo.eliminado", TipoRegla.Fija, 0, 0, C, "Archivo eliminado");
            c.mxAgregar("archivo.solo_formato", TipoRegla.Fija, 0, 0, N, "Cambio solo de formato o comentarios");

            // Clase
            c.mxAgregar("clase.metodos", TipoRegla.DiferenciaAbsoluta, 2, 5, N, "Diferencia en el número de métodos");
            c.mxAgregar("clase.atributos", TipoRegla.DiferenciaAbsoluta, 1, 3, N, "Diferencia en el número de atributos");
            c.mxAgregar("clase.herencia", TipoRegla.Fija, 0, 0, M, "Cambio de clase base o interfaces");
            c.mxAgregar("clase.acceso", TipoRegla.Fija, 0, 0, M, "Cambio de visibilidad de la clase");
            c.mxAgregar("clase.agregada", TipoRegla.Fija, 0, 0, M, "Clase nueva");
            c.mxAgregar("clase.eliminada", TipoRegla.Fija, 0, 0, C, "Clase eliminada");
            c.mxAgregar("clase.renombrada", TipoRegla.Fija, 0, 0, M, "Clase renombrada");
            c.mxAgregar("clase.reubicada", TipoRegla.Fija, 0, 0, M, "Clase movida a otro archivo o espacio de nombres");
            c.mxAgregar("clase.miembros", TipoRegla.Fija, 0, 0, N, "Cambio en campos o propiedades, fuera de los métodos");

            // Método
            c.mxAgregar("metodo.cuerpo", TipoRegla.CambioPorcentual, 20, 50, N, "Porcentaje del cuerpo del método que cambió");
            c.mxAgregar("metodo.parametros", TipoRegla.DiferenciaAbsoluta, 1, 2, N, "Diferencia en el número de parámetros");
            c.mxAgregar("metodo.retorno", TipoRegla.Fija, 0, 0, M, "Cambio del tipo de retorno");
            c.mxAgregar("metodo.acceso", TipoRegla.Fija, 0, 0, C, "Cambio de visibilidad del método");
            c.mxAgregar("metodo.complejidad", TipoRegla.ValorAbsoluto, 10, 15, N, "Complejidad ciclomática del método");
            c.mxAgregar("metodo.lineas", TipoRegla.ValorAbsoluto, 40, 80, N, "Longitud del método en líneas");
            c.mxAgregar("metodo.llamadas", TipoRegla.ValorAbsoluto, 5, 10, N, "Llamadas a otros métodos");
            c.mxAgregar("metodo.trycatch", TipoRegla.ValorAbsoluto, 2, 3, N, "Bloques try/catch del método");
            c.mxAgregar("metodo.trycatch_eliminado", TipoRegla.Fija, 0, 0, M, "Se eliminó manejo de excepciones");
            c.mxAgregar("metodo.agregado", TipoRegla.Fija, 0, 0, M, "Método nuevo");
            c.mxAgregar("metodo.eliminado", TipoRegla.Fija, 0, 0, M, "Método eliminado");
            c.mxAgregar("metodo.renombrado", TipoRegla.Fija, 0, 0, M, "Método renombrado");
            return c;
        }

        // ---------------- Archivo de configuración ----------------
        // Formato: clave;tipo;limiteNormal;limiteModerada;severidadFija
        // Las líneas que empiezan con # son comentarios.

        public void mxGuardar(string tcRuta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Reglas y umbrales del modelo de detección de anomalías estructurales");
            sb.AppendLine("# clave;tipo;limiteNormal;limiteModerada;severidadFija");
            foreach (var r in paReglas.Values)
            {
                sb.AppendLine("# " + r.pcDescripcion);
                sb.AppendLine(string.Join(";", new[] {
                    r.pcClave, r.pnTipo.ToString(),
                    r.pnLimiteNormal.ToString(CultureInfo.InvariantCulture),
                    r.pnLimiteModerada.ToString(CultureInfo.InvariantCulture),
                    r.pnSeveridadFija.ToString() }));
            }
            File.WriteAllText(tcRuta, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Lee las reglas desde reglas.cfg, ubicado junto al ejecutable.
        /// Si el archivo no existe, lo crea con los valores por defecto para que se puedan calibrar.
        /// </summary>
        public static CNReglasDeteccion mxCargarDeAplicacion()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reglas.cfg");
            if (File.Exists(ruta)) return mxCargar(ruta);

            var porDefecto = mxPorDefecto();
            try { porDefecto.mxGuardar(ruta); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            return porDefecto;
        }

        /// <summary>Parte de los valores por defecto y reemplaza los que estén en el archivo.</summary>
        public static CNReglasDeteccion mxCargar(string tcRuta)
        {
            var c = mxPorDefecto();
            if (!File.Exists(tcRuta)) return c;
            int numero = 0;
            foreach (string linea in File.ReadAllLines(tcRuta, Encoding.UTF8))
            {
                numero++;
                string l = linea.Trim();
                if (l.Length == 0 || l.StartsWith("#")) continue;
                string[] p = l.Split(';');
                if (p.Length < 5) throw new FormatException("Línea " + numero + " del archivo de reglas: se esperaban 5 campos.");
                ReglaDeteccion r;
                if (!c.paReglas.TryGetValue(p[0].Trim(), out r)) throw new FormatException("Línea " + numero + ": regla desconocida '" + p[0] + "'.");
                r.pnTipo = (TipoRegla)Enum.Parse(typeof(TipoRegla), p[1].Trim(), true);
                r.pnLimiteNormal = double.Parse(p[2].Trim(), CultureInfo.InvariantCulture);
                r.pnLimiteModerada = double.Parse(p[3].Trim(), CultureInfo.InvariantCulture);
                r.pnSeveridadFija = (Severidad)Enum.Parse(typeof(Severidad), p[4].Trim(), true);
            }
            return c;
        }
    }
}
