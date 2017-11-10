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
            int puerto = 3002;
            cliente_udp.crear_socket(puerto);
            cliente_udp.obtenerMAC();
            cliente_udp.forma = this;
            cliente_udp.hilo_escucha.Start();
            cliente_tcp.forma = this;
            mesas = new List<IPEndPoint>();
            fichas = new List<Ficha>();
            listView1.MultiSelect = false;
            juego = new Juego(this.ModificarJugador);
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
                    listView1.Visible = button1.Visible = button2.Visible = false;
                    label13.Text = "Esperando Fichas...";
                    label2.Visible = label3.Visible = label4.Visible = label5.Visible = true;
                    label6.Visible = label7.Visible = label8.Visible = label9.Visible = true;
                    label10.Visible = label11.Visible = label12.Visible = label13.Visible = true;
                }
                else
                {
                    listView1.Visible = button1.Visible = button2.Visible = true;
                    label2.Visible = label3.Visible = label4.Visible = label5.Visible = false;
                    label6.Visible = label7.Visible = label8.Visible = label9.Visible = false;
                    label10.Visible = label11.Visible = label12.Visible = label13.Visible = false;
                }
            }
        }

        public void ModificarFicha(Ficha ficha, int i)
        {
            if (this.InvokeRequired)
            {
                parametro_ficha_entero delegado = new parametro_ficha_entero(ModificarFicha);
                object[] parametros = new object[] { ficha, i };
                this.Invoke(delegado, parametros);
            }
            else
            {
                switch (i)
                {
                    case 1:
                        label2.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        break;
                    case 2:
                        label3.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        break;
                    case 3:
                        label4.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        break;
                    case 4:
                        label5.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        break;
                    case 5:
                        label6.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        break;
                    case 6:
                        label7.Text = ficha.entero_uno + " | " + ficha.entero_dos;
                        break;
                    case 7:
                        label8.Text = ficha.entero_uno + " | " + ficha.entero_dos;
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
                    case 1:
                        label9.Text = linea;
                        break;
                    case 2:
                        label10.Text = linea;
                        break;
                    case 3:
                        label11.Text = linea;
                        break;
                    case 4:
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
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
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

        private void button3_Click(object sender, EventArgs e)
        {
            cliente_udp.Reiniciar();
        }
    }
}
