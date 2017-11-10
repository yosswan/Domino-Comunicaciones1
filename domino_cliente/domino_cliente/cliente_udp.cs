using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Net.NetworkInformation;

namespace domino_cliente
{
    partial class cliente_udp
    {
        public static UdpClient socket;
        public static bool corriendo = false;
        public static bool jugando = false;


        public static string IP_Broadcast = "255.255.255.255";
        public static IPEndPoint IP_servidor;
        public static string direccionMAC;
        public static string multicastIP;
        public static int PUERTO_SERVIDOR = 3001;
        public static NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

        public static Form1 forma;

        public static Thread hilo_escucha = new Thread(new ThreadStart(recibir_data));

        public static void crear_socket(int puerto)
        {
            socket = new UdpClient(puerto);
        }

        public static void obtenerMAC()
        {
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.Name.Contains("Ethernet"))
                    direccionMAC = adapter.GetPhysicalAddress().ToString();
            }
        }

        public static void recibir_data()
        {
            IPEndPoint ipRemota = new IPEndPoint(IPAddress.Any, 0);

            corriendo = true;

            while (corriendo)
            {
                if (socket.Available == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                byte[] buffer = socket.Receive(ref ipRemota);
                if (!jugando)
                    if(!solicitud)
                        AtenderMesa(ReadToMesa(buffer), ipRemota);
                    else if (!mensajeInicio)
                        AtenderInicioDeJuego(ReadToInicioDeJuego(buffer));
                    else if (!mensajeRonda)
                        AtenderInicioRonda(ReadToInicioRonda(buffer));
                    else
                        AtenderMensajeGeneral(ReadToMensajeGeneral(buffer));
                else
                    AtenderMensajeGeneral(ReadToMensajeGeneral(buffer));
            }
        }

        public static void enviar_data(byte[] buffer, IPEndPoint ipRemota)
        {
            try
            {
                socket.Send(buffer, buffer.Length, ipRemota);
            }
            catch
            {
                cliente_udp.Reiniciar();
            }
        }

        public static void enviar_paquete()
        {
            enviar_data(ObjectToByte(new Paquete()), new IPEndPoint(IPAddress.Parse(IP_Broadcast), PUERTO_SERVIDOR));
        }

        public static void enviar_jugador(string nombre, IPEndPoint ip)
        {
            if (!solicitud)
            {
                IP_servidor = ip;
                cliente_tcp.ip = ip;
                if (cliente_tcp.crearSocket())
                {
                    cliente_tcp.hilo_escucha.Start();
                    if (!cliente_tcp.enviar_data(ObjectToByte(new NombreJugador(nombre))))
                    {
                        cliente_tcp.corriendo = false;
                        cliente_tcp.desconectar();
                    }
                    else
                        solicitud = true;
                }
            }
        }

        public static void enviar_verificacion()
        {
            cliente_tcp.enviar_data(ObjectToByte(new Verificacion(forma.juego.identificador)));
        }

        public static void enviar_Jugada(Token f, bool punta)
        {
            cliente_tcp.enviar_data(ObjectToByte(new Jugada(f, punta)));
        }

    }
}
