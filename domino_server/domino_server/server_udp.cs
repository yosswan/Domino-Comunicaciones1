using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace domino_server
{
    partial class server_udp
    {
        public static UdpClient socket;
        public static IPEndPoint enlace;
        public static bool corriendo = true;

        public static int puerto = 3001;
        public static string multicastIP = "224.1.0.24";
        public static IPEndPoint multicastEndPoint = new IPEndPoint(IPAddress.Parse(multicastIP), puerto);

        public static int clientes = 0;
        public static int tiempo = 0;
        

        public static Form1 forma;

        public static Thread hilo_escucha = new Thread(new ThreadStart(recibir_data));

        public static void recibir_data()
        {
            socket = new UdpClient(puerto);
            byte[] buffer;
            IPEndPoint ipRemota = new IPEndPoint(IPAddress.Any, 0);

            while (corriendo && !forma.juego.jugando)
            {
                
                if (socket.Available == 0) 
                {
                    Thread.Sleep(100);
                    continue; 
                }
                
                buffer = socket.Receive(ref ipRemota);
                MensajeJugador obj = ReadToJugador(buffer);
                string datos = obj.identificador;

                if (datos != "DOMINOCOMUNICACIONESI")
                    continue;

                if (!forma.juego.jugando)
                {
                    enviar_Mesa(ipRemota);

                }
            } 
        }

        public static void enviar_data( byte[] buffer, IPEndPoint destino)
        {
            try
            {
                destino.Port = 3002;
                socket.Send(buffer, buffer.Length, destino);
                destino.Port = 3003;
                socket.Send(buffer, buffer.Length, destino);
                destino.Port = 3004;
                socket.Send(buffer, buffer.Length, destino);
                destino.Port = 3005;
                socket.Send(buffer, buffer.Length, destino);
            }
            catch
            {

            }
        }

        public static void enviar_Mesa(IPEndPoint ip)
        {
            enviar_data(ObjectToByte(new Mesa(forma.nombre_mesa)), ip);
        }

        public static void enviar_Disponibilidad(int pos, string multicastIP, string identificador)
        {
            server_tcp.enviar_data(ObjectToByte(new Disponibilidad(multicastIP, identificador)), pos);
        }

        public static void enviar_Fichas(Ficha[] fichas, int pos)
        {
            server_tcp.enviar_data(ObjectToByte(new Fichas(fichas)), pos);
        }

        public static void enviar_MensajeDeJuego(string jugador, int punta1, int punta2, Evento evento)
        {
            enviar_data(ObjectToByte(new MensajeDeJuego(jugador, punta1, punta2, evento)), multicastEndPoint);
        }

        public static void enviar_FinDePartida(string jugador, string motivo, Puntaje[] puntuacion)
        {
            enviar_data(ObjectToByte(new FinDePartida(jugador, motivo, puntuacion)), multicastEndPoint);
        }

        public static void enviar_FinDeRonda(string jugador, string motivo, int puntuacion)
        {
            enviar_data(ObjectToByte(new FinDeRonda(jugador, motivo, puntuacion)), multicastEndPoint);
        }

        public static void enviar_InicioRonda()
        {
            enviar_data(ObjectToByte(new InicioRonda(forma.juego.ronda)), multicastEndPoint);
        }

        public static void enviar_InicioDeJuego(DatosJugador[] jugadores)
        {
            enviar_data(ObjectToByte(new InicioDeJuego(jugadores)), multicastEndPoint);
        }

    }
}
