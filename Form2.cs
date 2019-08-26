using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bejeweled_multiplayer_client
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 a = Owner as Form1;
            switch (a.LogIn(textBox2.Text, textBox3.Text, textBox1.Text))
            {
                case (0):
                    MessageBox.Show("Bienvenido");
                    a.name = textBox1.Text;
                    this.Dispose();
                    this.Close();
                    break;
                case (1):
                    MessageBox.Show("Error");
                    break;
                case (2):
                    MessageBox.Show("Nombre ya en uso");
                    break;
                case (3):
                    MessageBox.Show("El server esta lleno");
                    break;
                case (4):
                    MessageBox.Show("IP en formato erroneo");
                    break;
            }
        }
    }
}
