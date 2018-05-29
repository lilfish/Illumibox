using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace LightController_Prototype
{
    public partial class Form1 : Form
    {

        

        //arduino things needed for startup
        SerialPort port;
        String[] ports;
        bool isconnected = false;

        //color picker things needed for startup
        Bitmap b = new Bitmap(252, 391);
        double colorV = 0;
        float brightnesV = 255;
        int sensitivity = 0;

        

        public Form1()
        {
            InitializeComponent();
    }

        private void ValueToColor(double value)
        {

            //Console.WriteLine(value);
            Color this_color = b.GetPixel(Convert.ToInt32(value), 0);

            Color new_color = Color.FromArgb(Convert.ToInt32(brightnesV), this_color.R, this_color.G, this_color.B);

            
            panel1.BackColor = new_color;

            FormValues.Message = new_color;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //get porst for arduino.
            ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                //Console.WriteLine(ports[i]);
                ports_combobox.Items.Add(ports[i]);
            }

            //creates the gradient scale which the display is based upon... 
            LinearGradientBrush br = new LinearGradientBrush(new RectangleF(0, 0, 252, 5), Color.Black, Color.Black, 0, false);
            ColorBlend cb = new ColorBlend();
            cb.Positions = new[] { 0, 1 / 6f, 2 / 6f, 3 / 6f, 4 / 6f, 5 / 6f, 1 };
            cb.Colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.FromArgb(153, 204, 255), Color.White };
            br.InterpolationColors = cb;

            //puts the gradient scale onto a bitmap which allows for getting a color from pixel
            Graphics g = Graphics.FromImage(b);
            g.FillRectangle(br, new RectangleF(0, 0, b.Width, b.Height));

            pictureBox1.Image = b;
        }

        //change color
        private void btn_right_Click(object sender, EventArgs e)
        {
            if (sensitivity != 0)
                colorV = colorV + sensitivity;

            if (sensitivity == 0)
                colorV = colorV + 1;

            if (colorV > 250)
            {
                colorV = 0;
            }
            ValueToColor(colorV);
        }

        private void btn_left_Click(object sender, EventArgs e)
        {
            if (sensitivity != 0)
                colorV = colorV - sensitivity;

            if (sensitivity == 0)
                colorV = colorV - 1;

            if (colorV < 0)
            {
                colorV = 250;
            }
            ValueToColor(colorV);
        }

        //change brightness
        private void btn_up_Click(object sender, EventArgs e)
        {
            if (sensitivity != 0)
                brightnesV = brightnesV + sensitivity;

            if (sensitivity == 0)
                brightnesV = brightnesV + 1;

            if (brightnesV > 250)
            {
                brightnesV = 250;
            }
            ValueToColor(colorV);
        }

        private void btn_down_Click(object sender, EventArgs e)
        {
            if (sensitivity != 0)
                brightnesV = brightnesV - sensitivity;

            if (sensitivity == 0)
                brightnesV = brightnesV - 1;

            if (brightnesV < 0)
            {
                brightnesV = 0;
            }
            ValueToColor(colorV);
        }

        //disconnect arduino if form is closing.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port != null)
                port.Close();
        }

        //Connect to arduino
        string com = null;
        private void btn_connect_Click(object sender, EventArgs e)
        {
            com = ports_combobox.GetItemText(ports_combobox.SelectedItem).ToString();
            if (!String.IsNullOrEmpty(com))
            {
                if (!isconnected)
                {
                    connectToArduino();
                }
                else
                {
                    disconnectArduino();
                }
            }
        }
        //connect
        private void connectToArduino()
        {
            isconnected = true;
            string selectedPort = ports_combobox.GetItemText(ports_combobox.SelectedItem);
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
                port.WriteLine(" STARTEDUP");
                btn_connect.Text = "disconnect";
                timer1.Start();
            }
            catch (Exception e)
            {
                port.Close();
                Console.WriteLine(String.Format("Error ({0}) No RF module", e.Message));
                btn_connect.Text = "error";
            }

        }
        //disconnect
        private void disconnectArduino()
        {
            isconnected = false;
            btn_connect.Text = "connect";
            port.Write("#connectionClosed\n");
            port.Close();
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isconnected == true)
            {
                string portReader;
                portReader = port.ReadExisting().ToString();
                if (!String.IsNullOrEmpty(portReader))
                {
                    if (portReader.Contains("R") == true)
                    {
                        resetAll();
                    }
                    else
                    { 
                        //Console.WriteLine(portReader);
                        portReader.Replace("  ", string.Empty);
                        var reader_splitted = portReader.Split(',');

                        int number_x = 0;
                        int number_y = 0;

                        bool result_y = Int32.TryParse(reader_splitted[1], out number_y);

                        bool result_x = Int32.TryParse(reader_splitted[0], out number_x);



                        if (result_x && result_y)
                        {
                            if (number_x > 5)
                            {
                                if (sensitivity != 0)
                                {
                                    if (sensitivity > 0)
                                    {
                                        colorV = colorV + Convert.ToDouble((number_x * sensitivity));
                                    }
                                    else
                                    {
                                        colorV = colorV + Convert.ToDouble((number_x / sensitivity));
                                    }
                                }
                                else if (sensitivity == 0)
                                {
                                    colorV = colorV + Convert.ToDouble((number_x * 1));
                                }
                            }

                            colorV = colorV + Convert.ToDouble((number_x * 1));

                            label_x.Text = "X = " + (number_x).ToString();

                            if (number_y > 5)
                            {
                                if (sensitivity != 0)
                                {
                                    if (sensitivity > 0)
                                    {
                                        brightnesV = brightnesV + (float)((number_y * sensitivity));
                                    }
                                    else
                                    {
                                        brightnesV = brightnesV + (float)((number_y / sensitivity));
                                    }
                                }
                                else if (sensitivity == 0)
                                {
                                    brightnesV = brightnesV + (float)((number_y * 1));
                                }
                            }

                            label_y.Text = "Y = " + (number_y).ToString();

                            if (brightnesV > 250)
                            {
                                brightnesV = 250;
                            }
                            if (brightnesV < 0)
                            {
                                brightnesV = 0;
                            }
                            if (colorV < 0)
                            {
                                colorV = 250;
                            }
                            if (colorV > 250)
                            {
                                colorV = 0;
                            }

                            ValueToColor(colorV);
                        }
                        else
                        {
                            Console.WriteLine("error?");
                        }
                    }

                }
            }
        }

        private void label_y_Click(object sender, EventArgs e)
        {

        }

        private void label_x_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            sensitivity = trackBar1.Value;
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            resetAll();
        }
        private void resetAll()
        {
            trackBar1.Value = 0;
            sensitivity = 0;
            colorV = 0;
            brightnesV = 255;
            ValueToColor(colorV);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2(this);
            frm.Show();
            FormValues.Message = panel1.BackColor;
        }
    }
}
