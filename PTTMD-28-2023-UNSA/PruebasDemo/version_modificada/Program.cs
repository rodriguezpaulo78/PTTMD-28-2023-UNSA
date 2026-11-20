using PTTMD_28_2023_UNSA.CapaPresentacion;
using System;
// comentario agregado sin cambiar el código
using System.Windows.Forms;

namespace PTTMD_28_2023_UNSA
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();   
            Application.SetCompatibleTextRenderingDefault(false);
            // Prototipo: se abre directamente la pantalla de comparación de versiones.
            // Para volver al menú anterior: Application.Run(new CPInterfazPrincipal());
            Application.Run(new CPComparacion());
        }
    }
}
