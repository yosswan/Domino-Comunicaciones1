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
        public static void GenerarJugada(MensajeGeneral mensaje, bool primeraJugada)
        {
            if (!primeraJugada)
            {
                int n = -1;
                foreach (Ficha f in forma.fichas)
                {
                    if (f.entero_uno == mensaje.punta_dos || f.entero_dos == mensaje.punta_dos)
                    {
                        enviar_Jugada(f, false);
                        n = forma.fichas.IndexOf(f);
                        break;
                    }
                    else if (f.entero_uno == mensaje.punta_uno || f.entero_dos == mensaje.punta_uno)
                    {
                        enviar_Jugada(f, true);
                        n = forma.fichas.IndexOf(f);
                        break;
                    }
                }
                if (n != -1)
                    forma.fichas.RemoveAt(n);
                else
                    enviar_Jugada(new Ficha(), false);
            }
            else
            {
                int maxDoble = 0, maxFicha = 0, iDoble = -1, iFicha = -1;
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
                    enviar_Jugada(forma.fichas[iDoble], true);
                    forma.fichas.RemoveAt(iDoble);
                }
                else
                {
                    enviar_Jugada(forma.fichas[iFicha], true);
                    forma.fichas.RemoveAt(iFicha);
                }
            }
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