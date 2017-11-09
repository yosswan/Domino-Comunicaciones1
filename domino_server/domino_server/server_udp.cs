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

namespace domino_server
{
    partial class server_udp
    {
        public static UdpClient socket;
        public static IPEndPoint enlace;
        public static bool corriendo = false;

        public static string multicastIP = "224.1.0.24";
        public static IPEndPoint multicastEndPoint = new IPEndPoint(IPAddress.Parse(multicastIP), 3001);

        public static EndPoint[] ip_clientes = new IPEndPoint[4];
        public static int clientes = 0;
        public static int tiempo = 0;
        public static int puerto = 3001;

        public static Form1 forma;

        public static Thread hilo_escucha = new Thread(new ThreadStart(recibir_data));

        public static void recibir_data()
        {
            socket = new UdpClient(3001);
            byte[] buffer;
            IPEndPoint ipRemota = new IPEndPoint(IPAddress.Any, 0);

            corriendo = true;

            while (corriendo)
            {
                
                if(!forma.juego.jugando)
                {
                    if(tiempo>0)
                    {
                        forma.cambiar_label("Espera: "+tiempo/10.0+" s");
                    }
                    else
                    {
                        //forma.cambiar_label("");
                    }
                }
                
                if (socket.Available == 0) 
                {
                    Thread.Sleep(100);
                    if (clientes >= 2 && !forma.juego.jugando)
                    {
                        tiempo++;
                        if(tiempo==100)
                        {
                            tiempo = 0;
                            forma.juego.jugando = true;
                            forma.cambiar_label("Jugando...");
                        }
                    }
                    continue; 
                }
                
                buffer = socket.Receive(ref ipRemota);
                MensajeJugador obj = ReadToJugador(buffer);
                string datos = obj.identificador;

                if (datos != "DOMINOCOMUNICACIONESI")
                    continue;

                if (!forma.juego.jugando)
                {
                    tiempo = 0;
                    ip_clientes[clientes] = ipRemota;
                    //enviar_data(new IPEndPoint(IPAddress.Parse(multicastIP), ipRemota.Port), "Jugador " + (clientes + 1));
                    forma.agregar_linea(clientes + " " + ipRemota.ToString());
                    clientes++;

                    if(clientes==4)
                    {
                        forma.juego.jugando = true;
                        forma.cambiar_label("Jugando...");
                    }

                }
                else
                {
                    //enviar_data(ipRemota, "Mesa llena");
                }
            } 
        }

        public static void enviar_data( byte[] buffer, IPEndPoint destino)
        {
            socket.Send(buffer, buffer.Length, destino);
        }

        public static void enviar_Mesa(IPEndPoint ip)
        {
            enviar_data(ObjectToByte(new Mesa(forma.nombre_mesa)), ip);
        }

        public static void enviar_Disponibilidad(IPEndPoint ip, string multicastIP, bool disponible)
        {
            enviar_data(ObjectToByte(new Disponibilidad(multicastIP, disponible)), ip);
        }

        public static void enviar_Fichas(Ficha[] fichas, IPEndPoint ip)
        {
            enviar_data(ObjectToByte(new Fichas(fichas)), ip);
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

    }
}
