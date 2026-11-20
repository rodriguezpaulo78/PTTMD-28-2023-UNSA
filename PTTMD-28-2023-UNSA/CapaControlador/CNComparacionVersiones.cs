using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;
using PTTMD_28_2023_UNSA.Utilidades;

namespace PTTMD_28_2023_UNSA.CapaControlador
{
    /// <summary>
    /// Submodelo 2 (Doc 01, sección 4.4): comparación de DOS versiones.
    /// Empareja archivos, clases y métodos, detecta qué cambió y clasifica
    /// cada cambio con las reglas del modelo.
    /// </summary>
    public class CNComparacionVersiones
    {
        /// <summary>Similitud mínima (0 a 1) para considerar que un elemento fue renombrado o movido y no eliminado.</summary>
        public double pnUmbralSimilitud { get; set; }

        private readonly CNReglasDeteccion poReglas;
        private List<Anomalia> paAnomalias;

        public CNComparacionVersiones(CNReglasDeteccion toReglas)
        {
            poReglas = toReglas ?? CNReglasDeteccion.mxPorDefecto();
            pnUmbralSimilitud = 0.80;
        }

        private class Par<T> { public T Base; public T Nueva; public double Similitud; }

        public Comparacion mxComparar(Evaluacion toEvaBase, Evaluacion toEvaNueva)
        {
            var reloj = Stopwatch.StartNew();
            paAnomalias = new List<Anomalia>();

            // 1. Nivel físico: archivos
            var toMapaRutas = mxCompararArchivos(toEvaBase, toEvaNueva);

            // 2. Nivel jerárquico y lógico: clases y métodos
            mxCompararClases(toEvaBase, toEvaNueva, toMapaRutas);

            // 3. Métricas globales del proyecto
            mxCompararProyecto(toEvaBase, toEvaNueva);

            reloj.Stop();
            return new Comparacion
            {
                poEvaluacionBase = toEvaBase,
                poEvaluacionNueva = toEvaNueva,
                pdFecCom = DateTime.Now,
                pnMilisegundos = reloj.ElapsedMilliseconds,
                paAnomalias = paAnomalias
                    .OrderByDescending(h => (int)h.pnSeveridad)
                    .ThenBy(h => h.pcArchivo, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(h => (int)h.pnNivel)
                    .ThenBy(h => h.pcElemento, StringComparer.OrdinalIgnoreCase)
                    .ToList()
            };
        }

        // =====================================================================
        // ARCHIVOS
        // =====================================================================

        /// <summary>
        /// Compara los archivos de ambas versiones.
        /// Devuelve, para cada archivo de la versión base que sigue existiendo,
        /// su ruta en la versión nueva (igual, o distinta si fue movido o renombrado).
        /// </summary>
        private Dictionary<string, string> mxCompararArchivos(Evaluacion toEvaBase, Evaluacion toEvaNueva)
        {
            var cmp = StringComparer.OrdinalIgnoreCase;
            var porRutaNueva = toEvaNueva.paArchivos.ToDictionary(a => a.pcRuta, cmp);
            var mapa = new Dictionary<string, string>(cmp);
            var taSoloBase = new List<Archivo>();

            foreach (var b in toEvaBase.paArchivos)
            {
                Archivo n;
                if (!porRutaNueva.TryGetValue(b.pcRuta, out n)) { taSoloBase.Add(b); continue; }
                mapa[b.pcRuta] = n.pcRuta;
                porRutaNueva.Remove(b.pcRuta);
                mxCompararArchivoModificado(b, n);
            }
            var taSoloNueva = porRutaNueva.Values.ToList();

            // Archivos que desaparecen de una ruta y aparecen en otra: ¿movidos o renombrados?
            var pares = mxEmparejar(taSoloBase, taSoloNueva, a => a.pcHashLogico, a => a.paLineasNormalizadas, 1);
            var movidos = pares.Where(p => cmp.Equals(p.Base.pcNombre, p.Nueva.pcNombre)).ToList();
            var renombrados = pares.Where(p => !cmp.Equals(p.Base.pcNombre, p.Nueva.pcNombre)).ToList();
            Severidad sevMovidos = poReglas.mxEvaluarMagnitud("archivo.movido", movidos.Count);
            Severidad sevRenombrados = poReglas.mxEvaluarMagnitud("archivo.renombrado", renombrados.Count);

            foreach (var p in pares)
            {
                mapa[p.Base.pcRuta] = p.Nueva.pcRuta;
                bool movido = cmp.Equals(p.Base.pcNombre, p.Nueva.pcNombre);
                string detalle = p.Similitud >= 1.0 ? "con el mismo contenido" : "con " + mxPct(p.Similitud * 100) + " de contenido igual";
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Archivo,
                    pcArchivo = p.Nueva.pcRuta,
                    pcElemento = "",
                    pnCambio = movido ? TipoCambio.Movido : TipoCambio.Renombrado,
                    pcMetrica = movido ? "Ubicación del archivo" : "Nombre del archivo",
                    pcValorBase = p.Base.pcRuta,
                    pcValorNuevo = p.Nueva.pcRuta,
                    pcVariacion = (movido ? movidos.Count : renombrados.Count) + " en total",
                    pnSeveridad = movido ? sevMovidos : sevRenombrados,
                    pcRegla = movido ? "archivo.movido" : "archivo.renombrado",
                    pcDescripcion = (movido ? "Archivo reubicado " : "Archivo renombrado ") + detalle + "."
                });
                if (p.Similitud < 1.0) mxCompararArchivoModificado(p.Base, p.Nueva);
            }

            foreach (var b in taSoloBase)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Archivo, pcArchivo = b.pcRuta, pcElemento = "", pnCambio = TipoCambio.Eliminado,
                    pcMetrica = "Existencia del archivo", pcValorBase = b.pnLineas + " líneas", pcValorNuevo = "-", pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("archivo.eliminado"), pcRegla = "archivo.eliminado",
                    pcDescripcion = "Archivo eliminado sin reemplazo aparente. Contenía " + b.paClases.Count + " clase(s) y " + b.paClases.Sum(c => c.paMetodos.Count) + " método(s)."
                });

            foreach (var n in taSoloNueva)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Archivo, pcArchivo = n.pcRuta, pcElemento = "", pnCambio = TipoCambio.Agregado,
                    pcMetrica = "Existencia del archivo", pcValorBase = "-", pcValorNuevo = n.pnLineas + " líneas", pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("archivo.agregado"), pcRegla = "archivo.agregado",
                    pcDescripcion = "Archivo nuevo con " + n.paClases.Count + " clase(s) y " + n.paClases.Sum(c => c.paMetodos.Count) + " método(s)."
                });

            return mapa;
        }

        private void mxCompararArchivoModificado(Archivo b, Archivo n)
        {
            if (b.pcHashFisico == n.pcHashFisico) return;

            if (b.pcHashLogico == n.pcHashLogico)
            {
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Archivo, pcArchivo = n.pcRuta, pcElemento = "", pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Contenido del archivo", pcValorBase = b.pnTamano + " bytes", pcValorNuevo = n.pnTamano + " bytes", pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("archivo.solo_formato"), pcRegla = "archivo.solo_formato",
                    pcDescripcion = "El archivo cambió solo en formato o comentarios; el código es el mismo."
                });
                return;
            }

            // Archivos de configuración, de proyecto o de recursos: cualquier cambio de contenido se señala,
            // aunque no cambie el número de líneas (por ejemplo, una cadena de conexión distinta).
            if (!CNAnalisisEstructural.mxEsCodigo(n.pcNombre))
            {
                double cambiado = Utilitarios.mxPorcentajeCambiado(b.paLineasNormalizadas, n.paLineasNormalizadas);
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Archivo, pcArchivo = n.pcRuta, pcElemento = "", pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Contenido del archivo", pcValorBase = b.pnLineas + " líneas", pcValorNuevo = n.pnLineas + " líneas", pcVariacion = mxPct(cambiado) + " cambió",
                    pnSeveridad = poReglas.mxSeveridadFija("archivo.configuracion"), pcRegla = "archivo.configuracion",
                    pcDescripcion = "Cambió un archivo de configuración, de proyecto o de recursos (" + mxPct(cambiado) + " de su contenido). Revise que el cambio esté autorizado."
                });
                return;
            }

            double variacion = Utilitarios.mxVariacionPorcentual(b.pnLineas, n.pnLineas);
            mxAgregar(new Anomalia
            {
                pnNivel = NivelElemento.Archivo, pcArchivo = n.pcRuta, pcElemento = "", pnCambio = TipoCambio.Modificado,
                pcMetrica = "Líneas del archivo", pcValorBase = b.pnLineas.ToString(), pcValorNuevo = n.pnLineas.ToString(), pcVariacion = mxPctSigno(variacion),
                pnSeveridad = poReglas.mxEvaluarMagnitud("archivo.lineas", variacion), pcRegla = "archivo.lineas",
                pcDescripcion = "Archivo modificado. Sus líneas de código variaron " + mxPctSigno(variacion) + "."
            });

            int dif = n.paClases.Count - b.paClases.Count;
            if (dif != 0)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Archivo, pcArchivo = n.pcRuta, pcElemento = "", pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Clases en el archivo", pcValorBase = b.paClases.Count.ToString(), pcValorNuevo = n.paClases.Count.ToString(), pcVariacion = mxSigno(dif),
                    pnSeveridad = poReglas.mxEvaluarMagnitud("archivo.clases", dif), pcRegla = "archivo.clases",
                    pcDescripcion = "El número de clases del archivo cambió en " + mxSigno(dif) + "."
                });
        }

        // =====================================================================
        // CLASES
        // =====================================================================

        private void mxCompararClases(Evaluacion toEvaBase, Evaluacion toEvaNueva, Dictionary<string, string> toMapaRutas)
        {
            var cmp = StringComparer.OrdinalIgnoreCase;
            var rutasNuevasConocidas = new HashSet<string>(toMapaRutas.Values, cmp);

            var clasesBase = mxIndexarClases(toEvaBase);
            var clasesNueva = mxIndexarClases(toEvaNueva);
            var taSoloBase = new List<Clase>();

            foreach (var kv in clasesBase)
            {
                Clase n;
                if (!clasesNueva.TryGetValue(kv.Key, out n)) { taSoloBase.Add(kv.Value); continue; }
                clasesNueva.Remove(kv.Key);
                mxCompararClase(kv.Value, n, toMapaRutas, false);
            }
            var taSoloNueva = clasesNueva.Values.ToList();

            // Clases que desaparecen con un nombre y aparecen con otro: ¿renombradas o cambiadas de espacio de nombres?
            var pares = mxEmparejar(taSoloBase, taSoloNueva, c => c.pcHash, c => c.paLineasNormalizadas, 3);
            foreach (var p in pares)
            {
                bool mismoNombre = p.Base.pcNomCla == p.Nueva.pcNomCla;
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = p.Nueva.pcRutaArchivo, pcElemento = p.Nueva.pcNomCla,
                    pnCambio = mismoNombre ? TipoCambio.Movido : TipoCambio.Renombrado,
                    pcMetrica = mismoNombre ? "Espacio de nombres de la clase" : "Nombre de la clase",
                    pcValorBase = p.Base.pcNombreCompleto, pcValorNuevo = p.Nueva.pcNombreCompleto, pcVariacion = mxPct(p.Similitud * 100) + " igual",
                    pnSeveridad = poReglas.mxSeveridadFija(mismoNombre ? "clase.reubicada" : "clase.renombrada"),
                    pcRegla = mismoNombre ? "clase.reubicada" : "clase.renombrada",
                    pcDescripcion = (mismoNombre ? "La clase cambió de espacio de nombres" : "La clase '" + p.Base.pcNomCla + "' desaparece y aparece '" + p.Nueva.pcNomCla + "'") +
                                  " con " + mxPct(p.Similitud * 100) + " de contenido igual."
                });
                mxCompararClase(p.Base, p.Nueva, toMapaRutas, true);
            }

            // Las clases de un archivo eliminado o agregado ya quedan cubiertas por el hallazgo del archivo.
            foreach (var b in taSoloBase.Where(c => toMapaRutas.ContainsKey(c.pcRutaArchivo)))
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = toMapaRutas[b.pcRutaArchivo], pcElemento = b.pcNomCla, pnCambio = TipoCambio.Eliminado,
                    pcMetrica = "Existencia de la clase", pcValorBase = b.paMetodos.Count + " métodos", pcValorNuevo = "-", pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("clase.eliminada"), pcRegla = "clase.eliminada",
                    pcDescripcion = "Clase eliminada (" + b.paMetodos.Count + " métodos, " + b.pnLineas + " líneas) sin equivalente en la versión nueva."
                });

            foreach (var n in taSoloNueva.Where(c => rutasNuevasConocidas.Contains(c.pcRutaArchivo)))
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla, pnCambio = TipoCambio.Agregado,
                    pcMetrica = "Existencia de la clase", pcValorBase = "-", pcValorNuevo = n.paMetodos.Count + " métodos", pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("clase.agregada"), pcRegla = "clase.agregada",
                    pcDescripcion = "Clase nueva (" + n.paMetodos.Count + " métodos, " + n.pnLineas + " líneas) dentro de un archivo existente."
                });
        }

        /// <summary>Índice de clases por nombre completo. Las clases parciales se distinguen por su archivo.</summary>
        private static Dictionary<string, Clase> mxIndexarClases(Evaluacion p)
        {
            var todas = p.paArchivos.SelectMany(a => a.paClases).ToList();
            var repetidos = new HashSet<string>(todas.GroupBy(c => c.pcNombreCompleto).Where(g => g.Count() > 1).Select(g => g.Key));
            var indice = new Dictionary<string, Clase>();
            foreach (var c in todas)
            {
                string tcClave = repetidos.Contains(c.pcNombreCompleto) ? c.pcNombreCompleto + "@" + c.pcRutaArchivo.ToLowerInvariant() : c.pcNombreCompleto;
                int i = 2;
                string unica = tcClave;
                while (indice.ContainsKey(unica)) unica = tcClave + "#" + (i++);
                indice[unica] = c;
            }
            return indice;
        }

        private void mxCompararClase(Clase b, Clase n, Dictionary<string, string> toMapaRutas, bool tlYaRenombrada)
        {
            // ¿La clase pasó a otro archivo?
            string rutaEsperada;
            if (!tlYaRenombrada && toMapaRutas.TryGetValue(b.pcRutaArchivo, out rutaEsperada) &&
                !StringComparer.OrdinalIgnoreCase.Equals(rutaEsperada, n.pcRutaArchivo))
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla, pnCambio = TipoCambio.Movido,
                    pcMetrica = "Archivo que contiene la clase", pcValorBase = b.pcRutaArchivo, pcValorNuevo = n.pcRutaArchivo, pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("clase.reubicada"), pcRegla = "clase.reubicada",
                    pcDescripcion = "La clase fue movida de un archivo a otro."
                });

            if (b.pcHash == n.pcHash) return;

            int antes = paAnomalias.Count;

            if (b.pcClaseBase != n.pcClaseBase)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Herencia", pcValorBase = mxVacio(b.pcClaseBase), pcValorNuevo = mxVacio(n.pcClaseBase), pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("clase.herencia"), pcRegla = "clase.herencia",
                    pcDescripcion = "Cambió la clase base o las interfaces de la clase."
                });

            if (b.pcAcceso != n.pcAcceso)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Visibilidad de la clase", pcValorBase = b.pcAcceso, pcValorNuevo = n.pcAcceso, pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("clase.acceso"), pcRegla = "clase.acceso",
                    pcDescripcion = "Cambió la visibilidad de la clase."
                });

            int difMetodos = n.paMetodos.Count - b.paMetodos.Count;
            if (difMetodos != 0)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Número de métodos", pcValorBase = b.paMetodos.Count.ToString(), pcValorNuevo = n.paMetodos.Count.ToString(), pcVariacion = mxSigno(difMetodos),
                    pnSeveridad = poReglas.mxEvaluarMagnitud("clase.metodos", difMetodos), pcRegla = "clase.metodos",
                    pcDescripcion = "El número de métodos de la clase cambió en " + mxSigno(difMetodos) + "."
                });

            int difAtributos = n.pnAtributos - b.pnAtributos;
            if (difAtributos != 0)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Atributos definidos", pcValorBase = b.pnAtributos.ToString(), pcValorNuevo = n.pnAtributos.ToString(), pcVariacion = mxSigno(difAtributos),
                    pnSeveridad = poReglas.mxEvaluarMagnitud("clase.atributos", difAtributos), pcRegla = "clase.atributos",
                    pcDescripcion = "El número de atributos de la clase cambió en " + mxSigno(difAtributos) + "."
                });

            mxCompararMetodos(b, n);

            // La clase cambió, pero ni sus métodos ni sus métricas lo explican: el cambio está en campos o propiedades.
            if (paAnomalias.Count == antes && !tlYaRenombrada)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Clase, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Miembros de la clase", pcValorBase = b.pnLineas + " líneas", pcValorNuevo = n.pnLineas + " líneas",
                    pcVariacion = mxPct(Utilitarios.mxPorcentajeCambiado(b.paLineasNormalizadas, n.paLineasNormalizadas)) + " cambiado",
                    pnSeveridad = poReglas.mxSeveridadFija("clase.miembros"), pcRegla = "clase.miembros",
                    pcDescripcion = "La clase cambió fuera de sus métodos (campos, propiedades o atributos)."
                });
        }

        // =====================================================================
        // MÉTODOS
        // =====================================================================

        private void mxCompararMetodos(Clase b, Clase n)
        {
            var nuevos = new List<Metodo>(n.paMetodos);
            var taSoloBase = new List<Metodo>();

            // 1. Misma firma (nombre y tipos de parámetros)
            foreach (var mb in b.paMetodos)
            {
                var mn = nuevos.FirstOrDefault(x => x.pcFirma == mb.pcFirma);
                if (mn == null) { taSoloBase.Add(mb); continue; }
                nuevos.Remove(mn);
                mxCompararMetodo(n, mb, mn);
            }

            // 2. Mismo nombre con otros parámetros: el método sigue existiendo, cambió su firma
            foreach (var mb in taSoloBase.ToList())
            {
                var candidatos = nuevos.Where(x => x.pcNomMet == mb.pcNomMet).ToList();
                if (candidatos.Count == 0) continue;
                var mn = candidatos.OrderByDescending(x => Utilitarios.mxSimilitud(mb.paLineasCuerpo, x.paLineasCuerpo)).First();
                taSoloBase.Remove(mb);
                nuevos.Remove(mn);
                mxCompararMetodo(n, mb, mn);
            }

            // 3. Otro nombre con el mismo cuerpo (o casi): método renombrado
            var pares = mxEmparejar(taSoloBase, nuevos, m => m.pcHash, m => m.paLineasCuerpo, 3);
            foreach (var p in pares)
            {
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Metodo, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla + "." + p.Nueva.pcFirma, pnCambio = TipoCambio.Renombrado,
                    pcMetrica = "Nombre del método", pcValorBase = p.Base.pcFirma, pcValorNuevo = p.Nueva.pcFirma, pcVariacion = mxPct(p.Similitud * 100) + " igual",
                    pnSeveridad = poReglas.mxSeveridadFija("metodo.renombrado"), pcRegla = "metodo.renombrado",
                    pcDescripcion = "El método '" + p.Base.pcNomMet + "' desaparece y aparece '" + p.Nueva.pcNomMet + "' con " + mxPct(p.Similitud * 100) + " del cuerpo igual."
                });
                mxCompararMetodo(n, p.Base, p.Nueva);
            }

            foreach (var mb in taSoloBase)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Metodo, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla + "." + mb.pcFirma, pnCambio = TipoCambio.Eliminado,
                    pcMetrica = "Existencia del método", pcValorBase = mb.pnCantLineas + " líneas", pcValorNuevo = "-", pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("metodo.eliminado"), pcRegla = "metodo.eliminado",
                    pcDescripcion = "Método eliminado (" + mb.pnCantLineas + " líneas, complejidad " + mb.pnComplejidad + ") sin equivalente en la clase."
                });

            foreach (var mn in nuevos)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Metodo, pcArchivo = n.pcRutaArchivo, pcElemento = n.pcNomCla + "." + mn.pcFirma, pnCambio = TipoCambio.Agregado,
                    pcMetrica = "Existencia del método", pcValorBase = "-", pcValorNuevo = mn.pnCantLineas + " líneas", pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("metodo.agregado"), pcRegla = "metodo.agregado",
                    pcDescripcion = "Método nuevo (" + mn.pnCantLineas + " líneas, complejidad " + mn.pnComplejidad + ", " + mn.pnLlamadas + " llamadas)."
                });
        }

        private void mxCompararMetodo(Clase clase, Metodo b, Metodo n)
        {
            string tcElemento = clase.pcNomCla + "." + n.pcFirma;
            string tcArchivo = clase.pcRutaArchivo;

            if (b.pcAcceso != n.pcAcceso)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Metodo, pcArchivo = tcArchivo, pcElemento = tcElemento, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Visibilidad del método", pcValorBase = b.pcAcceso, pcValorNuevo = n.pcAcceso, pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("metodo.acceso"), pcRegla = "metodo.acceso",
                    pcDescripcion = "Cambió la visibilidad del método de '" + b.pcAcceso + "' a '" + n.pcAcceso + "'."
                });

            if (b.pcTipoRetorno != n.pcTipoRetorno)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Metodo, pcArchivo = tcArchivo, pcElemento = tcElemento, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Tipo de retorno", pcValorBase = mxVacio(b.pcTipoRetorno), pcValorNuevo = mxVacio(n.pcTipoRetorno), pcVariacion = "-",
                    pnSeveridad = poReglas.mxSeveridadFija("metodo.retorno"), pcRegla = "metodo.retorno",
                    pcDescripcion = "Cambió el tipo de retorno del método."
                });

            if (b.pcFirma != n.pcFirma && b.pcNomMet == n.pcNomMet)
            {
                int dif = n.pnParametros - b.pnParametros;
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Metodo, pcArchivo = tcArchivo, pcElemento = tcElemento, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Parámetros", pcValorBase = b.pcFirma, pcValorNuevo = n.pcFirma, pcVariacion = mxSigno(dif),
                    pnSeveridad = poReglas.mxEvaluarMagnitud("metodo.parametros", dif), pcRegla = "metodo.parametros",
                    pcDescripcion = dif == 0 ? "Cambió el tipo de los parámetros del método." : "El número de parámetros cambió en " + mxSigno(dif) + "."
                });
            }

            if (b.pcHash == n.pcHash) return;

            double cambiado = Utilitarios.mxPorcentajeCambiado(b.paLineasCuerpo, n.paLineasCuerpo);
            mxAgregar(new Anomalia
            {
                pnNivel = NivelElemento.Metodo, pcArchivo = tcArchivo, pcElemento = tcElemento, pnCambio = TipoCambio.Modificado,
                pcMetrica = "Cuerpo del método", pcValorBase = b.paLineasCuerpo.Count + " líneas", pcValorNuevo = n.paLineasCuerpo.Count + " líneas", pcVariacion = mxPct(cambiado) + " cambiado",
                pnSeveridad = poReglas.mxEvaluarMagnitud("metodo.cuerpo", cambiado), pcRegla = "metodo.cuerpo",
                pcDescripcion = cambiado > poReglas.mxObtener("metodo.cuerpo").pnLimiteModerada
                    ? "El método fue reescrito casi por completo conservando su nombre (" + mxPct(cambiado) + " del cuerpo cambió)."
                    : "Cambió el " + mxPct(cambiado) + " del cuerpo del método."
            });

            // Métricas con umbral absoluto: solo hay hallazgo si el método pasa a una categoría peor.
            mxTransicion(tcArchivo, tcElemento, "metodo.complejidad", "Complejidad ciclomática", b.pnComplejidad, n.pnComplejidad);
            mxTransicion(tcArchivo, tcElemento, "metodo.lineas", "Longitud del método", b.pnCantLineas, n.pnCantLineas);
            mxTransicion(tcArchivo, tcElemento, "metodo.llamadas", "Llamadas a otros métodos", b.pnLlamadas, n.pnLlamadas);
            mxTransicion(tcArchivo, tcElemento, "metodo.trycatch", "Bloques try/catch", b.pnTryCatch, n.pnTryCatch);

            if (n.pnTryCatch < b.pnTryCatch)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Metodo, pcArchivo = tcArchivo, pcElemento = tcElemento, pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Bloques try/catch", pcValorBase = b.pnTryCatch.ToString(), pcValorNuevo = n.pnTryCatch.ToString(), pcVariacion = mxSigno(n.pnTryCatch - b.pnTryCatch),
                    pnSeveridad = poReglas.mxSeveridadFija("metodo.trycatch_eliminado"), pcRegla = "metodo.trycatch_eliminado",
                    pcDescripcion = "Se eliminó manejo de excepciones del método."
                });
        }

        private void mxTransicion(string tcArchivo, string tcElemento, string tcClave, string tcMetrica, int tnValorBase, int tnValorNuevo)
        {
            Severidad antes = poReglas.mxCategoriaValor(tcClave, tnValorBase);
            Severidad despues = poReglas.mxCategoriaValor(tcClave, tnValorNuevo);
            if (despues <= antes) return;
            mxAgregar(new Anomalia
            {
                pnNivel = NivelElemento.Metodo, pcArchivo = tcArchivo, pcElemento = tcElemento, pnCambio = TipoCambio.Modificado,
                pcMetrica = tcMetrica, pcValorBase = tnValorBase.ToString(), pcValorNuevo = tnValorNuevo.ToString(), pcVariacion = mxSigno(tnValorNuevo - tnValorBase),
                pnSeveridad = despues, pcRegla = tcClave,
                pcDescripcion = tcMetrica + " pasó de " + tnValorBase + " (" + mxTexto(antes) + ") a " + tnValorNuevo + " (" + mxTexto(despues) + ")."
            });
        }

        // =====================================================================
        // PROYECTO
        // =====================================================================

        private void mxCompararProyecto(Evaluacion b, Evaluacion n)
        {
            mxPorcentual("proyecto.archivos", "Número total de archivos", b.pnTotalArchivos, n.pnTotalArchivos);
            mxPorcentual("proyecto.clases", "Número total de clases", b.pnTotalClases, n.pnTotalClases);
            mxPorcentual("proyecto.lineas", "Total de líneas de código", b.pnTotalLineas, n.pnTotalLineas);

            Severidad antes = poReglas.mxCategoriaValor("proyecto.complejidad_promedio", b.pnComplejidadPromedio);
            Severidad despues = poReglas.mxCategoriaValor("proyecto.complejidad_promedio", n.pnComplejidadPromedio);
            if (despues > antes)
                mxAgregar(new Anomalia
                {
                    pnNivel = NivelElemento.Proyecto, pcArchivo = "(proyecto)", pcElemento = "", pnCambio = TipoCambio.Modificado,
                    pcMetrica = "Complejidad promedio por método", pcValorBase = mxNum(b.pnComplejidadPromedio), pcValorNuevo = mxNum(n.pnComplejidadPromedio),
                    pcVariacion = mxPctSigno(Utilitarios.mxVariacionPorcentual(b.pnComplejidadPromedio, n.pnComplejidadPromedio)),
                    pnSeveridad = despues, pcRegla = "proyecto.complejidad_promedio",
                    pcDescripcion = "La complejidad promedio del proyecto pasó de " + mxTexto(antes) + " a " + mxTexto(despues) + "."
                });
        }

        private void mxPorcentual(string tcClave, string tcMetrica, int tnValorBase, int tnValorNuevo)
        {
            if (tnValorBase == tnValorNuevo) return;
            double v = Utilitarios.mxVariacionPorcentual(tnValorBase, tnValorNuevo);
            mxAgregar(new Anomalia
            {
                pnNivel = NivelElemento.Proyecto, pcArchivo = "(proyecto)", pcElemento = "", pnCambio = TipoCambio.Modificado,
                pcMetrica = tcMetrica, pcValorBase = tnValorBase.ToString(), pcValorNuevo = tnValorNuevo.ToString(), pcVariacion = mxPctSigno(v),
                pnSeveridad = poReglas.mxEvaluarMagnitud(tcClave, v), pcRegla = tcClave,
                pcDescripcion = tcMetrica + " varió " + mxPctSigno(v) + " entre versiones."
            });
        }

        // =====================================================================
        // EMPAREJAMIENTO POR CONTENIDO
        // =====================================================================

        /// <summary>
        /// Empareja elementos que desaparecieron con elementos que aparecieron.
        /// Primero por hash idéntico; después por similitud de líneas, del par más parecido al menos parecido.
        /// Los elementos emparejados se retiran de ambas listas.
        /// </summary>
        private List<Par<T>> mxEmparejar<T>(List<T> taSoloBase, List<T> taSoloNueva, Func<T, string> hash, Func<T, List<string>> lineas, int tnMinimoLineas)
        {
            var pares = new List<Par<T>>();

            foreach (var b in taSoloBase.ToList())
            {
                if (lineas(b).Count < tnMinimoLineas) continue;
                var n = taSoloNueva.FirstOrDefault(x => hash(x) == hash(b));
                if (n == null) continue;
                pares.Add(new Par<T> { Base = b, Nueva = n, Similitud = 1.0 });
                taSoloBase.Remove(b);
                taSoloNueva.Remove(n);
            }

            var candidatos = new List<Par<T>>();
            foreach (var b in taSoloBase)
            {
                if (lineas(b).Count < tnMinimoLineas) continue;
                foreach (var n in taSoloNueva)
                {
                    if (lineas(n).Count < tnMinimoLineas) continue;
                    double s = Utilitarios.mxSimilitud(lineas(b), lineas(n));
                    if (s >= pnUmbralSimilitud) candidatos.Add(new Par<T> { Base = b, Nueva = n, Similitud = s });
                }
            }
            foreach (var c in candidatos.OrderByDescending(x => x.Similitud))
            {
                if (!taSoloBase.Contains(c.Base) || !taSoloNueva.Contains(c.Nueva)) continue;
                pares.Add(c);
                taSoloBase.Remove(c.Base);
                taSoloNueva.Remove(c.Nueva);
            }
            return pares;
        }

        // =====================================================================
        // FORMATO
        // =====================================================================

        private void mxAgregar(Anomalia toAnomalia)
        {
            // Los campos de texto son los que se guardarán en la tabla de anomalías
            toAnomalia.pcSeveridad = mxTexto(toAnomalia.pnSeveridad);
            toAnomalia.pcTipoAnom = toAnomalia.pnCambio.ToString();
            toAnomalia.pdFecDet = DateTime.Now;
            paAnomalias.Add(toAnomalia);
        }

        private static string mxSigno(int n) { return (n > 0 ? "+" : "") + n.ToString(CultureInfo.InvariantCulture); }
        private static string mxNum(double d) { return d.ToString("0.##", CultureInfo.InvariantCulture); }
        private static string mxPct(double d) { return d.ToString("0.#", CultureInfo.InvariantCulture) + " %"; }
        private static string mxPctSigno(double d) { return (d > 0 ? "+" : "") + mxPct(d); }
        private static string mxVacio(string s) { return string.IsNullOrEmpty(s) ? "(ninguno)" : s; }

        public static string mxTexto(Severidad s)
        {
            return s == Severidad.Critica ? "Crítica" : s == Severidad.Moderada ? "Moderada" : "Normal";
        }
    }
}
