using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace domino_cliente
{
    public delegate void parametro_string(string msj);
    public delegate void parametro_vacio();
    public delegate void parametro_bool(bool b);
    public delegate void parametro_entero(int i);
    public delegate void parametro_ficha_entero(Ficha v, int i);
    public delegate void parametro_string_entero(string s, int a);

    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            Random r = new Random();
            //puertos aleatorios para pruebas en una pc
            int puerto = cliente_udp.PUERTO_SERVIDOR;
            
            cliente_udp.obtenerMAC();
            cliente_udp.forma = this;
            
            cliente_tcp.forma = this;
            mesas = new List<IPEndPoint>();
            fichas = new List<Ficha>();
            listView1.MultiSelect = false;
            juego = new Juego(this.ModificarJugador, this);
            mesa = new Label[28];
            inicializar_vector_mesa();
        }

        void inicializar_vector_mesa()
        {
            mesa[0] = label25;
            mesa[1] = label24;
            mesa[2] = label23;
            mesa[3] = label22;
            mesa[4] = label21;
            mesa[5] = label20;
            mesa[6] = label40;
            mesa[7] = label41;
            mesa[8] = label26;
            mesa[9] = label27;
            mesa[10] = label28;
            mesa[11] = label29;
            mesa[12] = label30;
            mesa[13] = label31;
            mesa[14] = label32;
            mesa[15] = label33;
            mesa[16] = label34;
            mesa[17] = label35;
            mesa[18] = label36;
            mesa[19] = label37;
            mesa[20] = label39;
            mesa[21] = label38;
            mesa[22] = label14;
            mesa[23] = label15;
            mesa[24] = label16;
            mesa[25] = label19;
            mesa[26] = label18;
            mesa[27] = label17;
        }

        public void agregar_item(string linea)
        {
            if (this.InvokeRequired)
            {
                parametro_string delegado = new parametro_string(agregar_item);
                object[] parametros = new object[] { linea };
                this.Invoke(delegado, parametros);
            }
            else
            {
                bool ban = false;
                foreach (var item in listView1.Items)
                {
                    if (item.ToString().Contains(linea))
                    {
                        ban = true;
                        break;
                    }
                }
                if(!ban)
                    listView1.Items.Add(linea);
            }
        }

        public void cambiarRonda(string linea)
        {
            if (this.InvokeRequired)
            {
                parametro_string delegado = new parametro_string(cambiarRonda);
                object[] parametros = new object[] { linea };
                this.Invoke(delegado, parametros);
            }
            else
            {
                label13.Text = linea;
                foreach (var item in mesa)
                {
                    item.Visible = false;
                }
            }
        }

        public void ModificarInterfaz(int i)
        {
            if (this.InvokeRequired)
            {
                parametro_entero delegado = new parametro_entero(ModificarInterfaz);
                object[] parametros = new object[] { i };
                this.Invoke(delegado, parametros);
            }
            else
            {
                if (i == 2)
                {
                    listView1.Visible = button2.Visible = false;
                    label13.Text = "Esperando Fichas...";
                    label13.Visible = true;
                    panel1.Visible = true;
                }
                else
                {
                    listView1.Visible = button2.Visible = true;
                    label2.Visible = label3.Visible = label4.Visible = label5.Visible = false;
                    label6.Visible = label7.Visible = label8.Visible = label9.Visible = false;
                    label10.Visible = label11.Visible = label12.Visible = label13.Visible = false;
                    textBox1.Visible = true;
                    button4.Visible = true;
                    panel1.Visible = false;
                    foreach (var item in mesa)
                    {
                        item.Visible = false;
                    }
                }
            }
        }

        public void AgregarFichaMesa(Ficha ficha, int i)
        {
            if (this.InvokeRequired)
            {
                parametro_ficha_entero delegado = new parametro_ficha_entero(AgregarFichaMesa);
                object[] parametros = new object[] { ficha, i };
                this.Invoke(delegado, parametros);
            }
            else
            {
                if(i < 6 || i > 21)
                    mesa[i].Text = "| " + ficha.entero_uno + " | " + ficha.entero_dos + " |";
                else if(i > 7 && i < 20)
                    mesa[i].Text = "| " + ficha.entero_dos + " | " + ficha.entero_uno + " |";
                else if(i == 6 || i == 7)
                    mesa[i].Text = "--\n" + ficha.entero_uno + "\n--\n" + ficha.entero_dos + "\n--";
                else
                    mesa[i].Text = "--\n" + ficha.entero_dos + "\n--\n" + ficha.entero_uno + "\n--";
                mesa[i].Visible = true;
            }
        }

        public void ModificarFicha(Ficha ficha, int i)
        {
            if (this.InvokeRequired)
            {
                parametro_ficha_entero delegado = new parametro_ficha_entero(ModificarFicha);
                object[] parametros = new object[] { ficha, i };
                try
                {
                    this.Invoke(delegado, parametros);
                }
                catch { }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        if (ficha != null)
                        {
                            label2.Visible = true;
                            label2.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        }
                        else
                            label2.Visible = false;
                        break;
                    case 1:
                        if (ficha != null)
                        {
                            label3.Visible = true;
                            label3.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        }
                        else
                            label3.Visible = false;
                        break;
                    case 2:
                        if (ficha != null)
                        {
                            label4.Visible = true;
                            label4.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        }
                        else
                            label4.Visible = false;
                        break;
                    case 3:
                        if (ficha != null)
                        {
                            label5.Visible = true;
                            label5.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        }
                        else
                            label5.Visible = false;
                        break;
                    case 4:
                        if (ficha != null)
                        {
                            label6.Visible = true;
                            label6.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        }
                        else
                            label6.Visible = false;
                        break;
                    case 5:
                        if (ficha != null)
                        {
                            label7.Visible = true;
                            label7.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        }
                        else
                            label7.Visible = false;
                        break;
                    case 6:
                        if (ficha != null)
                        {
                            label8.Visible = true;
                            label8.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        }
                        else
                            label8.Visible = false;
                        break;
                }
            }
        }

        public void ModificarJugador(string linea, int i)
        {
            if (this.InvokeRequired)
            {
                parametro_string_entero delegado = new parametro_string_entero(ModificarJugador);
                object[] parametros = new object[] { linea, i };
                this.Invoke(delegado, parametros);
            }
            else
            {
                switch (i)
                {
                    case 0:
                        label9.Visible = true;
                        label9.Text = linea;
                        break;
                    case 1:
                        label10.Visible = true;
                        label10.Text = linea;
                        break;
                    case 2:
                        label11.Visible = true;
                        label11.Text = linea;
                        break;
                    case 3:
                        label12.Visible = true;
                        label12.Text = linea;
                        break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            actualizar();
        }

        public void actualizar()
        {
            cliente_udp.enviar_paquete();
            limpiarLisview();
        }

        public void limpiarLisview()
        {
            if (this.InvokeRequired)
            {
                parametro_vacio delegado = new parametro_vacio(limpiarLisview);
                this.Invoke(delegado);
            }
            else
            {
                listView1.Items.Clear();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cliente_udp.corriendo = cliente_tcp.corriendo = false;
            try
            {
                cliente_udp.socket.Close();
            }
            catch { }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            cliente_udp.corriendo = true;
            if (listView1.SelectedIndices.Count != 0)
            {
                cliente_udp.enviar_jugador(juego.nombre, mesas.ElementAt(listView1.SelectedIndices[0]));
            }
        }

        public void visibilidadBoton3(bool v)
        {
            if (this.InvokeRequired)
            {
                parametro_bool delegado = new parametro_bool(visibilidadBoton3);
                object[] parametros = new object[] { v };
                this.Invoke(delegado, parametros);
            }
            else
            {
                button3.Visible = v;
                if (v)
                    panel1.Visible = !v;
            }
        }

        public void visibilidadBoton2(bool v)
        {
            if (this.InvokeRequired)
            {
                parametro_bool delegado = new parametro_bool(visibilidadBoton2);
                object[] parametros = new object[] { v };
                this.Invoke(delegado, parametros);
            }
            else
            {
                button2.Visible = v;
            }
        }

        
        public List<IPEndPoint> mesas;
        public List<Ficha> fichas;
        public Juego juego;
        Label[] mesa;

        private void button3_Click(object sender, EventArgs e)
        {
            cliente_udp.Reiniciar();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            cliente_udp.crear_socket(Int32.Parse(textBox1.Text));
            cliente_udp.hilo_escucha.Start();
            button4.Visible = false;
            textBox1.Visible = false;
        }
    }
}
