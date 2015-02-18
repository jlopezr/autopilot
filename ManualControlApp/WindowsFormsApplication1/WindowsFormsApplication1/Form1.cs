using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XPlane;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public bool Manual;
        public bool first = true; //first time on manual to start parser and connection
        private float joyposX; //elevator -1 to 1
        private float joyposY; //ailerons -1 to 1
        //private float rudder; //not used
        private float throttle = 0; //0 to 1
        XplaneConnection connection = new XplaneConnection();
        //XplaneParser parser;
        //parser = new XplaneParser(connection);
        private bool check2 = false;


        public Form1()
        {
            InitializeComponent();
            XplanePacketsId.Load(XplaneVersion.Xplane10);
            //XplaneConnection connection = new XplaneConnection(); 
            XplaneParser parser = new XplaneParser(connection); //No se puede poner arriba??
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.button1.Click += new System.EventHandler(this.button1_Click);
            bool w = SystemInformation.MouseWheelPresent;
            this.checkBox2.Click += new System.EventHandler(this.checkbox2_Click);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyPress);
            if (!w)
            {
                check2 = true;
                this.checkBox2.Checked = check2;
            }
            this.textBox3.Text = Convert.ToString(throttle);
        }

        private void checkbox2_Click(object sender, EventArgs e)
        {
            if (!check2)
            {
                check2 = true;
                this.checkBox2.Checked = check2;
            }
            else
            {
                check2 = false;
                this.checkBox2.Checked = check2;
            }
        }


        private void Form1_KeyPress(object sender, KeyEventArgs e)
        {
            if (Manual && check2)
            {
                Keys key = e.KeyCode;
                int keyn = (int)e.KeyValue;
                if (key >= Keys.D0 && key <= Keys.D9)
                {
                    keyn = keyn - (int)Keys.D0;
                    throttle = (float)keyn;
                    throttle = throttle / 10;
                }
                else if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                {
                    keyn = keyn - (int)Keys.NumPad0;
                    throttle = (float)keyn;
                    throttle = throttle / 10;
                }
                else if (key==Keys.Down||key==Keys.Up)
                {
                    if (key == Keys.Up)
                    {
                        if (throttle <= 0.99)
                        {
                            throttle = throttle + 0.01f;
                        }
                        else if (throttle > 0.99)
                        {
                            throttle = 1;
                        }
                    }
                    else
                    {
                        if (throttle >= 0.01)
                        {
                            throttle = throttle - 0.01f;
                        }
                        else if (throttle < 0.01)
                        {
                            throttle = 0;
                        }
                    }
                    
                }
                byte[] cosa = XplanePacketGenerator.JoystickPacket(throttle, joyposX, 0, joyposY);
                connection.SendPacket(cosa);
                this.textBox3.Text = Convert.ToString(throttle);
            }
        }


        private void button1_Click(object sender, System.EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Manual)
            {
                int mouseX = e.X;
                int mouseY = e.Y;
                joyposX = (((float)mouseX) - (this.panel1.Width / 2)) / (this.panel1.Width / 2);
                joyposY = ((((float)mouseY) - (this.panel1.Height / 2)) / (this.panel1.Height / 2));
                connection.SendPacket(XplanePacketGenerator.JoystickPacket(throttle, joyposX, 0, joyposY));
            }
            this.textBox1.Text = Convert.ToString(joyposX);
            this.textBox2.Text = Convert.ToString(joyposY);

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Manual)
            {
                Manual = false;
                joyposX = 0;
                joyposY = 0;
                this.checkBox1.Checked = Manual;
                byte[] cosa = XplanePacketGenerator.JoystickPacket(throttle, joyposX, 0, joyposY);
                connection.SendPacket(cosa); //Center controls
            }
            else
            {
                Manual = true;
                this.checkBox1.Checked = Manual;
                if (first)
                {
                    connection.OpenConnections();
                    XplaneParser parser = new XplaneParser(connection);
                    parser.Start();
                }
                byte[] cosa = XplanePacketGenerator.JoystickPacket(0, 0, 0, 0);
                connection.SendPacket(cosa); //Center controls
            }
        }

        private void panel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Manual && e.Delta > 0 && throttle <= 0.99)
            {
                throttle = throttle + 0.01f;
            }
            else if (Manual && e.Delta < 0 && throttle > 0.01f)
            {
                throttle = throttle - 0.01f;
            }
            else if (Manual && e.Delta < 0 && throttle <= 0.01f)
            {
                throttle = 0;
            }
            else if (Manual && e.Delta > 0 && throttle > 0.99)
            {
                throttle = 1;
            }
            byte[] cosa = XplanePacketGenerator.JoystickPacket(throttle, joyposX, 0, joyposY);
            connection.SendPacket(cosa);
            this.textBox3.Text = Convert.ToString(throttle);
        }

        void panel1_MouseLeave(object sender, EventArgs e)
        {
            if (panel1.Focused)
                panel1.Parent.Focus();
        }

        void panel1_MouseEnter(object sender, EventArgs e)
        {
            if (!panel1.Focused)
                panel1.Focus();
        }

    }
}
