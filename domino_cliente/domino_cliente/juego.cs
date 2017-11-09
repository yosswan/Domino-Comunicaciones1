using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domino_cliente
{
    public class Juego
    {
        ValorFicha[] fichas = new ValorFicha[28];
        public List<Jugador> jugadores;
        public int cantidadJugadores;
        public int punta2, punta1, pos2, pos1;
        parametro_string_entero actualizarJugadores;

        public Juego(parametro_string_entero delegado)
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            jugadores = new List<Jugador>();
            cantidadJugadores = 0;
            actualizarJugadores = delegado;
        }

        public void clear()
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            jugadores.Clear();
            cantidadJugadores = 0;
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

        public void agregarFicha(ValorFicha f, bool punta, string jugador)
        {
            bool ban = false;
            foreach (Jugador j in jugadores)
            {
                if (j.getNombre() == jugador)
                {
                    j.agregarFicha(f);
                    ban = true;
                }
            }
            if (!ban)
            {
                jugadores.Add(new Jugador(jugador, cantidadJugadores, actualizarJugadores));
                jugadores.ElementAt(cantidadJugadores).agregarFicha(f);
                cantidadJugadores++;
            }

            if (punta)
            {
                if (f.entero_uno == punta1)
                {
                    punta1 = f.entero_dos;
                }
                else if (f.entero_dos == punta1)
                {
                    punta1 = f.entero_uno;
                }
                fichas[pos1] = f;
                pos1--;
            }
            else
            {
                if (f.entero_uno == punta2)
                {
                    punta2 = f.entero_dos;
                }
                else if (f.entero_dos == punta2)
                {
                    punta2 = f.entero_uno;
                }
                fichas[pos2] = f;
                pos2++;
            }
        }
    }
}
