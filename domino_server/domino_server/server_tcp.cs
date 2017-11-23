using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace domino_server
{
    class server_tcp
    {
        public static TcpListener listener;
        public static Socket[] clientes = new Socket[4];
        public static Form1 forma;
        public static int cant_clientes = 0;
        public static int max_clientes;
        public static bool[] hilos = new bool[4];

        public static Thread[] hilos_escucha = new Thread[4];

        public static Thread hilo_recepcion = new Thread(new ThreadStart(recepcion_clientes));
        private static int tiempo = 0;

        public static void recepcion_clientes()
        {
            listener = new TcpListener(server_udp.puerto);
            listener.Start();

            /*while (!server_udp.corriendo)
            {
                Thread.Sleep(10);
            }*/

            while ((cant_clientes <= 4 && !forma.juego.jugando) && server_udp.corriendo)
            {
                if (!forma.juego.jugando)
                {
                    if (tiempo > 0)
                    {
                        forma.cambiar_label("Espera: " + tiempo / 10.0 + " s");
                    }
                }
                if(listener.Pending())
                {
                    tiempo = 0;   
                    clientes[cant_clientes]=listener.AcceptSocket();
                    hilos_escucha[cant_clientes] = new Thread(recibir_data);
                    hilos_escucha[cant_clientes].Start(cant_clientes);
                    cant_clientes++;
                    if (cant_clientes == 4)
                    {
                        Thread.Sleep(100);
                        forma.juego.jugando = true;
                        forma.cambiar_label("Jugando...");
                        forma.juego.iniciar();
                    }
                }
                else 
                {
                    if (cant_clientes >= 2 && !forma.juego.jugando)
                    {
                        tiempo++;
                        if (tiempo == 100)
                        {
                            tiempo = 0;
                            forma.juego.jugando = true;
                            forma.cambiar_label("Jugando...");
                            forma.juego.iniciar();
                        }
                    }
                    Thread.Sleep(100);
                }
            }

        }

        public static void recibir_data(object param)
        {
            
            int pos = (int)param;
            hilos[pos] = true;
            Byte[] buffer;
            while (server_udp.corriendo && hilos[pos])
            {
                int data = 0;
                data = clientes[pos].Available;

                if (data == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                else
                {
                    buffer = new Byte[data];
                }

                int cont = clientes[pos].Receive(buffer);

                if (!forma.juego.jugando)
                {
                    MensajeJugador mensaje = server_udp.ReadToJugador(buffer);
                    server_udp.atenderJugadorTCP(mensaje.nombre_jugador, pos);
                }
                else
                {
                    MensajeDeJugador mensaje = server_udp.ReadToMensajeDeJugador(buffer);
                    if (!server_udp.atenderMensajeJugador(mensaje, (IPEndPoint)clientes[pos].RemoteEndPoint))
                        desconectarJugador(pos, false);
                }

            }
        }

        public static void desconectarJugador(int pos, bool eliminar)
        {
            hilos[pos] = false;
            clientes[pos].Close();
            int menos = 0;
            for (int i = 0; i < pos; i++)
            {
                if (!hilos[i])
                    menos++;
            }
            if(eliminar)
                forma.juego.EliminarJugador(pos - menos);
        }

        public static void enviar_data(byte[] buffer, int pos)
        {
            try
            {
                clientes[pos].Send(buffer);
            }
            catch
            {
                desconectarJugador(pos, true);
            }
        }

    }
}
