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
        List<Ficha> fichas, pozo;
        Ficha[] juego = new Ficha[28];
        public List<Jugador> jugadores;
        public int punta2, punta1, pos2, pos1, mano = 0, mano_inicial = 0, turno = 0;
        public bool jugando = false;
        Evento evento_pasado;
        Form1 forma;
        bool primeraJugada = true;
        public int ronda = 0, baseToken = 0;

        public Juego(Form1 form)
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            jugadores = new List<Jugador>();
            fichas = new List<Ficha>();
            pozo = new List<Ficha>();
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
            turno = mano_inicial;
            server_udp.enviar_MensajeDeJuego(jugadores[mano_inicial].identificador, punta1, punta2, evento_pasado);
            forma.agregar_linea("El juego inicia con: " + jugadores[mano_inicial].identificador);
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
                forma.agregar_linea("fichas enviadas a " + jugadores[i].identificador + ":");
                for (int j = 0; j < 7; j++)
                {
                    r = random.Next(fichas.Count);
                    jugadores[i].agregarFicha(fichas[r]);
                    mensaje += fichas[r].ToString();
                    fichas.RemoveAt(r);
                }
                server_udp.enviar_Fichas(jugadores[i].fichas.ToArray(), jugadores[i].pos);
                forma.agregar_linea(mensaje);
            }

            bool doble = false;
            int maxDoble = -1, max = -1, imax = 0, imaxDoble = 0;
            for (int i = 0; i < this.fichas.Count; i++)
            {
                if (!fichas.Contains(this.fichas[i]))
                {
                    if (this.fichas[i].isDoble())
                    {
                        doble = true;
                        if (this.fichas[i].getPintas() > maxDoble)
                        {
                            maxDoble = this.fichas[i].getPintas();
                            imaxDoble = i;
                        }
                    }
                    else
                    {
                        if (this.fichas[i].getPintas() > max)
                        {
                            max = this.fichas[i].getPintas();
                            imax = i;
                        }
                    }
                }
            }

            if(ronda == 0)
                for (int i = 0; i < jugadores.Count; i++)
                {
                    if (doble)
                    {
                        if (jugadores[i].fichas.Contains(this.fichas[imaxDoble]))
                        {
                            mano_inicial = i;
                            break;
                        }
                    }else
                        if (jugadores[i].fichas.Contains(this.fichas[imax]))
                        {
                            mano_inicial = i;
                            break;
                        }
                }
            pozo = fichas;
            
        }

        private string generarToken()
        {
            baseToken++;
            return "ficha" + baseToken;
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
                if (j.identificador == nombre)
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
            forma.agregar_linea("Token que mando al servidor: " + t.token);
            if (t.token == "-1")
            {
                string mensaje = jugadores[turno].identificador + " pasó, el turno es de ";
                if(turno == mano)
                    if (mano < jugadores.Count - 1)
                        mano++;
                    else
                        mano = 0;
                Evento aux = new Evento(2, jugadores[turno].identificador, null, punta);
                if (turno < jugadores.Count - 1)
                    turno++;
                else
                    turno = 0;
                evento_pasado = aux;
                server_udp.enviar_MensajeDeJuego(jugadores[turno].identificador, punta1, punta2, evento_pasado);
                mensaje += jugadores[turno].identificador;
                forma.agregar_linea(mensaje);
                return true;
            }

            Ficha f = null;

            if (jugadores[turno].ip == ip)
            {
                f = jugadores[turno].buscarToken(t);
                if(f != null)
                    forma.agregar_linea("Ficha correspondiente: " + f.ToString());
            }
            else
            {
                forma.agregar_linea("Ip no coincide");
                ban = false;
                EliminarJugador(turno);
                return false;
            }

            if (f != null)
            {
                if (primeraJugada)
                {
                    primeraJugada = false;
                    punta1 = f.entero_uno;
                    punta2 = f.entero_dos;
                    juego[pos2] = f;
                    pos2++;
                }
                else if (punta)
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
                        forma.agregar_linea("no coincidio ficha");
                        EliminarJugador(turno);
                        return false;
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
                        forma.agregar_linea("no coincidio ficha");
                        EliminarJugador(turno);
                        return false;
                    }
                }
            }
            else
            {
                forma.agregar_linea("No se encontro ficha");
                ban = false;
                EliminarJugador(turno);
                return false;
            }
            if (ban)
            {
                forma.agregar_linea(jugadores[turno].identificador + " jugo la ficha " + f.ToString() + "por la punta " + (punta ? "uno" : "dos"));
                jugadores[turno].fichas.Remove(f);
                bool trancado = comprobarTranca();
                if (jugadores[turno].fichas.Count != 0 && !trancado)
                {
                    Evento aux = new Evento(0, jugadores[turno].identificador, f.getValorFicha(), punta);
                    if (turno < jugadores.Count - 1)
                        turno++;
                    else
                        turno = 0;
                    evento_pasado = aux;
                    server_udp.enviar_MensajeDeJuego(jugadores[turno].identificador, punta1, punta2, evento_pasado);
                    forma.agregar_linea("El turno es de: " + jugadores[turno].identificador);
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

        public void EliminarJugador(int i)
        {
            /*//trampita
            if (i >= jugadores.Count)
                i = jugadores.Count - 1;
            //*/
            forma.agregar_linea("Se elimino al jugador " + jugadores[i].identificador);

            if (i == mano)
                if (mano < jugadores.Count - 1)
                    mano++;
                else
                    mano = 0;
            if (i == mano_inicial)
                if (mano_inicial == 0)
                    mano_inicial = jugadores.Count - 1;
                else
                    mano_inicial--;
            if (i == jugadores.Count - 1)
                turno = 0;
            Evento aux = new Evento(1, jugadores[i].identificador, null, false);
            pozo.AddRange(jugadores[i].fichas);
            jugadores.RemoveAt(i);
            if (jugadores.Count == 1)
            {
                string motivo = "desconexion de los demas jugadores";
                Puntaje[] puntuacion = new Puntaje[jugadores.Count];
                puntuacion[0] = new Puntaje(jugadores[0].identificador, jugadores[0].getPuntuacion());
                server_udp.enviar_FinDePartida(jugadores[0].identificador, motivo, puntuacion);
                forma.agregar_linea("El jugador " + jugadores[0].identificador + " ganó la partida por motivo: " + motivo);
                return;
            }
            if (jugadores.Count == 0)
            {
                string motivo = "desconexion de todos los jugadores";
                server_udp.enviar_FinDePartida("ninguno", motivo, null);
                forma.agregar_linea("Se acabo el juego por desconexion de todos los jugadores");
                return;
            }
            evento_pasado = aux;
            server_udp.enviar_MensajeDeJuego(jugadores[turno].identificador, punta1, punta2, evento_pasado);
            forma.agregar_linea("El turno es de: " + jugadores[turno].identificador);
        }

        void Reiniciar()
        {
            //validar desconexion
            foreach (var item in jugadores)
            {
                item.fichas.Clear();
            }
            if (mano_inicial < jugadores.Count - 1)
                mano_inicial++;
            else
                mano_inicial = 0;
            pos2 = 0;
            pos1 = 27;
            mano = mano_inicial;
            turno = mano;
            punta1 = punta2 = -1;
            inicializarJuego();
            primeraJugada = true;
            ronda++;
            server_udp.enviar_InicioRonda();
            forma.agregar_linea("Comienzo de ronda " + ronda);
            repartirFichas();
            server_udp.enviar_MensajeDeJuego(jugadores[mano_inicial].identificador, punta1, punta2, evento_pasado);
            forma.agregar_linea("El turno de saque es de: " + jugadores[mano_inicial].identificador);
        }

        void comprobarFinal(string motivo, int c)
        {
            jugadores[c].setPuntuacion(calcularPuntos(c));
            if (jugadores[c].getPuntuacion() >= 100)
            {
                Puntaje[] puntuacion = new Puntaje[jugadores.Count];
                for (int i = 0; i < jugadores.Count; i++)
                {
                    puntuacion[i] = new Puntaje(jugadores[i].identificador, jugadores[i].getPuntuacion());
                }
                server_udp.enviar_FinDePartida(jugadores[c].identificador, motivo, puntuacion);
                forma.visibilidadBoton(true);
                forma.agregar_linea("El jugador " + jugadores[c].identificador + " ganó la partida por motivo: " + motivo);
            }
            else
            {
                server_udp.enviar_FinDeRonda(jugadores[c].identificador, motivo, jugadores[c].getPuntuacion());
                forma.agregar_linea("El jugador " + jugadores[c].identificador + " ganó la ronda por motivo: " + motivo + " con " + jugadores[c].getPuntuacion() + " puntos");
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

            foreach (var item in pozo)
            {
                vector[item.entero_uno]++;
                vector[item.entero_dos]++;
            }

            for (int i = 0; i < 28; i++)
            {
                if (juego[i] != null)
                {
                    vector[juego[i].entero_uno]++;
                    vector[juego[i].entero_dos]++;
                }
            }
            if (vector[punta1] == 8 && vector[punta2] == 8)
                return true;
            return false;
        }
    }
}
