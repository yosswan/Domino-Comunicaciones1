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
        public static void GenerarJugada(MensajeGeneral mensaje, bool primeraJugada)
        {
            /*forma.juego: accede al objeto juego
             * juego.fichas: fichas del juego en la mesa
             * juego.pos1(inicia en 0 y se mueve hacia alante) y juego.pos2(inicia en 27 hacia atras): indican las posiciones en el vector fichas de las puntas
             * juego.punta1 y juego.punta2: indican los valores de las puntas del juego
             * juego.jugadores[x].fichas: fichas jugadas por el jugador x
             * enviar_Jugada(f.getToken(), bool): envia ficha por la punta: 1(true), 2(false)
             * forma.fichas: fichas del jugador
             * mensaje.punta1 y mensaje.punta2 accede a los valores de las puntas enviadas por el servidor
             * forma.fichas.RemoveAt(i): elimina la ficha i de la coleccion
             * enviar_Jugada(new Token(), false): indica al servidor que el jugador pasa
             * juego.jugadores[x].pases: tiene los valores con los que ha pasado el jugador x
             * juego.mano: posicion del jugador que tiene la mano
             */

            

            if (!primeraJugada)
            {

                int n = -1;
                foreach (Ficha f in forma.fichas)
                {
                    if (f.entero_uno == mensaje.punta_dos || f.entero_dos == mensaje.punta_dos)
                    {
                        enviar_Jugada(f.getToken(), false);
                        n = forma.fichas.IndexOf(f);
                        break;
                    }
                    else if (f.entero_uno == mensaje.punta_uno || f.entero_dos == mensaje.punta_uno)
                    {
                        enviar_Jugada(f.getToken(), true);
                        n = forma.fichas.IndexOf(f);
                        break;
                    }
                }
                if (n != -1)
                {
                    BorrarFicha(n);
                    forma.fichas.RemoveAt(n);
                }
                else
                {
                    enviar_Jugada(new Token(), false);
                }
            }
            else
            {
                int maxDoble = -1, maxFicha = -1, iDoble = -1, iFicha = -1;
                for (int j = 0; j < forma.fichas.Count; j++)
                {
                    if(forma.fichas[j].entero_dos == forma.fichas[j].entero_uno)
                    {
                        if (forma.fichas[j].entero_uno * 2 > maxDoble)
                        {
                            maxDoble = forma.fichas[j].entero_uno * 2;
                            iDoble = j;
                        }
                    }
                    else if (forma.fichas[j].entero_uno + forma.fichas[j].entero_dos > maxFicha)
                    {
                        maxFicha = forma.fichas[j].entero_uno + forma.fichas[j].entero_dos;
                        iFicha = j;
                    }
                }
                if (iDoble > -1)
                {
                    enviar_Jugada(forma.fichas[iDoble].getToken(), true);
                    BorrarFicha(iDoble);
                    forma.fichas.RemoveAt(iDoble);
                }
                else
                {
                    enviar_Jugada(forma.fichas[iFicha].getToken(), true);
                    BorrarFicha(iFicha);
                    forma.fichas.RemoveAt(iFicha);
                }
            }
        }

        static void BorrarFicha(int i)
        {
            for (int j = i; j < forma.fichas.Count - 1; j++)
            {
                forma.ModificarFicha(forma.fichas[i + 1], i);
            }
            forma.ModificarFicha(null, i);
        }

        public static void Reiniciar()
        {
            forma.limpiarLisview();
            forma.fichas.Clear();
            solicitud = false;
            jugando = false;
            
            primeraJugada = true;
            mensajeInicio = false;
            mensajeRonda = false;
            conectado = false;
            corriendo = false;
            forma.ModificarInterfaz(1);
            forma.juego.clear();
            forma.visibilidadBoton3(false);
            socket.DropMulticastGroup(IPAddress.Parse(multicastIP));
            for (int i = 0; i < 4; i++)
            {
                forma.ModificarJugador("", i);
            }
        }
    }

}