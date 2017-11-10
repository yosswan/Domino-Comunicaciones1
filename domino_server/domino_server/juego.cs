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
        public int ronda = 0, saque = 0;

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
            forma.limpiarLisview();
            forma.cambiar_label("Eventos:");
            jugando = true;
            DatosJugador[] datos = new DatosJugador[jugadores.Count];
            forma.agregar_linea("Jugadores:");
            for (int i = 0; i < jugadores.Count; i++)
            {
                datos[i] = jugadores[i].getDatos();
                forma.agregar_linea(datos[i].ToString());
            }
            server_udp.enviar_InicioDeJuego(datos);
            forma.agregar_linea("Mensaje de inicio de juego enviado a " + server_udp.multicastIP);
            server_udp.enviar_InicioRonda();
            forma.agregar_linea("Mensaje de inicio de ronda enviado a " + server_udp.multicastIP);
            repartirFichas();
            server_udp.enviar_MensajeDeJuego(jugadores[saque].getNombre(), punta1, punta2, evento_pasado);
            forma.agregar_linea("El juego inicia con: " + jugadores[saque].getNombre());
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
                string mensaje = "";
                forma.agregar_linea("fichas enviadas a " + jugadores[i].getNombre() + ":");
                for (int j = 0; j < 7; j++)
                {
                    r = random.Next(fichas.Count);
                    if (ronda == 0 && fichas[r].entero_dos == fichas[r].entero_uno && fichas[r].entero_uno == 6)
                        saque = i;
                    jugadores[i].agregarFicha(fichas[r]);
                    mensaje += fichas[r].ToString();
                    fichas.RemoveAt(r);
                }
                server_udp.enviar_Fichas(jugadores[i].fichas.ToArray(), jugadores[i].ip);
                forma.agregar_linea(mensaje);
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

            if (t.token == "-1")
            {
                string mensaje = jugadores[turno].getNombre() + " pasó, el turno es de ";
                if(turno == mano)
                    if (mano < jugadores.Count - 1)
                        mano++;
                    else
                        mano = 0;
                Evento aux = new Evento(2, jugadores[turno].getNombre(), null, punta);
                if (turno < jugadores.Count - 1)
                    turno++;
                else
                    turno = 0;
                evento_pasado = aux;
                server_udp.enviar_MensajeDeJuego(jugadores[turno].getNombre(), punta1, punta2, evento_pasado);
                mensaje += jugadores[turno].getNombre();
                forma.agregar_linea(mensaje);
            }

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
                forma.agregar_linea(jugadores[turno].getNombre() + " jugo la ficha " + f.ToString() + "por la punta " + (punta?"uno":"dos"));
                jugadores[turno].fichas.Remove(f);
                bool trancado = comprobarTranca();
                if (jugadores[turno].fichas.Count != 0 && !trancado)
                {
                    Evento aux = new Evento(0, jugadores[turno].getNombre(), f.getValorFicha(), punta);
                    if (turno < jugadores.Count - 1)
                        turno++;
                    else
                        turno = 0;
                    evento_pasado = aux;
                    server_udp.enviar_MensajeDeJuego(jugadores[turno].getNombre(), punta1, punta2, evento_pasado);
                    forma.agregar_linea("El turno es de: " + jugadores[turno].getNombre());
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
            forma.agregar_linea("Se elimino al jugador " + jugadores[i].getNombre());
            if (i == saque)
                if (saque == 0)
                    saque = jugadores.Count - 1;
                else
                    saque--;
            if (i == mano)
                if (mano < jugadores.Count - 1)
                    mano++;
                else
                    mano = 0;
            Evento aux = new Evento(1, jugadores[turno].getNombre(), null, false);
            jugadores.RemoveAt(i);
            if (jugadores.Count == 1)
            {
                string motivo = "desconexion de los demas jugadores";
                Puntaje[] puntuacion = new Puntaje[jugadores.Count];
                puntuacion[0] = new Puntaje(jugadores[0].getNombre(), jugadores[0].getPuntuacion());
                server_udp.enviar_FinDePartida(jugadores[0].getNombre(), motivo, puntuacion);
                forma.agregar_linea("El jugador " + jugadores[0].getNombre() + " ganó la partida por motivo: " + motivo);
                return;
            }
            evento_pasado = aux;
            server_udp.enviar_MensajeDeJuego(jugadores[turno].getNombre(), punta1, punta2, evento_pasado);
            forma.agregar_linea("El turno es de: " + jugadores[turno].getNombre());
        }

        void Reiniciar()
        {
            foreach (var item in jugadores)
            {
                item.fichas.Clear();
            }
            if (saque < jugadores.Count - 1)
                saque++;
            else
                saque = 0;
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            inicializarJuego();
            server_udp.enviar_InicioRonda();
            forma.agregar_linea("Comienzo de ronda " + ronda);
            repartirFichas();
            server_udp.enviar_MensajeDeJuego(jugadores[saque].getNombre(), punta1, punta2, evento_pasado);
            forma.agregar_linea("El turno de saque es de: " + jugadores[turno].getNombre());
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
                forma.agregar_linea("El jugador " + jugadores[c].getNombre() + " ganó la partida por motivo: " + motivo);
            }
            else
            {
                server_udp.enviar_FinDeRonda(jugadores[c].getNombre(), motivo, jugadores[c].getPuntuacion());
                Reiniciar();
                forma.agregar_linea("El jugador " + jugadores[c].getNombre() + " ganó la ronda por motivo: " + motivo + " con " + jugadores[c].getPuntuacion() + "puntos");
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
