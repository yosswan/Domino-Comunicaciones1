using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace domino_cliente
{
    class cliente_tcp
    {
        public static TcpClient cliente;
        public static bool corriendo = false;
        public static Form1 forma;

        public static IPEndPoint ip;

        public static Thread hilo_escucha = new Thread(new ThreadStart(recibir_data));

        public static bool crearSocket()
        {
            cliente = new TcpClient();
            try
            {
                cliente.Connect(ip);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void desconectar()
        {
            cliente.Close();
        }

        public static void recibir_data()
        {
            
            byte[] buffer;

            corriendo = true;

            while (corriendo)
            {
                int data = cliente.Available;
                if (data == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                buffer = new byte[data];
                int cont = cliente.GetStream().Read(buffer, 0, cliente.Available);

                while (cliente_udp.jugando && corriendo)
                {
                    Thread.Sleep(1);
                }

                if (!cliente_udp.conectado)
                    cliente_udp.AtenderDisponibilidad(cliente_udp.ReadToDisponibilidad(buffer));
                else
                {
                    while (!cliente_udp.mensajeRonda && corriendo)
                    {
                        Thread.Sleep(1);
                    }
                    cliente_udp.AtenderFichas(cliente_udp.ReadToFichas(buffer));
                }
            }
        }

        public static bool enviar_data(byte[] buffer)
        {
            try
            {
                cliente.GetStream().Write(buffer, 0, buffer.Length);
                return true;
            }
            catch
            {
                MessageBox.Show("No se pudo enviar mensaje TCP");
                return false;
            }
        }
    }
}
