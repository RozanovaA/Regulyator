using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private double poloj_private;
        private double davl;
        private double ust;
        private int t=1;
        
        bool KVO;
        bool KVZ;

        public double Poloj
            {
            get
            {
                return poloj_private;
            }
            set
            {
                if (value > 100) { value = 100; }
                if (value < 0) { value = 0; }
                poloj_private = value;

                if (value > 95)
                {
                    KVO = true;
                    textBox2.Text = "true";
                    textBox3.Clear();
                }
                else if (value < 5)
                {
                    KVZ = true;
                    textBox3.Text = "true";
                    textBox2.Clear();
                }
                else
                {
                    KVZ = false; KVO = false;
                    textBox2.Clear();
                    textBox3.Clear();
                }

                //  
                
                textBox1.Text = Poloj.ToString();
            }
        }
        public double PIreg()
        {


            return 0;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        public static double integ;

        private void timer1_Tick(object sender, EventArgs e)
        {
            double P;
            try
            {
                davl = Convert.ToDouble(textBox5.Text);
                ust = Convert.ToDouble(textBox4.Text);

            }
            catch (Exception)
            {
                davl = 0;
                ust = 0;
            }
           
            P = ust - davl;
            double chast;
            if ((-0.2<P)&&(P<0.2))
            {
                integ = 0;
            }
            double a = Math.Abs(P);
            if ((a>0.2)&&(a<0.9))
            {
                integ=integ+P; 
                
                if (integ>1)
                {
                    Poloj -= P;
                    integ = 0;
                }
                if (integ < -1)
                {
                    Poloj -= P;
                    integ = 0;
                }

            }

            if ((P>0.9)||(P<-0.9))
            {
                chast = P * 0.5;
                Poloj -= chast;
            
            }         
            
     
            davl = (100- Poloj) / 20;
            textBox5.Text = davl.ToString();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
