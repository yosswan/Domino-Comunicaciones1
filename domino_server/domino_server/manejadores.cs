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

        public static bool seccion_critica = false;

        public static void atenderJugadorUDP(IPEndPoint ip)
        {
            if (!forma.juego.jugando && forma.juego.jugadores.Count < 4)
                enviar_Mesa(ip);
        }

        public static void atenderJugadorTCP(string nombre, int pos)
        {
            if (!seccion_critica)
            {
                seccion_critica = true;
                if (!forma.juego.jugando && forma.juego.jugadores.Count < 4)
                {
                    tiempo = 0;
                    string identificador = "jugador " + forma.juego.jugadores.Count;
                    forma.juego.agregarJugador(nombre, identificador, (IPEndPoint)server_tcp.clientes[pos].RemoteEndPoint);
                    enviar_Disponibilidad(pos, multicastIP, identificador);

                }
                seccion_critica = false;
            }
        }

        public static bool atenderMensajeJugador(MensajeDeJugador mj, IPEndPoint ip)
        {
            if (mj.ficha != null)
            {
                return forma.juego.agregarFicha(mj.ficha, mj.punta, ip);
            }
            return false;
        }
    }

}