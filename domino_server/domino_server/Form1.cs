using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace domino_server
{
    public delegate void parametro_string_entero(string s, int i);
    public delegate void parametro_string(string s);
    public delegate void parametro_bool(bool b);
    delegate void parametro_vacio();

    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            juego = new Juego(this);
            server_tcp.forma = this;
            server_tcp.hilo_recepcion.Start();
        }

        public void cambiar_label(string msj)
        {
            if (this.InvokeRequired)
            {
                parametro_string delegado = new parametro_string(cambiar_label);
                object[] parametros = new object[] { msj };
                try
                {
                    this.Invoke(delegado, parametros);
                }catch(Exception e){}
            }
            else
            {
                label1.Text = msj;
            }
        }

        public void agregar_linea(string linea)
        {
            if (this.InvokeRequired) 
            {
                parametro_string delegado = new parametro_string(agregar_linea);
                object[] parametros = new object[] { linea };
                this.Invoke(delegado, parametros);
            }
            else
            {
                listView1.Items.Add(linea);
            } 
        }

        public void visibilidadBoton(bool b)
        {
            if (this.InvokeRequired)
            {
                parametro_bool delegado = new parametro_bool(visibilidadBoton);
                object[] parametros = new object[] { b };
                this.Invoke(delegado, parametros);
            }
            else
            {
                button1.Visible = b; ;
            }
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

        private void Form1_Load(object sender, EventArgs e)
        {
            server_udp.forma = this;
            server_udp.hilo_escucha.Start();
            server_udp.hiloInformacion.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            server_udp.corriendo = false;
            server_udp.socket.Close();
            //server_udp.socket.DropMulticastGroup(server_udp.multicastEndPoint.Address);
        }

        public string nombre_mesa = "Mesa Mi Esfuerzo :p";
        public Juego juego;

        private void button1_Click(object sender, EventArgs e)
        {
            juego.clear();
        }
    }
}
