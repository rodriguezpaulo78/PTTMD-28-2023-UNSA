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
    public partial class CPInterfazPrincipal : Form
    {
        public CPInterfazPrincipal()
        {
            InitializeComponent();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            CPInterfazReconocimiento form2 = new CPInterfazReconocimiento();
            this.Hide();
            form2.ShowDialog();
            this.Show();
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            CPInterfazAnalisis form2 = new CPInterfazAnalisis();
            this.Hide();
            form2.ShowDialog();
            this.Show();
        }
    }
}
