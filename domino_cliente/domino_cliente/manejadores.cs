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

namespace domino_cliente
{
    partial class cliente_udp
    {
        public static bool primeraJugada = true;
        public static bool solicitud = false;
        public static bool conectado = false;
        public static Thread hiloVerificacion;

        private static void tareaHiloVerificacion()
        {
            while (jugando)
            {
                enviar_verificacion();
                Thread.Sleep(9000);
            }
        }

        public static void AtenderMesa(Mesa mesa, string ip)
        {
            forma.agregar_item(mesa.nombre_mesa);
            forma.mesas.Add(ip);
        }

        public static void AtenderDisponibilidad(Disponibilidad disponibilidad)
        {
            if (disponibilidad.disponible)
            {
                socket.JoinMulticastGroup(IPAddress.Parse(disponibilidad.multicast_ip));
                conectado = true;
                forma.ModificarInterfaz(2);
            }
            else
            {
                forma.actualizar();
                solicitud = false;
            }
        }

        public static void AtenderFichas(Fichas fichas)
        {
            for (int i = 0; i < fichas.fichas.Length; i++)
            {
                forma.fichas.Add(fichas.fichas[i]);
                forma.ModificarFicha(fichas.fichas[i], i);
            }
        }

        public static void AtenderMensajeGeneral(MensajeGeneral mensaje)
        {
            if (mensaje.tipo == 0)
                if (primeraJugada)
                {
                    jugando = true;
                    hiloVerificacion = new Thread(new ThreadStart(tareaHiloVerificacion));
                    if (mensaje.jugador == direccionMAC)
                    {
                        GenerarJugada(mensaje, true);
                    }
                }
                else
                {
                    forma.juego.agregarFicha(mensaje.evento_pasado.ficha, mensaje.evento_pasado.punta, mensaje.evento_pasado.jugador);
                    if (mensaje.jugador == direccionMAC)
                    {
                        GenerarJugada(mensaje, false);
                    }
                }
            else if (mensaje.tipo == 1)
            {
                forma.fichas.Clear();
                forma.juego.actualizarPuntuacion(mensaje.jugador, mensaje.puntuacion);
            }
            else if (mensaje.tipo == 2)
            {
                foreach (Puntaje p in mensaje.puntuacion_general)
                {
                    forma.juego.actualizarPuntuacion(p.jugador, p.puntuacion);
                    forma.visibilidadBoton3(true);
                }
            }
        }
    }
}