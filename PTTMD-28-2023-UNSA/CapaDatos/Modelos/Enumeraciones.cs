namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
    // Nivel jerárquico del modelo (Doc 01, sección 4.2)
    public enum NivelElemento { Proyecto, Archivo, Clase, Metodo }

    // Tipo de cambio detectado entre dos versiones
    public enum TipoCambio { Agregado, Eliminado, Modificado, Renombrado, Movido }

    // Clasificación de la anomalía (Doc 01, sección 6.3.2)
    public enum Severidad { Normal = 0, Moderada = 1, Critica = 2 }

    // Forma en que una regla convierte un cambio en una severidad
    public enum TipoRegla
    {
        // Variación porcentual entre versiones (por ejemplo, líneas del proyecto ±15 %)
        CambioPorcentual,
        // Diferencia en unidades entre versiones (por ejemplo, ±2 métodos)
        DiferenciaAbsoluta,
        // Valor de la métrica en cada versión. Solo genera anomalía si la categoría EMPEORA
        ValorAbsoluto,
        // Cantidad de elementos afectados por un mismo tipo de cambio (por ejemplo, archivos movidos)
        Conteo,
        // El tipo de cambio tiene una severidad fija
        Fija
    }
}
