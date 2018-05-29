using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LightController_Prototype
{
    public partial class Form2 : Form
    {
        Form opener;

        public Form2(Form parentForm)
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            opener = parentForm;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                
                this.Close();
            }
        }

        public Color oldColor;
        public Color newColor;
        bool change = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            newColor = FormValues.Message;

            if (change == false)
            {
                oldColor = newColor;
                this.BackColor = newColor;
                change = true;
            }

            if (oldColor != newColor)
            {
                color_fade(newColor);
            }
        }

        private async void color_fade(Color newColor)
        {
            //
            this.BackColor = newColor;
            oldColor = newColor;
        }
    }

}

