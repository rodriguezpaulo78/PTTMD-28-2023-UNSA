using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTTMD_28_2023_UNSA.CapaPresentacion
{
    public partial class CPInterfazAnalisis : Form
    {
        public CPInterfazAnalisis()
        {
            InitializeComponent();
        }

        private void btnExplorar_Click(object sender, EventArgs e)
        {
            var formSelector = new CPProyectos();

            formSelector.AlSeleccionarProyecto = (proyecto) =>
            {
                txcRuta.Text = proyecto;
                // Guardar ID, cargar datos del proyecto, etc.
            };

            formSelector.Show();
        }
    }
}
