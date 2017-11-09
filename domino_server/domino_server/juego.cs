using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace domino_server
{
    public class Juego
    {
        List<Ficha> fichas;
        Ficha[] juego = new Ficha[28];
        public List<Jugador> jugadores;
        public int punta2, punta1, pos2, pos1, mano = 0, turno = 0;
        public bool jugando = false;
        Evento evento_pasado;
        Form1 forma;
        public int ronda = 0;

        public Juego(Form1 form)
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            jugadores = new List<Jugador>();
            fichas = new List<Ficha>();
            cargarFichas();
            evento_pasado = null;
            forma = form;
            inicializarJuego();
        }

        void inicializarJuego()
        {
            for (int i = 0; i < 28; i++)
            {
                juego[i] = null;
            }
        }

        void cargarFichas()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = i; j < 7; j++)
                {
                    Ficha ficha = new Ficha(generarToken(), i, j);
                    fichas.Add(ficha);
                }
            }
        }

        public void iniciar()
        {
            jugando = true;
            repartirFichas();
            server_udp.enviar_MensajeDeJuego(jugadores[0].getNombre(), punta1, punta2, evento_pasado);
            forma.limpiarLisview();
            forma.cambiar_label("Eventos:");
        }

        public void repartirFichas()
        {
            List<Ficha> fichas = new List<Ficha>();
            foreach (var item in this.fichas)
            {
                fichas.Add(item);
            }

            Random random = new Random();
            int r;
            for (int i = 0; i < jugadores.Count; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    r = random.Next(fichas.Count);
                    jugadores[i].agregarFicha(fichas[r]);
                    fichas.RemoveAt(r);
                }
                server_udp.enviar_Fichas(jugadores[i].fichas.ToArray(), jugadores[i].ip);
            }
            
        }

        private string generarToken()
        {
            return "";
        }

        public void clear()
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            jugadores.Clear();
            inicializarJuego();
            jugando = false;
            forma.cambiar_label("Clientes Conectados:");
            forma.limpiarLisview();
            forma.visibilidadBoton(false);
        }

        public void actualizarPuntuacion( string nombre, int puntos)
        {
            foreach (Jugador j in jugadores)
            {
                if (j.getNombre() == nombre)
                {
                    j.setPuntuacion(puntos);
                    break;
                }
            }
        }

        public void agregarJugador(string nombre, string identificador, IPEndPoint ip)
        {
            jugadores.Add(new Jugador(nombre, identificador, ip, jugadores.Count, forma.agregar_linea));
        }

        public bool agregarFicha(Token t, bool punta, IPEndPoint ip)
        {
            bool ban = true;
            Ficha f = null;
            if (jugadores[turno].ip == ip)
            {
                f = jugadores[turno].buscarToken(t);
            }
            else
            {
                ban = false;
                EliminarJugador(turno);
            }

            if (f != null)
            {

                if (punta)
                {
                    if (f.entero_uno == punta1)
                    {
                        punta1 = f.entero_dos;
                        juego[pos1] = f;
                        pos1--;
                    }
                    else if (f.entero_dos == punta1)
                    {
                        punta1 = f.entero_uno;
                        juego[pos1] = f;
                        pos1--;
                    }
                    else
                    {
                        ban = false;
                        EliminarJugador(turno);
                    }
                }
                else
                {
                    if (f.entero_uno == punta2)
                    {
                        punta2 = f.entero_dos;
                        juego[pos2] = f;
                        pos2++;
                    }
                    else if (f.entero_dos == punta2)
                    {
                        punta2 = f.entero_uno;
                        juego[pos2] = f;
                        pos2++;
                    }
                    else
                    {
                        ban = false;
                        EliminarJugador(turno);
                    }
                }
            }
            else
            {
                ban = false;
                EliminarJugador(turno);
            }
            if (ban)
            {
                jugadores[turno].fichas.Remove(f);
                bool trancado = comprobarTranca();
                if (jugadores[turno].fichas.Count != 0 && !trancado)
                {
                    Evento aux = new Evento(0, jugadores[0].getNombre(), f.getValorFicha(), punta);
                    if (turno < jugadores.Count - 1)
                        turno++;
                    else
                        turno = 0;
                    evento_pasado = aux;
                    server_udp.enviar_MensajeDeJuego(jugadores[turno].getNombre(), punta1, punta2, evento_pasado);
                }
                else
                {
                    if (trancado)
                    {
                        string motivo = "tranca - ";
                        int min = 64;
                        List<int> candidatos = new List<int>();
                        for (int i = 0; i < jugadores.Count; i++)
                        {
                            int pintas = jugadores[i].calcularPintas();
                            if (pintas < min)
                            {
                                candidatos.Clear();
                                min = pintas;
                                candidatos.Add(i);
                            }
                            else if (pintas == min)
                            {
                                candidatos.Add(i);
                            }
                        }

                        if (candidatos.Count == 1)
                        {
                            motivo += "cantidad minima de pintas";
                            comprobarFinal(motivo, candidatos[0]);
                        }
                        else
                        {
                            if (candidatos.Contains(turno))
                            {
                                motivo += "ejecuto la jugada de tranca";
                                comprobarFinal(motivo, turno);
                            }
                            else
                            {
                                while (!candidatos.Contains(mano))
                                {
                                    if (mano < jugadores.Count - 1)
                                        mano++;
                                    else
                                        mano = 0;
                                }
                                motivo += "jugador con la mano";
                                comprobarFinal(motivo, mano);
                            }
                        }
                    }
                    else
                    {
                        string motivo = "domino";
                        comprobarFinal(motivo, turno);
                    }
                }
            }

            return ban;
        }

        void EliminarJugador(int i)
        {
            Evento aux = new Evento(1, jugadores[turno].getNombre(), null, false);
            jugadores.RemoveAt(i);
            server_udp.enviar_MensajeDeJuego(jugadores[turno].getNombre(), punta1, punta2, evento_pasado);
        }

        void Reiniciar()
        {
            foreach (var item in jugadores)
            {
                item.fichas.Clear();
            }
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            inicializarJuego();
            repartirFichas();
            server_udp.enviar_MensajeDeJuego(jugadores[0].getNombre(), punta1, punta2, evento_pasado);
        }

        void comprobarFinal(string motivo, int c)
        {
            jugadores[c].setPuntuacion(calcularPuntos(c));
            if (jugadores[c].getPuntuacion() >= 100)
            {
                Puntaje[] puntuacion = new Puntaje[jugadores.Count];
                for (int i = 0; i < jugadores.Count; i++)
                {
                    puntuacion[i] = new Puntaje(jugadores[i].getNombre(), jugadores[i].getPuntuacion());
                }
                server_udp.enviar_FinDePartida(jugadores[c].getNombre(), motivo, puntuacion);
                forma.visibilidadBoton(true);
            }
            else
            {
                server_udp.enviar_FinDeRonda(jugadores[c].getNombre(), motivo, jugadores[c].getPuntuacion());
                Reiniciar();
            }
        }

        int calcularPuntos(int ganador)
        {
            int acum = 0;
            for (int i = 0; i < jugadores.Count; i++)
            {
                if (i != ganador)
                    acum += jugadores[i].calcularPintas();
            }
            return acum;
        }

        bool comprobarTranca()
        {
            int[] vector = new int[7];
            for (int i = 0; i < 28; i++)
            {
                if (juego[i] != null)
                {
                    vector[juego[i].entero_uno]++;
                    vector[juego[i].entero_dos]++;
                }
            }
            for (int i = 0; i < 7; i++)
            {
                if (vector[i] == 8)
                    return true;
            }
            return false;
        }
    }
}
