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
using System.Windows.Forms;

namespace domino_cliente
{
    partial class cliente_udp
    {
        public static bool primeraJugada = true;
        public static bool solicitud = false;
        public static bool conectado = false;
        public static bool mensajeInicio = false;
        public static bool mensajeRonda = false;
        public static Thread hiloVerificacion;

        private static void tareaHiloVerificacion()
        {
            while (jugando)
            {
                enviar_verificacion();
                Thread.Sleep(9000);
            }
        }

        public static void AtenderMesa(Mesa mesa, IPEndPoint ip)
        {
            forma.agregar_item(mesa.nombre_mesa);
            forma.mesas.Add(ip);
            forma.visibilidadBoton2(true);
        }

        public static void AtenderInicioRonda(InicioRonda i)
        {
            forma.cambiarRonda("Ronda: " + i.ronda);
            mensajeRonda = true;
            primeraJugada = true;
        }

        public static void AtenderInicioDeJuego(InicioDeJuego i)
        {
            if (i.jugadores != null)
            {
                forma.juego.agregarJugadores(i.jugadores);
                mensajeInicio = true;
            }
            else
                MessageBox.Show("mensaje erroneo");
        }

        public static void AtenderDisponibilidad(Disponibilidad disponibilidad)
        {
            multicastIP = disponibilidad.multicast_ip;
            socket.JoinMulticastGroup(IPAddress.Parse(disponibilidad.multicast_ip));
            conectado = true;
            forma.juego.identificador = disponibilidad.jugador;
            forma.ModificarInterfaz(2);
        }

        public void FallaConexion()
        {
            forma.actualizar();
            solicitud = false;
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
            if (mensaje.tipo == 3)
                if (primeraJugada)
                {
                    primeraJugada = false;
                    jugando = true;
                    //hiloVerificacion = new Thread(new ThreadStart(tareaHiloVerificacion));
                    if (mensaje.jugador == forma.juego.identificador)
                    {
                        GenerarJugada(mensaje, true);
                    }
                }
                else
                {
                    forma.juego.agregarFicha(mensaje.evento_pasado.ficha, mensaje.evento_pasado.punta, mensaje.evento_pasado.jugador);
                    if (mensaje.jugador == forma.juego.identificador)
                    {
                        GenerarJugada(mensaje, false);
                    }
                }
            else if (mensaje.tipo == 4)
            {
                mensajeRonda = false;
                jugando = false;
                
                forma.fichas.Clear();
                forma.juego.actualizarPuntuacion(mensaje.jugador, mensaje.puntuacion);
                forma.juego.Reiniciar();
                //MessageBox.Show(forma.juego.identificador + " fin de ronda");
            }
            else if (mensaje.tipo == 5)
            {
                if(mensaje.puntuacion_general != null)
                    foreach (Puntaje p in mensaje.puntuacion_general)
                    {
                        forma.juego.actualizarPuntuacion(p.jugador, p.puntuacion);
                    }
                forma.visibilidadBoton3(true);
                jugando = false;
            }
            else if (mensaje.tipo == 6)
            {
                forma.juego.quitarJugador(mensaje.jugador);
            }
        }
    }
}