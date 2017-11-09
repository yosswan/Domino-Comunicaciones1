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
        public static string IP_servidor;
        public static string multicastIP = "224.1.0.24";
        public static string direccionMAC;
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
                string datos = Encoding.Default.GetString(buffer, 0, buffer.Length);
                forma.agregar_item(datos);
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

        public static void enviar_jugador(string nombre, string ip)
        {
            IP_servidor = ip;
            enviar_data(ObjectToByte(new NombreJugador(nombre)), new IPEndPoint(IPAddress.Parse(ip), PUERTO_SERVIDOR));
        }

        public static void enviar_verificacion()
        {
            enviar_data(ObjectToByte(new Verificacion(direccionMAC)), new IPEndPoint(IPAddress.Parse(IP_servidor), PUERTO_SERVIDOR));
        }

        public static void enviar_Jugada(Ficha f, bool punta)
        {
            enviar_data(ObjectToByte(new Jugada(f, punta)), new IPEndPoint(IPAddress.Parse(IP_servidor), PUERTO_SERVIDOR));
        }

    }
}
