﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domino_cliente
{
    public class Jugador
    {
        List<ValorFicha> fichas = new List<ValorFicha>();
        public List<int> pases = new List<int>();
        string nombre;
        public string identificador;
        int puntuacion;
        int pos;
        public bool yo = false;
        public parametro_string_entero modificarLabel;

        public void agregarPase(int i)
        {
            pases.Add(i);
        }

        public Jugador(string nombre, string identificador, int pos, parametro_string_entero delegado)
        {
            this.nombre = nombre;
            this.identificador =
            this.identificador = identificador;
            this.pos = pos;
            modificarLabel = delegado;
            //modificarLabel(label(), pos);
        }

        public void agregarFicha(ValorFicha f)
        {
            fichas.Add(f);
        }

        public void setNombre(string nombre)
        {
            this.nombre = nombre;
            modificarLabel(label(), pos);
        }

        public string label()
        {
            if(yo)
                return "yo: " + identificador + ": " + puntuacion;
            else
                return identificador + ": " + puntuacion;
        }

        public string getNombre()
        {
            return nombre;
        }

        public void setPuntuacion(int puntuacion)
        {
            this.puntuacion = puntuacion;
            modificarLabel(label(), pos);
        }

        public int getPuntuacion()
        {
            return puntuacion;
        }
    }
}
