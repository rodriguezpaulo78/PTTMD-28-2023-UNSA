namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
    // Regla de detección con sus umbrales (Doc 01, tabla 6.3.3)
    public class ReglaDeteccion
    {
        // Clave única de la regla (por ejemplo, metodo.cuerpo)
        public string pcClave { get; set; }

        // Forma de evaluar el cambio
        public TipoRegla pnTipo { get; set; }

        // Hasta este valor el cambio es Normal
        public double pnLimiteNormal { get; set; }

        // Hasta este valor el cambio es Moderado; por encima, Crítico
        public double pnLimiteModerada { get; set; }

        // Severidad cuando la regla es de tipo Fija
        public Severidad pnSeveridadFija { get; set; }

        // Descripción de la regla
        public string pcDescripcion { get; set; }
    }
}
