using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace bejeweled_multiplayer_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public int size;
        public int color;
        public int timer;
        public string name;
        TcpClient tcpClient = new TcpClient();
        conect conn;
        Thread connThread;
        Stream stm;
        ASCIIEncoding buffSal;
       /* int puerto;
        string IP;*/


        private void button1_Click(object sender, EventArgs e)
        {
            Form form2 = new Form2();
            form2.Owner = this;
            form2.Show();
           
        }
        public void start()
        {
            tableLayoutPanel2.ColumnCount = size;
            tableLayoutPanel2.RowCount = size;
            tableLayoutPanel2.Padding = Padding.Empty;
            tableLayoutPanel2.Margin = Padding.Empty;
            tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            for (int i = 0; i < size * size; i++)
            {
                Button button = new Button();
                button.AllowDrop = true;
                button.Text = "";
                button.TabStop = false;
                button.Padding = Padding.Empty;
                button.Margin = Padding.Empty;
                button.Dock = DockStyle.Fill;
                button.MouseDown += this.button1_MouseDown;
                button.MouseUp += this.button1_MouseUp;
                tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / size));
                tableLayoutPanel2.Controls.Add(button);
                button.DragDrop += this.button1_DragDrop;
                button.DragEnter += this.button1_DragEnter;
                button.BackColor = Color.White;
            }
            //TODO llenar()
            //label4.Text = timer.ToString();
            label5.Text = size.ToString();
            label6.Text = color.ToString();
            button1.Enabled = false;
            timer1.Enabled = true;
            timer1.Start();
        }
        private void button1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }
        private void button1_DragDrop(object sender, DragEventArgs e)
        {
            Button a = sender as Button;
            string temp = e.Data.GetData(DataFormats.Text).ToString();
            string d=tableLayoutPanel2.GetPositionFromControl(sender as Control).ToString();
            string[] ab = temp.Split(',');
            Button ba = tableLayoutPanel2.GetControlFromPosition(Int32.Parse(ab[0]), Int32.Parse(ab[1])) as Button;
            ba.Text = "";
            //TODO cliente debe mandar al servidor el movimiento, no debe llamar a move
            try
            {
                byte[] byteSal = buffSal.GetBytes(temp + "-" + d);
                stm.Write(byteSal, 0, byteSal.Length);
                
            }
            catch (Exception ext)
            {
                MessageBox.Show(ext.Message);
            }
        }
        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            Button a = sender as Button;
            a.Text = "S";
            string temp = tableLayoutPanel2.GetPositionFromControl(sender as Control).ToString();
            a.DoDragDrop(temp, DragDropEffects.Move);
        }
        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            this.label2.Focus();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer--;
            label4.Text = timer.ToString();
            if (timer<=0)
            {
                tableLayoutPanel2.Enabled = false;
            } 
        }

        public delegate void refresh_scoredelegate(String clients);
        public void refresh_score(String clients)
        {
            if (tableLayoutPanel1.InvokeRequired)
            {
                tableLayoutPanel1.Invoke(new refresh_scoredelegate(refresh_score), clients);
            }
            else
            {
                //primero se borran todos los labels del score
                while (tableLayoutPanel1.Controls.Count > 0)
                {
                    tableLayoutPanel1.Controls.RemoveAt(0);
                }
                //ahora se deben agregar los titulos de Name y Score
                Label Name = new Label();
                Name.Text = "Name";
                Label Score = new Label();
                Score.Text = "Score";
                Score.TextAlign = Name.TextAlign = ContentAlignment.MiddleCenter;
                Score.Font = Name.Font = new Font(Name.Font, FontStyle.Bold);

                tableLayoutPanel1.Controls.Add(Name);
                tableLayoutPanel1.Controls.Add(Score);
                //ahora se agregan los clientes
                foreach (String cli in clients.Split(';'))
                {
                    String[] client = cli.Split(',');
                    Name = new Label();
                    Name.Text = client[0];
                    Score = new Label();
                    Score.Text = client[1];
                    tableLayoutPanel1.Controls.Add(Name);
                    tableLayoutPanel1.Controls.Add(Score);
                }

            }

        }


        public delegate void cambiar_scoredelegate(string cliente, string score);
        public void cambiar_score(string cliente, string score)
        {
            if (tableLayoutPanel1.InvokeRequired)
            {
                object[] arg = { cliente , score};
                tableLayoutPanel1.Invoke(new cambiar_scoredelegate(cambiar_score), arg);
            }
            else
            {
                if (cliente != null)
                {
                    //se busca el label con el score (nombre) del cliente
                    int index = 0;
                    foreach (Label item in tableLayoutPanel1.Controls)
                    {
                        if (item.Text == cliente)
                        {
                            index = tableLayoutPanel1.Controls.IndexOf(item);
                            break;
                        }
                    }
                        index = index / 2;
                    //aqui se cambia el score del label por el que tiene el cliente
                    tableLayoutPanel1.GetControlFromPosition(1, index).Text = score;
                }
            }

        }

        public delegate void addclientdelegate(string name);
        public void addclient(string name)
        {
            if (this.tableLayoutPanel1.InvokeRequired)
            {
                this.tableLayoutPanel1.Invoke(new addclientdelegate(this.addclient), name);
            }
            else
            {
                //TODO comprobar que no exista ya
                int index = -1;
                //se busca si exite un label con el nombre del nuevo cliente
                foreach (Label item in tableLayoutPanel1.Controls)
                {
                    if (item.Text == name)
                    {
                        index = tableLayoutPanel1.Controls.IndexOf(item);
                        break;
                    }
                }
                if (index == -1)
                {//entonces no existe
                    Label a = new Label();
                    a.Text = name;
                    Label b = new Label();
                    b.Text = "0";
                    tableLayoutPanel1.Controls.Add(a);
                    tableLayoutPanel1.Controls.Add(b);
                }
            }

        }

        public delegate void removeclientdelegate(string name);
        public void removeclient(string name)
        {
            if (this.tableLayoutPanel1.InvokeRequired)
            {
                this.tableLayoutPanel1.Invoke(new removeclientdelegate(this.removeclient), name);
            }
            else
            {
                int index = 0;
                //se busca el label con el nombre del cliente
                foreach (Label item in tableLayoutPanel1.Controls)
                {
                    if (item.Text == name)
                    {
                        index = tableLayoutPanel1.Controls.IndexOf(item);
                        break;
                    }
                }
                if (index != 2)
                {
                    index = index / 2;
                }
                //se remueven los label de nombre y de score
                tableLayoutPanel1.Controls.Remove(tableLayoutPanel1.GetControlFromPosition(index, 1));
                tableLayoutPanel1.Controls.Remove(tableLayoutPanel1.GetControlFromPosition(index, 0));
            }
        }

        private bool IsEmpty(bool[,] a, bool[,] b)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (a[i, j] != b[i, j])
                    {
                        return false;  // If one is not equal, the two arrays differ
                    }
                }
            }
            return true;
        }

        public delegate void llenardelegate(string msg);
        public void llenar(string msg)
        {
            if (this.tableLayoutPanel2.InvokeRequired)
            {
                this.tableLayoutPanel2.Invoke(new llenardelegate(this.llenar), msg);
            }
            else
            {
                Button but;
                msg = msg.Substring(1);
                string[] array = msg.Split(',');
                Color col;
                int pos = 0;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        but = tableLayoutPanel2.GetControlFromPosition(i, j) as Button;
                        pos = j % size + (i * size);
                        col = Color.FromArgb(Int32.Parse(array[pos]));
                        but.BackColor = col;
                    }
                }
            }
            
        }

        public bool IsTextAValidIPAddress(string text)
        {
            System.Net.IPAddress test;
            return System.Net.IPAddress.TryParse(text, out test);
        }
        public int LogIn(string ip, string puerto, string name)
        {
            try
            {
                if (IsTextAValidIPAddress(ip))
                {
                    int port=Int32.Parse(puerto);
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ip, port);
                    stm = tcpClient.GetStream();
                    buffSal = new ASCIIEncoding();
                    byte[] byteSal = buffSal.GetBytes(name);
                    stm.Write(byteSal, 0, byteSal.Length);
                    byte[] byteLectura = new byte[1024];
                    int k = stm.Read(byteLectura, 0, 1024);
                    string txtMensajes = "";
                    int correctlogin;
                    for (int i = 0; i < k; i++)
                    {
                        txtMensajes += Convert.ToChar(byteLectura[i]);
                    }
                    if (txtMensajes.Contains("_X001Y_*"))
                    {//aceptado
                        correctlogin = 0;
                    }
                    else
                    {
                        if (txtMensajes == "_X001E_")
                        {//ya existe el nombre
                            correctlogin = 2;
                        }
                        else
                        {
                            if (txtMensajes == "_X001X_")
                            {//el server esta lleno
                                correctlogin = 3;
                            }
                            else
                            {//error no especificado
                                correctlogin = 1;
                            }
                        }
                    }

                    if (correctlogin == 0)
                    {
                        string[] info=txtMensajes.Split('*');
                        //cod,size, color, timer
                        size = Int32.Parse(info[1]);
                        color = Int32.Parse(info[2]);
                        timer = Int32.Parse(info[3]);
                        //tcpClient.Close();
                        //stm.Close();

                        

                        start();
                        llenar(info[4]);
                        conn = new conect(stm, this);
                        connThread = new Thread(conn.connect);
                        connThread.Start();
                        return 0;
                    }
                    else
                    {
                        return correctlogin;
                    }
                }
                else
                {
                    return 4;
                }
            }
            catch (Exception ext)
            {
                MessageBox.Show(ext.Message);
                return 1;
            }
        }

        public void msg(string tp)
        {
            MessageBox.Show(tp);
        }

        public void disc()
        {
            try
            {
                tcpClient.Close();
                conn.STOP();
                //connThread.Abort();
                this.Close();
                this.Dispose();
            }
            catch (Exception ext)
            {
                MessageBox.Show(ext.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO desconectar del servidor
            try
            {
                tcpClient.Close();
                conn.STOP();
                this.Dispose();
                // connThread.Abort();
            }
            catch (Exception ext)
            {

            }
        }
    }

}

