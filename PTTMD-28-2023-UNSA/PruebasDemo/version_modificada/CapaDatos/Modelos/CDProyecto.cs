namespace PTTMD_28_2023_UNSA.CapaDatos
{
    /// <summary>
    /// Datos de un proyecto registrado, tal como los usan ProyectoMetodos y las pantallas existentes.
    /// La clase se usaba en el código pero no estaba definida, por lo que el proyecto no compilaba.
    /// </summary>
    public class CDProyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int NumeroArchivos { get; set; }
        public int NumeroClases { get; set; }
        public int LineasCodigo { get; set; }
        public int NumeroMetodos { get; set; }
        public string Ruta { get; set; }
    }
}
