using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GenerateBLK
{
    public partial class HMASplashScreen : Form
    {
        public HMASplashScreen()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            progressBar.Increment(1);
            if (progressBar.Value == 100) 
            {
                Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
