using PTTMD_28_2023_UNSA.CapaDatos;
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
    public partial class CPProyectos : Form
    {
        public Action<string> AlSeleccionarProyecto;
        public string pcRuta = string.Empty;

        public CPProyectos()
        {
            InitializeComponent();
        }

        private void FormSeleccionProyecto_Load(object sender, EventArgs e)
        {
            ProyectoMetodos proyectoMetodos = new ProyectoMetodos();
            var proyectos = proyectoMetodos.ObtenerTodos();

            // Agregamos columna de botón si no existe aún
            if (!dgvProyectos.Columns.Contains("Seleccionar"))
            {
                var btnCol = new DataGridViewButtonColumn();
                btnCol.HeaderText = "Acción";
                btnCol.Text = "Seleccionar";
                btnCol.Name = "Seleccionar";
                btnCol.UseColumnTextForButtonValue = true;
                dgvProyectos.Columns.Add(btnCol);
            }

            dgvProyectos.DataSource = proyectos;
        }

        private void dgvProyectos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProyectos.Columns[e.ColumnIndex].Name == "Seleccionar" && e.RowIndex >= 0)
            {
                var proyectoSeleccionado = dgvProyectos.Rows[e.RowIndex].DataBoundItem as CDProyecto;

                if (proyectoSeleccionado != null)
                {
                    pcRuta = proyectoSeleccionado.Ruta;
                    AlSeleccionarProyecto?.Invoke(pcRuta);
                    this.Close(); // opcional
                }
            }
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            AlSeleccionarProyecto?.Invoke(pcRuta);
        }
    }
}
