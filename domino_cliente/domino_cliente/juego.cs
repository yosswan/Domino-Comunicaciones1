﻿using System;
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
        public int mano;
        parametro_string_entero actualizarJugadores;
        public string nombre = "jugador";
        public string identificador;
        Form1 forma;

        public Juego(parametro_string_entero delegado, Form1 forma)
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            jugadores = new List<Jugador>();
            cantidadJugadores = 0;
            actualizarJugadores = delegado;
            Random r = new Random();
            nombre += r.Next(25) + r.Next(25);
            this.forma = forma;
        }

        public void clear()
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
            jugadores.Clear();
            cantidadJugadores = 0;
        }

        public void Reiniciar()
        {
            pos2 = 0;
            pos1 = 27;
            punta1 = punta2 = -1;
        }

        public void quitarJugador(string jugador)
        {
            Jugador aux = null;
            foreach (var item in jugadores)
            {
                if (item.identificador == jugador)
                    aux = item;
            }
            jugadores.Remove(aux);
            cantidadJugadores--;
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

        public void agregarJugadores(DatosJugador[] datos)
        {
            foreach (var item in datos)
            {
                agregarJugador(item);
            }
        }

        void agregarJugador(DatosJugador d)
        {
            if (d.nombre == nombre)
                identificador = d.identificador;
            else
            {
                jugadores.Add(new Jugador(d.nombre, d.identificador, cantidadJugadores, actualizarJugadores));
                forma.ModificarJugador(jugadores[cantidadJugadores].label(), cantidadJugadores);
                cantidadJugadores++;
            }
        }

        public void agregarFicha(ValorFicha f, bool punta, string jugador)
        {
            if (f != null)
            {
                foreach (Jugador j in jugadores)
                {
                    if (j.identificador == jugador)
                    {
                        j.agregarFicha(f);
                    }
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
}
